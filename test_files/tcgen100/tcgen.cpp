/* tcgen v1.00 - Test file generator for testing compression programs.

(C) 2011, Dell Inc. Written by Matt Mahoney

    LICENSE

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License as
    published by the Free Software Foundation; either version 3 of
    the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    General Public License for more details at
    Visit <http://www.gnu.org/copyleft/gpl.html>.

This program generates random files with known entropy for testing
data compression programs. A good compressor should compress a
file to close to the displayed size. To create test file:

  tcgen output command

If output is . then the output file name is the same as the command.
If output is - then output to stdout in binary mode.

The command is a string containing patterns of either hyphens
or the letters r, n, w, b, c, v, d, followed by 1 or more digits,
like "n20b5-v2". Scanning left to right, each letter sets a variable
of the same name. A hyphen or the end of command causes R blocks of
N words of length W bytes each in the range C..C+B-1 (mod 256) to be
written. Words are chosen randomly from a vocabulary
of size V, initialized at random without duplicates at the start
of each block. If D > 0 then each output byte has added to it the
D'th previous byte. The default pattern is r1n1w1b256c0v1d0.
Any characters which do not fit a pattern will seed the random
number generator differently in a predictable way, but are otherwise
ignored.

The entropy is calculated to be R * (log(B^W choose V) + N*log(V)),
reducing V to only words which are output at least once, then
displayed to stderr. It is required that V be in 1..B^W, W > 0,
B in 1..256, and C in 0..255.

The random number generator is a RC4 keystream seeded with the
command string (including terminating NUL) as the key. When a
random number in the range 0..n-1 is needed (where n is B or V),
then enough bytes are generated in MSB to LSB order to cover the
range, repeating until the result is in range. The exact
algorithm is as follows:

  (return random number in range 0..n-1)
  random(n)
    max = 0
    value = 0
    do
      while max < n-1 do
        max = max * 256 + 255
        value = value * 256 + RC4() (http://en.wikipedia.org/wiki/RC4)
    until value < n
    return value

  (generate output)
  repeat R times
    k = 0
    buf[0..D-1] = 0
    for i = 0..V-1 (initialize vocabulary)
      do
        for j = 0..W-1
          dict[i][j] = random(B) + C (mod 256)
      while dict[i] = dict[j] for some j < i
    for i = 0..N-1 (output block)
      s = dict[random(V)]
      for j = 0..W-1
        if D > 0 then
          buf[k] = buf[k] + s[j] (mod 256)
          output buf[k]
          k = (k + 1) mod D
        else
          output s[j]
*/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <math.h>
#include <fcntl.h>

typedef unsigned int uint;
typedef unsigned char byte;

void usage() {
  fprintf(stderr, 
    "tcgen v1.00 - File generator for testing data compressors.\n"
    "(C) 2011, Dell Inc. Written by Matt Mahoney %s.\n"
    "Licensed under GPL v3. http://www.gnu.org/copyleft/gpl.html\n"
    "\n"
    "To create a test file and show its entropy: tcgen file command\n"
    "If file is . then command is also the output file name.\n"
    "If file is - then write to standard output in binary mode.\n"
    "Command may contain the following patterns where # is a number:\n"
    "  r# = set number R of blocks to write, default r1\n"
    "  n# = set number N of words per block to write, default n1\n"
    "  k# = set N = # * 1000\n"
    "  m# = set N = # * 1000000\n"
    "  w# = set word length W, default w1\n"
    "  b# = set base (alpabet size) B, range 1..256, default b256\n"
    "  c# = set first character of alphabet C, range 0..255, default c0\n"
    "  v# = set vocabulary size V, range 1..B^W, default v1\n"
    "  d# = set delta offset to D bytes, default d0 = no delta coding\n"
    "  - or end of command = write R blocks of N words of W bytes each from\n"
    "a random vocabulary of V different words each block using bytes in the\n"
    "range C..C+B-1. If D > 0 then add the D'th previous output byte.\n"
    "Examples:\n"
    "  tcgen - b1c72-c73      Write HI to stdout\n"
    "  tcgen . n1000b1        Write 1000 zero bytes to file n1000b1\n"
    "  tcgen x n1000v256      Write 1000 random bytes to file x\n"
    "  tcgen - r100n4c48b2    Write 100 random 0000 or 1111 to stdout\n"
    "  tcgen x n256b1c1d1     Write an ascending byte sequence to file x\n",
    __DATE__);
  exit(1);
}

// Allocate array of n bytes
byte* allocate(uint n) {
  if (n<1) return 0;
  byte* p=(byte*)calloc(n, 1);
  if (!p) fprintf(stderr, "Out of memory\n"), exit(1);
  return p;
}

// RC4 keystream generator.
// See http://en.wikipedia.org/wiki/RC4
class RC4 {
public:
  void init(const char *key, int len);  // key schedule
  int get();  // get 1 byte of pseudo random stream
  uint random(uint n);  // random number in 0..n-1
private:
  byte s[256];
  int i, j;
  void swap(byte& a, byte& b) {
    byte t=a; a=b; b=t;
  }
} rc4;

