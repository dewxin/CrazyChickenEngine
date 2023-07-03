using Block.Assorted.Logging;
using Block0.Threading.Pipe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
            LogThreadIdWhenWindows();
            TryInitJobs();

            ExecuteJobs();

            shutdownEvent.Set();
        }


        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        private void LogThreadIdWhenWindows()
        {
            try
            {
                Log.Info($"worker run, threadId {GetCurrentThreadId()}");
            }
            catch(DllNotFoundException ex)
            {
                //do nothing
            }
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

                    workerJob.Awake();
                }

                foreach (var workerJob in WorkerJobManager.id2ManagedJobDict.Values)
                {
                    CurrentJob = workerJob;

                    workerJob.Start();
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

#if DEBUG
                    CurrentJob.Execute();
#else
                    try
                    {
                        CurrentJob.Execute();
                    }
                    catch(Exception ex)
                    {
                        Log.Error(ex.ToString());
                    }
#endif

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
