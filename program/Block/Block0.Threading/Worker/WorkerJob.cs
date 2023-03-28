using Block0.Threading.Pipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block0.Threading.Worker
{
    public enum WorkerJobID : byte
    {
        None = 0,
        Log,
        Network,

        SelfDefineMin,

        SelfDefineMax = 128,
    }

    public abstract class WorkerJob
    {
        //0表示 没有初始ID
        public byte JobID { get; internal set; }

        internal bool NeedsHandleMsg => ReceivingPipe.Count > 0 && CurrentWorker == null;
        internal Many4OnePipe<JobMsg> ReceivingPipe { get; private set; } = new Many4OnePipe<JobMsg>();
        public volatile Worker CurrentWorker;
        public abstract void Init();
        public abstract void Execute();

        public bool TryGetMsg(out JobMsg pipeItem)
        {
            return ReceivingPipe.TryDequeue(out pipeItem);
        }


        public static void SendMsgToJob(JobMsg msg)
        {
            var job = WorkerJobManager.GetJob(msg.DestJobId);
            job.ReceivingPipe.SpinEnqueue(msg);
        }

        public static void SendMsgToJob(byte jobId, object methodParam)
        {
            JobMsg jobMsg = new JobMsg();
            jobMsg.DestJobId = jobId;
            jobMsg.MethodParam = methodParam;

            SendMsgToJob(jobMsg);
        }

    }

    public interface IUniqueTaskID
    {
        byte UniqueID { get;}
    }
}
