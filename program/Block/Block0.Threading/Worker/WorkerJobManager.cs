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
        private static byte IDGenerator = (byte)WorkerJobID.SelfDefineMax;

        internal static Dictionary<ushort, WorkerJob> id2JobDict = new Dictionary<ushort, WorkerJob>();

        public static WorkerJob GetJob(ushort id)
        {
            return id2JobDict[id];
        }

        public static void AddJob(WorkerJob workerJob)
        {
            if (workerJob is IUniqueTaskID workerID)
            {
                workerJob.JobID = workerID.UniqueID;
            }

            checked
            {
                if (workerJob.JobID == (byte)WorkerJobID.None)
                    workerJob.JobID = IDGenerator++;
            }

            //Worker worker = new Worker();
            //{
            //    worker.WorkerTask = workerTask;
            //    workerTask.CurrentWorker = worker;
            //}


            id2JobDict.Add(workerJob.JobID, workerJob);
            //return worker;
        }


        public static bool HasJobToHandle(out WorkerJob workerJob)
        {
            foreach(var job in id2JobDict.Values)
            {
                if(job.NeedsHandleMsg) 
                {
                    workerJob = job;
                    return true; 
                }
            }

            workerJob = null;
            return false;
        }

    }


}
