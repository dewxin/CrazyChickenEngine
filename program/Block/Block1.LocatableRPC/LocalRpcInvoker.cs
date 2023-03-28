using Block.RPC.Emitter;
using Block.RPC.Task;
using Block0.Threading.Pipe;
using Block0.Threading.Worker;
using Chunk.LocatableRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Block1.LocatableRPC
{
    public class LocalRpcInvoker : IRpcInvoker
    {
        public byte DestServiceId { get; set; }

        public LocalRpcInvoker()
        {
        }

        internal virtual RpcMsg NewMsg()
        {
            RpcMsg localRpcMsg = new RpcMsg();
            localRpcMsg.DestServiceId = DestServiceId;
            //可能是Task Thread访问的

            //TODO 这里做下DEBUG 校验 如果函数调用是需要返回值的并且 没有找不到当前服务ID， 那么抛个异常
            if (WorkerThreadHelper.LocalThreadWorker.IsValueCreated)
                localRpcMsg.SourceServiceId = WorkerThreadHelper.LocalThreadWorker.Value.CurrentJobID;
            return localRpcMsg;
        }

        public MethodCallTask<TRet> SendRequestParamObjRetObj<TParam, TRet>(ushort methodId, TParam param)
            where TRet : class
        {
            var methodCallTask = MethodCallTask<TRet>.Start(methodId);

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
            var methodCallTask = MethodCallTask<TRet>.Start(methodId);

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


    public class NetworkRpcInvoker: LocalRpcInvoker
    {
        public IPEndPoint RemoteIPEndPoint { get; set; }

        internal override RpcMsg NewMsg()
        {
            RemoteRpcMsg rpcMsg = new RemoteRpcMsg();
            rpcMsg.DestServiceId = (byte)WorkerJobID.Network;
            rpcMsg.ForwardType = RemoteRpcMsg.ForwardEnum.Output;
            rpcMsg.RealDestServiceId = DestServiceId;
            rpcMsg.RemoteIPEndPoint = RemoteIPEndPoint;
            //可能是Task Thread访问的

            //TODO 这里做下DEBUG 校验 如果函数调用是需要返回值的并且 没有找不到当前服务ID， 那么抛个异常
            if (WorkerThreadHelper.LocalThreadWorker.IsValueCreated)
                rpcMsg.SourceServiceId = WorkerThreadHelper.LocalThreadWorker.Value.CurrentJobID;
            return rpcMsg;
        }
    }
}
