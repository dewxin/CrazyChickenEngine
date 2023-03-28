using Block0.Threading.Pipe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;


namespace Block0.Threading.Worker
{
    public class WorkerThreadHelper
    {
        public static ThreadLocal<Worker> LocalThreadWorker { get; set; } = new ThreadLocal<Worker>();
    }

    public partial class Worker
    {
        public WorkerJob CurrentJob { get; internal set; }
        public static bool WorkerJobInited { get; set; }

        public bool NeedStop { get; protected set; }
        AutoResetEvent shutdownEvent = new AutoResetEvent(false);

        public double ElapsedMs { get; protected set; }

        //0表示 没有初始ID
        public byte CurrentJobID => CurrentJob.JobID;


        public void Run(Object threadContext)
        {
            WorkerThreadHelper.LocalThreadWorker.Value = this;

            //初始化的时候猥琐一下
            lock(typeof(WorkerManager))
            {
                if(!WorkerJobInited)
                {
                    foreach (var workerJob in WorkerJobManager.id2JobDict.Values)
                    {
                        CurrentJob = workerJob;
                        workerJob.Init();
                    }

                    WorkerJobInited= true;
                }
            }


            Stopwatch stopwatch = Stopwatch.StartNew();
            var lastTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            while (!NeedStop)
            {
                var nowTimeMs = stopwatch.Elapsed.TotalMilliseconds; 
                var elapsedMs = nowTimeMs - lastTimeMs;
                lastTimeMs = nowTimeMs;

                ElapsedMs = elapsedMs;

                while(WorkerJobManager.HasJobToHandle(out var workerJob) /*并且剩余时间片的估算值能够处理此次task*/)
                {
                    var prevWorker = Interlocked.CompareExchange(ref workerJob.CurrentWorker, this, null);
                    
                    //不是null说明被其他Worker抢了
                    if(prevWorker != null)  
                        continue;

                    CurrentJob = workerJob;

                    CurrentJob.Execute();

                    CurrentJob = null;
                    workerJob.CurrentWorker = null;
                }

                Thread.Yield();
            }

            shutdownEvent.Set();
        }


        public void Stop()
        {
            NeedStop = true;
            shutdownEvent.WaitOne();
        }
    }
}
