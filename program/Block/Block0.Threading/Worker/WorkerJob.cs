using Block0.Threading.Pipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block0.Threading.Worker
{
    public enum WorkerJobID : byte
    {
        None = 0,
        Anonymous = 1,

        Named = 50,
        Log,
        OutputNetwork,
        InputNetwork,
        UserNamed = 100,
    }

    public class ThreadLocal
    {
        public static ThreadLocal<WorkerJob> WorkerJob { get; set; } = new ThreadLocal<WorkerJob>();
    }

    public abstract class WorkerJob
    {
        //0表示 没有初始ID
        public byte JobID { get; internal set; }
        
        public JobMsg CurrentMsg { get; private set; }

        public virtual int Priority => ReceivingPipe.Count;

        internal Many4OnePipe<JobMsg> ReceivingPipe { get; private set; } = new Many4OnePipe<JobMsg>();
        public volatile Worker CurrentWorker;

        public virtual void Awake()
        {
            ThreadLocal.WorkerJob.Value = this;
        }

        public virtual void Start()
        {

        }

        public virtual void Execute()
        {
            ThreadLocal.WorkerJob.Value = this;

        }

        public bool TryGetMsg(out JobMsg jobMsg)
        {
            var result = ReceivingPipe.TryDequeue(out jobMsg);
            CurrentMsg = jobMsg;
            return result;
        }


        public static void SendMsgToJob(JobMsg msg)
        {
#if DEBUG
            TryAttachStackInfo2Msg(msg);
#endif

            var job = WorkerJobManager.GetJob(msg.DestJobId);
            job.ReceivingPipe.SpinEnqueue(msg);


            WorkerManager.HintJobCount(1);
        }

        private static void TryAttachStackInfo2Msg(JobMsg msg)
        {
            if (!ThreadLocal.WorkerJob.IsValueCreated)
                return;

            var workerJob = ThreadLocal.WorkerJob.Value;
            var stackInfo = workerJob.CurrentMsg?.StackInfo;

            var newInfo = StackInfo.CreateNew(stackInfo, workerJob.GetType().Name);
            msg.StackInfo = newInfo;
        }


        public static void SendMsgToJob(byte jobId, object methodParam)
        {
            JobMsg jobMsg = new JobMsg();
            jobMsg.DestJobId = jobId;
            jobMsg.MethodParam = methodParam;

            SendMsgToJob(jobMsg);
        }

    }

    public interface IUniqueJobID
    {
        byte UniqueID { get;}
    }


    //未受管理的任务 需要自己执行init execute函数
    public interface IUnManagedJob
    {

    }
}
