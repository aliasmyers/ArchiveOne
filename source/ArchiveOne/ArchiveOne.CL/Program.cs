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
using Mikesoft.ArchiveOne;

namespace Mikesoft.ArchiveOne.CL
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

            p.Setup(arg => arg.timerInterval)
             .As('t', "TimerInterval")
             .SetDefault(1000); // use the standard fluent Api to define a default value if non is specified in the arguments

            var result = p.Parse(args);

            if (result.HasErrors == false)
            {
                Application.StartOne app = new Application.StartOne(p.Object);
            }
            else {

                logger.Fatal("Arguments failed to parse. {0}", result.ErrorText);
            }

            Console.ReadKey();
        }
    }
}
