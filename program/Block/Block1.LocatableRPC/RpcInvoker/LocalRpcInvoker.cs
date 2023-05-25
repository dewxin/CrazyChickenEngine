using Block.RPC.Emitter;
using Block.RPC.Task;
using Block0.Threading.Pipe;
using Block0.Threading.Worker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Block1.LocatableRPC.RpcInvoker
{
    public class LocalRpcInvoker : IRpcInvoker
    {
        public byte DestServiceId { get; private set; }
        public MethodCallTaskCenter CallTaskCenter { get; private set; }

        public LocalRpcInvoker(byte dstServiceId, MethodCallTaskCenter methodCallTaskCenter)
        {
            Debug.Assert(dstServiceId != 0);
            this.DestServiceId = dstServiceId;
            this.CallTaskCenter= methodCallTaskCenter;
        }

        internal virtual RpcMsg NewMsg()
        {
            RpcMsg localRpcMsg = new RpcMsg();
            localRpcMsg.DestServiceId = DestServiceId;
            //可能是Task Thread访问的

            //TODO 这里做下DEBUG 校验 如果函数调用是需要返回值的并且 没有找不到当前服务ID， 那么抛个异常
            if (ThreadLocal.WorkerJob.IsValueCreated)
                localRpcMsg.SourceServiceId = ThreadLocal.WorkerJob.Value.JobID;
            return localRpcMsg;
        }

        public MethodCallTask<TRet> SendRequestParamObjRetObj<TParam, TRet>(ushort methodId, TParam param)
            where TRet : class
        {
            var methodCallTask = MethodCallTask<TRet>.Start(methodId, CallTaskCenter);

            RpcMsg localRpcMsg = NewMsg();
            {
                localRpcMsg.MethodId = methodId;
                localRpcMsg.MethodCallTaskId = methodCallTask.TaskId;
                localRpcMsg.MethodParam = param;
            }

            WorkerJob.SendMsgToJob(localRpcMsg);

            return methodCallTask;
        }

        public void SendRequestParamObjRetVoid<TParam>(ushort methodId, TParam param)
        {
            RpcMsg threadMsg = NewMsg();
            {
                threadMsg.MethodId = methodId;
                threadMsg.MethodParam = param;
            }

            WorkerJob.SendMsgToJob(threadMsg);
        }

        public MethodCallTask<TRet> SendRequestParamVoidRetObj<TRet>(ushort methodId)
            where TRet : class
        {
            var methodCallTask = MethodCallTask<TRet>.Start(methodId, CallTaskCenter);

            RpcMsg threadMsg = NewMsg();
            {
                threadMsg.MethodId = methodId;
                threadMsg.MethodCallTaskId = methodCallTask.TaskId;
            }

            WorkerJob.SendMsgToJob(threadMsg);

            return methodCallTask;
        }

        public void SendRequestParamVoidRetVoid(ushort methodId)
        {
            RpcMsg threadMsg = NewMsg();
            {
                threadMsg.MethodId = methodId;
            }

            WorkerJob.SendMsgToJob(threadMsg);
        }

        public void SendResponse(object ret, ushort taskId)
        {
            RpcMsg threadMsg = NewMsg();
            {
                threadMsg.MethodCallTaskId = taskId;
                threadMsg.IsMethodCallDoneReply = true;
                threadMsg.MethodParam = ret;
            }

            WorkerJob.SendMsgToJob(threadMsg);
        }
    }

}
