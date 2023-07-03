using Block.Assorted.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block0.Threading.Worker
{
    public static class WorkerManager
    {
        private static List<Worker> _workerList = new List<Worker>();

        public static void StartWork()
        {
            //TODO 应该考虑有几个任务
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
            for(int i = 0; i < workerCount;i++)
            {
                Worker worker = new Worker();
                _workerList.Add(worker);

                ThreadPool.QueueUserWorkItem(worker.Run);
            }

        }

        public static void EndWork()
        {
            throw new NotImplementedException();
        }
    }
}
