using System;
using System.Collections.Generic;
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
        readonly Timer _timer;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public StartOne(ApplicationArguments args)
        {
            logger.Trace("Entering Constructor for {0}", logger.Name);
            logger.Trace("Constructor Init'd with {0}", args);

        }

        public StartOne(KeyValueConfigurationCollection appSettings)
        {
            int timerInterval = Convert.ToInt32(appSettings["timerInterval"]);
            logger.Trace("Init Timer for {0} Milliseconds", timerInterval);
            _timer = new Timer(timerInterval) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => logger.Trace("It is {0} and all is well", DateTime.Now);
        }

        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }
    }
}