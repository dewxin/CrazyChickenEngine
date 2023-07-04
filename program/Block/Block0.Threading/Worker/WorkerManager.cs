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

        private static int WaitWorkerCount = 0;
        private static AutoResetEvent WorkerHireEvent = new AutoResetEvent(false);

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

        public static void HintJobCount(int count)
        {
            if(count > 0)
            {
                WorkerHireEvent.Set();
            }
        }

        public static void WaitForJob(Worker worker)
        {
            if (worker.Sentinel)
                return;

            Interlocked.Increment(ref WaitWorkerCount);
            //如果打开这里的注释，应该能看到 WaitWorkerCount数量在波动
            //Log.Info($"+1 ={WaitWorkerCount}"); 
            WorkerHireEvent.WaitOne();
            worker.IdleRate = 0;
            Interlocked.Decrement(ref WaitWorkerCount);
            //Log.Info($"-1 ={WaitWorkerCount}");
        }

        public static void EndWork()
        {
            throw new NotImplementedException();
        }
    }
}
