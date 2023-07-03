using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Block.Assorted.Logging;
using Block0.Threading.Worker;

namespace Block1.ThreadLog
{
    /// <summary>
    /// 先获得原有的log implementation，再用这里LogJob在新线程上代理原有的日志调用。
    /// </summary>
    public class LogJob : WorkerJob, IUniqueJobID
    {
        public byte UniqueID => (byte)WorkerJobID.Log;

        private ILog log;

        public override void Awake()
        {
            log = Log.LogImpl;
            if(log == null)
            {
                throw new ArgumentNullException("need to Set Log Implementation first, Log.Init()");
            }

            global::Block.Assorted.Logging.Log.Init(new ThreadLogImpl());
        }

        public override void Execute()
        {
            HandlePipeMsg();

        }

        public void HandlePipeMsg()
        {
            while (TryGetMsg(out var item))
            {
                if (item.MethodParam is LogMsg logItem)
                {
                    switch (logItem.LogLevel)
                    {
                        case LogLevel.Debug:
                            log.Debug(logItem.Message, logItem.CallerInfo);
                            break;

                        case LogLevel.Info:
                            log.Info(logItem.Message, logItem.CallerInfo);
                            break;

                        case LogLevel.Warn:
                            log.Warn(logItem.Message, logItem.CallerInfo);
                            break;

                        case LogLevel.Error:
                            log.Error(logItem.Message, logItem.CallerInfo);
                            break;

                        case LogLevel.Fatal:
                            log.Fatal(logItem.Message, logItem.CallerInfo);
                            break;
                    }

                }
            }
        }
    }
}
