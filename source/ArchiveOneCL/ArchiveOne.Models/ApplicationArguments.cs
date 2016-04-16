using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikesoft.ArchiveOneCL.Models
{
    public class ApplicationArguments
    {
        public string directory { get; set; }
        public bool silent { get; set; }
        public bool verbose { get; set; }

        public override string ToString()
        {
            string toString = "";
            toString = string.Format("directory: '{0}', silent: {1}, verbose: {2}", directory, silent.ToString(), verbose.ToString());
            return toString;
        }
    }
}
