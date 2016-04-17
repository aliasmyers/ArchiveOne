using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Configurators;
using Topshelf.Logging;
using NLog;
using Mikesoft.ArchiveOne;

namespace Mikesoft.ArchiveOne.Service
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("Starting ArchiveOneCL");
            logger.Trace("Setup Parsing Command-Line");
            
                #region ServiceRegion
                
                //When to configuration migrates to the app.config so this can run as a service
                logger.Trace("Starting Up Service...");
                //HostFactory.New(x => { x.UseNLog(); });
                HostFactory.Run(x =>
                {
                    x.Service<Application.StartOne>(s =>
                    {
                        ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                        configMap.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + @"Mikesoft.ArchiveOne.Service.exe.config";
                        Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                        s.ConstructUsing(name => new Application.StartOne(config.AppSettings.Settings));
                        s.WhenStarted(start => start.Start());
                        s.WhenStopped(start => start.Stop());
                    });
                    x.RunAsLocalSystem();
                    x.SetDescription("Command Line Application is a Windows Service");
                    x.SetDisplayName("ArchiveOne");
                    x.SetServiceName("ArchiveOne");
                });

                #endregion
            
        }
    }
}
