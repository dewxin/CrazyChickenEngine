using Block.Assorted.Logging;
using Block0.Threading.Pipe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;


namespace Block0.Threading.Worker
{
    public partial class Worker
    {
        public bool NeedStop { get; protected set; }

        public bool Sentinel { get; set; } 
        public float IdleRate { get; set; } = 0;
        //TODO move to config
        const float SLEEP_IDLE_RATE = 0.5f;
        const float NEW_SAMPLE_WEIGHT = 0.125f;

        public WorkerJob CurrentJob { get; set; }

        public double ElapsedMs { get; protected set; }


        public void Run(Object threadContext)
        {
            LogThreadIdWhenWindows();
            TryInitJobs();

            ExecuteJobs();

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
                    CurrentJob = null;
                }

                foreach (var workerJob in WorkerJobManager.id2ManagedJobDict.Values)
                {
                    CurrentJob = workerJob;
                    workerJob.Start();
                    CurrentJob = null;
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


                byte idleCount = 1;

                while (WorkerJobManager.GetUrgentJobAndCount(out var workerJob, out float count) 
                    /*TODO 并且剩余时间片的估算值能够处理此次task*/)
                {
                    var prevWorker = Interlocked.CompareExchange(ref workerJob.CurrentWorker, this, null);

                    //不是null说明被其他Worker抢了
                    if (prevWorker != null)
                        continue;

                    WorkerManager.HintJobCount(count - 1);


                    CurrentJob = workerJob;
                    idleCount = 0;

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

                IdleRate = IdleRate * (1 - NEW_SAMPLE_WEIGHT) + NEW_SAMPLE_WEIGHT * idleCount;

                if(!Sentinel && IdleRate > SLEEP_IDLE_RATE)
                {
                    WorkerManager.WaitForJob(this);
                }
                else
                {
                    Thread.Yield();
                }

            }
        }


        public void Stop()
        {
            NeedStop = true;
        }
    }
}
