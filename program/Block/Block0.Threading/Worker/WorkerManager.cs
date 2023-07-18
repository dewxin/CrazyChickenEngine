using Block.Assorted;
using Block.Assorted.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block0.Threading.Worker
{
    public static class WorkerManager
    {
        private static List<Worker> workerList = new List<Worker>();

        private static int waitWorkerCount = 0;
        private static AutoResetEvent WorkerHireEvent = new AutoResetEvent(false);

        public static int WaitWorkerCount => waitWorkerCount;
        public static int WorkerCount => workerList.Count;
        public static void StartWork()
        {
            StartWork(WorkerCountHeuristic());
        }

        private static int WorkerCountHeuristic()
        {
            //至少要留个线程给垃圾回收
            int maxCount = Environment.ProcessorCount - 1;
            //考虑当前任务的个数
            maxCount = Math.Min(WorkerJobManager.ManagedJobCount, maxCount);

            Log.Info($"ProcessorCount {Environment.ProcessorCount}, ManagedJobCount {WorkerJobManager.ManagedJobCount}");

            return maxCount;
        }

        public static void StartWork(int workerCount)
        {
            Log.Info($"worker count is {workerCount}");

            workerList.Add(new Worker {Sentinel = true });
            for(int i = 0; i < workerCount-1;i++)
            {
                Worker worker = new Worker();
                workerList.Add(worker);
            }


            foreach(var worker in workerList)
            {
                ThreadPool.QueueUserWorkItem(worker.Run);
            }

        }

        public static void HintJobCount(float count)
        {
            if(count > 0)
            {
                //TODO这边启动会有延迟，
                //从set到有worker去工作存在一定的interval
                //interval期间导致会多次set，导致超额的worker过来
                WorkerHireEvent.Set();
            }
        }

        public static void WaitForJob(Worker worker)
        {
            if (worker.Sentinel)
                return;

            //TODO
            //除了这里wait中的worker，还有一些worker的线程会因为Thread.Yield放弃CPU资源。
            //导致 并不是所有的job都有一个worker在为其服务。
            Interlocked.Increment(ref waitWorkerCount);
            WorkerHireEvent.WaitOne();
            worker.IdleRate = 0;
            Interlocked.Decrement(ref waitWorkerCount);
        }

        public static void EndWork()
        {
            throw new NotImplementedException();
        }
    }
}
