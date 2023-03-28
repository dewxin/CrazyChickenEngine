using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.Assorted.Logging.ILogImpl
{
    public class Log4NetImpl : ILog
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ILog));

        //https://logging.apache.org/log4net/log4net-2.0.14/release/config-examples.html
        public Log4NetImpl()
        {
            log4net.GlobalContext.Properties["AppName"] = AppDomain.CurrentDomain.FriendlyName;
            log4net.GlobalContext.Properties["StartTime"] = DateTime.Now.ToString(@"yyyy-MM-dd, HH\'mm\'ss");
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
        }

        private void PropagateCallerInfo(CallerInfo data)
        {
            log4net.GlobalContext.Properties["MethodName"] = data.MemberName;
            log4net.GlobalContext.Properties["SimpleFileName"] = data.SourceFilePath.Split('\\', '/').Last();
        }

        //TODO怎么进行AOP编程
        public void Debug(string message, CallerInfo data)
        {
            PropagateCallerInfo(data);
            log.Debug(message);
        }

        public void Error(string message, CallerInfo data)
        {
            PropagateCallerInfo(data);
            log.Error(message);
        }

        public void Fatal(string message, CallerInfo data)
        {
            PropagateCallerInfo(data);
            log.Fatal(message);
        }
        public void Info(string message, CallerInfo data)
        {
            PropagateCallerInfo(data);
            log.Info(message);
        }
        public void Warn(string message, CallerInfo data)
        {
            PropagateCallerInfo(data);
            log.Warn(message);
        }
    }
}
