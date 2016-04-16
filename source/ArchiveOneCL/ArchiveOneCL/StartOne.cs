using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using NLog;
using Mikesoft.ArchiveOneCL;

namespace Mikesoft.ArchiveOneCL.Application
{
    class StartOne
    {
        //readonly Timer _timer;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public StartOne(Models.ApplicationArguments args)
        {
            logger.Trace("Entering Constructor for {0}", logger.Name);
            logger.Trace("Constructor Init'd with {0}", args);

            //logger.Trace("Init Timer for {0} Milliseconds", "1000");
            //_timer = new Timer(1000) {AutoReset = true};
            //_timer.Elapsed += (sender, eventArgs) => logger.Trace("It is {0} and all is well", DateTime.Now);
    
        }

        //public void Start() { _timer.Start(); }
        //public void Stop() { _timer.Stop(); }
    }
}
