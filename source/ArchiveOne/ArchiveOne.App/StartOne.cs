using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using NLog;
using Mikesoft.ArchiveOne.Models;

namespace Mikesoft.ArchiveOne.Application
{
    public class StartOne
    {
        #region Class Properties

	    readonly Timer _timer;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ApplicationArguments args;

        #endregion

        #region Class Constructors 

        public StartOne(ApplicationArguments args)
        {
            logger.Trace("Entering Constructor for {0}", logger.Name);
            logger.Trace("Constructor Init'd with {0}", args.argumentList());

            this.args = args;
        }

        public StartOne(NameValueCollection appSettings)
        {
            ApplicationArguments args = new ApplicationArguments();
            args.directory = appSettings["directory"];
            args.timerInterval = Convert.ToInt32(appSettings["timerInterval"]);
            args.silent = Convert.ToBoolean(appSettings["silent"]);
            args.verbose = Convert.ToBoolean(appSettings["verbose"]);

            this.args = args;

            _timer = new Timer(args.timerInterval) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => ProgramEntry();;
        }

        #endregion

        #region Class Methods

        public void ProgramEntry(){
            logger.Trace("It is {0} and all is well, {1}", DateTime.Now, args.argumentList());

            //if directory is a value try to get the list of directory

            //set the logging level based on silent and verbose

            //get the list of files to operate on

            //operate on the files

            //complete processing
        }

        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }

        #endregion
    }
}