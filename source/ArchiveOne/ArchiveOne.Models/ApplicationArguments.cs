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
        
        public string directory { get; set; }
        public int timerInterval { get; set; }
        public bool silent { get; set; }
        public bool verbose { get; set; }
        public List<string> files { get; set; }

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

        #endregion
    }
}