// Key schedule
void RC4::init(const char *key, int len) {
  j=0;
  for (i=0; i<256; ++i)
    s[i]=i;
  for (i=0; i<256; ++i) {
    j=j+s[i]+key[i%len]&255;
    swap(s[i], s[j]);
  }
  i=j=0;
}

// Get 1 byte of keystream
int RC4::get() {
  i=i+1&255;
  j=j+s[i]&255;
  swap(s[i], s[j]);
  return s[s[i]+s[j]&255];
}

// Random number in range 0..n-1
uint RC4::random(uint n) {
  if (n<1) return 0;
  uint value, range;
  do {
    value=0;
    range=0;
    while (range<n-1) {
      value=value*256+get();
      range=range*256+255;
    }
  } while (value>=n);
  return value;
}

int main(int argc, char** argv) {

  // Get args
  if (argc<3)
    usage();

  // Seed PRNG
  RC4 rc4;
  rc4.init(argv[1], strlen(argv[2])+1);

  // Get output file name
  FILE* out=0;
  const char* filename=argv[1];
  if (!strcmp(filename, ".")) filename=argv[2];
  else if (!strcmp(filename, "-")) filename=0;  // stdout

  // Scan commmand
  double entropy=0;
  double size=0;
  uint r=1, n=1, w=1, b=256, c=0, v=1, d=0;
  bool done=false;
  for (int i=0; !done; ++i) {
    uint x=0;
    if (argv[2][i]) x=atoi(argv[2]+i+1);
    if (argv[2][i]==0 || argv[2][i]=='-' || isdigit(argv[2][i+1])) {
      switch(argv[2][i]) {
        case 'r': r=x; break;
        case 'n': n=x; break;
        case 'k': n=x*1000; break;
        case 'm': n=x*1000000; break;
        case 'w': w=x; break;
        case 'b': b=x; break;
        case 'c': c=x; break;
        case 'v': v=x; break;
        case 'd': d=x; break;
        case 0: done=true;
        case '-':

        // Generate r*n*w output bytes
        {
          // Check for valid b, w, v, c
          if (b<1 || b>256 || w<1 || v<1 || c>255) usage();
          const double wlogb=w*log(b);
          if (log(v)>wlogb+1e-14)
            fprintf(stderr, "v=%u cannot exceed b^w=%1.0f\n",
               v, exp(wlogb)), exit(1);

          // If all is OK then open output
          if (!out) {
            if (filename) {
              out=fopen(filename, "wb");
              if (!out) perror(filename), exit(1);
            }
            else {
#ifndef unix
              setmode(1, O_BINARY);
#endif
              out=stdout;
            }
          }

          byte* buf=allocate(d);     // rotating output buffer
          byte* dict=allocate(v*w);  // vocabulary
          byte* used=allocate(v);    // to marked used vocabulary
          for (uint ir=0; ir<r; ++ir) {
            if (d) memset(buf, 0, d);  // clear output buffer for each block
            memset(used, 0, v);        // mark vocabulary as unused
            uint nused=0;      // number of used[] set

            // Initialize vocabulary to v different words of length w
            for (uint i=0; i<v; ++i) {
              bool match;
              do {
                for (uint j=0; j<w; ++j)
                  dict[i*w+j]=rc4.random(b)+c;
                match=false;
                for (uint j=0; j<i && !match; ++j)
                  if (!memcmp(dict+i*w, dict+j*w, w))
                    match=true;
              } while (match);
            }

            // Output n words of length w
            uint k=0;  // buf index
            for (uint i=0; i<n; ++i) {

              // Pick a random word. If new, add it's entropy
              uint rw=rc4.random(v);
              if (!used[rw]) {
                if (wlogb>50) entropy+=wlogb;
                else entropy+=log(exp(wlogb)-nused);
                entropy-=log(++nused);
                used[rw]=1;
              }

              // Output word, possibly delta coded
              const byte* s=dict+rw*w;
              for (uint j=0; j<w; ++j) {
                if (d>0) {
                  putc(buf[k]+=s[j], out);
                  if (++k>=d) k=0;
                }
                else
                  putc(s[j], out);
              }
            }

            // Calculate entropy
            entropy+=n*log(v);
          }
          size+=double(r)*double(n)*double(w);
          free(used);
          free(dict);
          if (buf) free(buf);
          break;
        }
      }
    }
  }

  // Close output and show entropy
  if (out) {
    if (out!=stdout) fclose(out);
    entropy/=log(256);  // in bytes
    fprintf(stderr,
     "\nGenerated %1.0f bytes with %1.2f bytes entropy (%4.2f%%)\n",
      size, entropy, 100*entropy/(size+1e-20));
    if (filename)
      fprintf(stderr, "to file %s\n", filename);
  }
  else
    fprintf(stderr, "Command should end with . or _ and a number.\n");
  return 0;
}
