using Block.Assorted.Logging.ILogImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Block.Assorted.Logging;
using Block0.Threading.Worker;

namespace Chunk.ThreadLog
{
    public class LogJob : WorkerJob, IUniqueTaskID
    {
        public byte UniqueID => (byte)WorkerJobID.Log;

        private ILog log;

        public override void Init()
        {
            log = new Log4NetImpl();

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
                if (item.MethodParam is LogPipeItem logItem)
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
