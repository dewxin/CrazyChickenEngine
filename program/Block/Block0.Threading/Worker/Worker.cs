using Block0.Threading.Pipe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;


namespace Block0.Threading.Worker
{


    public partial class Worker
    {
        public WorkerJob CurrentJob { get; set; }

        public bool NeedStop { get; protected set; }
        AutoResetEvent shutdownEvent = new AutoResetEvent(false);

        public double ElapsedMs { get; protected set; }

        public void Run(Object threadContext)
        {
            TryInitJobs();

            ExecuteJobs();

            shutdownEvent.Set();
        }


        private void TryInitJobs()
        {
            lock (typeof(WorkerJobManager))
            {
                if (WorkerJobManager.workerJobInited)
                    return;

                foreach (var workerJob in WorkerJobManager.id2ManagedJobDict.Values)
                {
                    CurrentJob = workerJob;

                    workerJob.Init();
                }

                WorkerJobManager.workerJobInited = true;
            }
        }


        private void ExecuteJobs()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var lastTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            while (!NeedStop)
            {
                var nowTimeMs = stopwatch.Elapsed.TotalMilliseconds;
                var elapsedMs = nowTimeMs - lastTimeMs;
                lastTimeMs = nowTimeMs;

                ElapsedMs = elapsedMs;

                while (WorkerJobManager.HasJobToHandle(out var workerJob) /*并且剩余时间片的估算值能够处理此次task*/)
                {
                    var prevWorker = Interlocked.CompareExchange(ref workerJob.CurrentWorker, this, null);

                    //不是null说明被其他Worker抢了
                    if (prevWorker != null)
                        continue;

                    CurrentJob = workerJob;

                    CurrentJob.Execute();

                    CurrentJob = null;
                    workerJob.CurrentWorker = null;
                }

                Thread.Yield();
            }
        }


        public void Stop()
        {
            NeedStop = true;
            shutdownEvent.WaitOne();
        }
    }
}
