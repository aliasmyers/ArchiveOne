using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mikesoft.ArchiveOneCL.Models;

namespace ArchiveOneCL.Tests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void TestToStringOverride()
        {
            ApplicationArguments args = new ApplicationArguments();
            args.directory = "directory";
            args.silent = true;
            args.verbose = true;

            Assert.AreEqual("directory: 'directory', silent: True, verbose: True", args.ToString());
        }
    }
}
