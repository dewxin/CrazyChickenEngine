using Block.Assorted.Logging;
using Block0.Threading.Pipe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block0.Threading.Worker
{
    public abstract class MsgWorkerJob:WorkerJob
    {
        public JobMsg CurrentMsg { get; private set; }

        public override int ExecutePriority => ReceivingPipe.Count;

        internal Many4OnePipe<JobMsg> ReceivingPipe { get; private set; } = new Many4OnePipe<JobMsg>();


        protected Stopwatch stopwatch = new Stopwatch();

        //Milliseconds
        public float OneMsgTimeCost { get; private set; }

        //Milliseconds
        public override float EstimatedTimeCost => OneMsgTimeCost * ReceivingPipe.Count;

        public override void Awake()
        {
            ThreadLocal.WorkerJob.Value = this;
        }


        public virtual void Update()
        {

        }

        protected virtual void ExecuteEngine()
        {
        }

        public override void Execute()
        {
            ThreadLocal.WorkerJob.Value = this;
            Update();
            ExecuteEngine();

            //有时候是为了执行Update
            if (ReceivingPipe.Count == 0)
                return;

            stopwatch.Restart();
            int count = 0;
            while (TryGetMsg(out JobMsg item))
            {
                count++;
                ExecuteOneMsg(item);
            }
            var tmp1MsgCost = stopwatch.ElapsedMilliseconds / 8f / count;
            OneMsgTimeCost = OneMsgTimeCost*7/8f + tmp1MsgCost;
        }

        public abstract void ExecuteOneMsg(JobMsg jobMsg);

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

            var msgWorkerJob = WorkerJobManager.GetJob(msg.DestJobId) as MsgWorkerJob;
            if(msgWorkerJob == null)
                Log.Error($"destJob is not {nameof(MsgWorkerJob)}, id={msg.DestJobId}");

            msgWorkerJob.ReceivingPipe.SpinEnqueue(msg);

            if(msgWorkerJob.CurrentWorker==null)
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


}
