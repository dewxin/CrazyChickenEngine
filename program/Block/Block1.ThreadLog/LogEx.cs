using Block.Assorted.Logging;
using Block0.Threading.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chunk.ThreadLog
{
    public static class LogEx
    {
        public static void EnableAsync(this LogExtension logExtension)
        {
            WorkerJobManager.AddJob(new LogJob());
        }
    }
}
