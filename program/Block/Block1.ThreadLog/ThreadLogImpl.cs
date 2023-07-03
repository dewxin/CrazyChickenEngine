using Block0.Threading.Pipe;
using Block0.Threading.Worker;
using Block.Assorted.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block1.ThreadLog
{


    public class LogMsg: JobMsg
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public CallerInfo CallerInfo { get; set; }
    }

    public class ThreadLogImpl : ILog
    {
        private void PostLog(LogLevel logLevel, string message, CallerInfo callerInfo)
        {
            var pipeItem = new LogMsg
            {
                LogLevel = logLevel,
                Message = message,
                CallerInfo = callerInfo
            };

            WorkerJob.SendMsgToJob((byte)WorkerJobID.Log, pipeItem);
        }

        public void Debug(string message, CallerInfo invokerInfo)
        {
            PostLog(LogLevel.Debug, message, invokerInfo);
        }

        public void Error(string message, CallerInfo invokerInfo)
        {
            PostLog(LogLevel.Error, message, invokerInfo);
        }

        public void Fatal(string message, CallerInfo invokerInfo)
        {
            PostLog(LogLevel.Fatal, message, invokerInfo);
        }

        public void Info(string message, CallerInfo invokerInfo)
        {
            PostLog(LogLevel.Info, message, invokerInfo);
        }

        public void Warn(string message, CallerInfo invokerInfo)
        {
            PostLog(LogLevel.Warn, message, invokerInfo);
        }
    }
}
