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
            StartWork(Environment.ProcessorCount);
        }

        public static void StartWork(int workerCount)
        {

            for(int i = 0; i < workerCount;i++)
            {
                Worker worker = new Worker();
                _workerList.Add(worker);

                ThreadPool.QueueUserWorkItem(worker.Run);
            }

        }
    }
}
