using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block0.Threading.Worker
{

    public static class WorkerJobManager
    {
        private static byte IDGenerator = (byte)WorkerJobID.Anonymous;

        internal static bool workerJobInited;

        internal static Dictionary<ushort, WorkerJob> id2ManagedJobDict = new Dictionary<ushort, WorkerJob>();

        internal static Dictionary<ushort, WorkerJob> id2UnManagedJobDict = new Dictionary<ushort, WorkerJob>();

        public static int ManagedJobCount => id2ManagedJobDict.Count;

        public static WorkerJob GetJob(ushort id)
        {
            if(id2ManagedJobDict.TryGetValue(id, out var managedJob))
                return managedJob;
            if (id2UnManagedJobDict.TryGetValue(id, out var unManagedJob))
                return unManagedJob;

            return null;
        }

        public static void AddJob(WorkerJob workerJob)
        {
            if (workerJob is IUniqueJobID workerID)
                workerJob.JobID = workerID.UniqueID;

            checked
            {
                if (workerJob.JobID == (byte)WorkerJobID.None)
                    workerJob.JobID = IDGenerator++;
            }


            if (workerJob is IUnManagedJob)
                id2UnManagedJobDict.Add(workerJob.JobID, workerJob);
            else
                id2ManagedJobDict.Add(workerJob.JobID, workerJob);
        }

        public static bool GetUrgentJobAndCount(out WorkerJob workerJob, out int count)
        {
            count = 0;
            int maxPriority = 0;
            workerJob = null;
            foreach(var job in id2ManagedJobDict.Values)
            {
                if (job.CurrentWorker != null)
                    continue;

                if(job.ExecutePriority > 0)
                    count++;
                if (job.ExecutePriority > maxPriority)
                    workerJob = job;
            }

            return workerJob != null;
        }


    }


}
