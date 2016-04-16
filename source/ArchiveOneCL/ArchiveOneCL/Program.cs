using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Topshelf;
//using Topshelf.Configurators;
//using Topshelf.Logging;
using NLog;
using Fclp;
using Mikesoft.ArchiveOneCL;

namespace Mikesoft.ArchiveOneCL
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("Starting ArchiveOneCL");
            logger.Trace("Setup Parsing Command-Line");
            
            // create a generic parser for the ApplicationArguments type
            var p = new FluentCommandLineParser<Models.ApplicationArguments>();

            // specify which property the value will be assigned too.
            p.Setup(arg => arg.directory)
             .As('d', "directory") // define the short and long option name
             .Required(); // using the standard fluent Api to declare this Option as required.

            p.Setup(arg => arg.silent)
             .As('s', "silent")
             .SetDefault(false);

            p.Setup(arg => arg.verbose)
             .As('v', "verbose")
             .SetDefault(false); // use the standard fluent Api to define a default value if non is specified in the arguments

            var result = p.Parse(args);

            if (result.HasErrors == false)
            {

                Application.StartOne app = new Application.StartOne(p.Object);

                #region ServiceRegion
                
                
                //When to configuration migrates to the app.config so this can run as a service
                //logger.Trace("Starting Up Service...");
                //HostFactory.New(x => { x.UseNLog(); });
                //HostFactory.Run(x =>                                 //1
                //{
                    
                //    x.Service<Application.StartOne>(s =>                        //2
                //    {
                //        s.ConstructUsing(name => new Application.StartOne(p.Object));     //3
                //        s.WhenStarted(start => start.Start());              //4
                //        s.WhenStopped(start => start.Stop());               //5
                //    });
                //    x.RunAsLocalSystem();                            //6

                //    x.SetDescription("Command Line Application that can run as a Service");        //7
                //    x.SetDisplayName("ArchiveOne");                       //8
                //    x.SetServiceName("ArchiveOne");                       //9
                //});         
                #endregion
            }
            else {
                logger.Fatal("Arguments failed to parse.");
            }

            Console.ReadKey();
        }
    }
}
