using log4net;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[assembly: log4net.Config.XmlConfigurator()]
namespace UnitTest.Log4Net
{
    [TestClass]
    public class Log4NetTutorial
    {

        public class Bar
        {
            private static readonly ILog log = LogManager.GetLogger(typeof(Bar));

            public void DoIt()
            {
                log.Debug("Did it again!");
            }
        }
        [TestMethod]
        public void Tutorial1_UseLogger()
        {

            ILog log = LogManager.GetLogger(typeof(Log4NetTutorial));

            // Set up a simple configuration that logs on the console.
            BasicConfigurator.Configure();
            log.Info("Entering application.");
            Bar bar = new Bar();
            bar.DoIt();
            log.Info("Exiting application.");
        }

        [TestMethod]
        public void Tutorial2_XmlConfig()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("Log4Net/Tutorial2.xml"));

            ILog log = LogManager.GetLogger(typeof(Log4NetTutorial));

            // Set up a simple configuration that logs on the console.
            log.Info("Entering application.");
            Bar bar = new Bar();
            bar.DoIt();
            log.Info("Exiting application.");
        }
    }
}
