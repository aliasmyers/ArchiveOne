using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikesoft.ArchiveOne.Models
{
    public class ApplicationArguments
    {
        #region Class Properties
        
        //TODO create a argument definition class that is filled in a config file.
        //property name, single letter argument name, full argument name, required, default, description
        
                
        public string directory { get; set; }
        public int timerInterval { get; set; }
        public List<string> files { get; set; }
        public bool silent { get; set; }
        public bool verbose { get; set; }
        

        #endregion

        #region Class Constructors
        
        public ApplicationArguments()
        {
            directory = "";
            timerInterval = 1000;
            silent = false;
            verbose = false;
        }

        #endregion

        #region Class Methods

        public string argumentList()
        {
            string arguments = "directory: " + directory;
            arguments += " timerInterval: " + timerInterval.ToString();
            arguments += " silent: " + Convert.ToString(silent);
            arguments += " verbose: " + Convert.ToString(verbose);

            return arguments;
        }

        /// <summary>
        /// The list of valid arguments for the command line
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> getApplicationParameters()
        {
            Dictionary<string, string> these_args = new Dictionary<string, string>();
            these_args.Add("directory", "");
            these_args.Add("timerInterval", "1000");
            these_args.Add("files", "");
            these_args.Add("silent", "false");
            these_args.Add("verbose", "false");

            return these_args;
        }

        #endregion
    }
}
