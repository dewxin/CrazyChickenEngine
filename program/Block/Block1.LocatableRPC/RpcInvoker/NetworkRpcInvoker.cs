using Block.Assorted.Logging;
using Block.RPC.Task;
using Block0.Threading.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Block1.LocatableRPC.RpcInvoker
{
    public class NetworkRpcInvoker : LocalRpcInvoker
    {
        public NetworkRpcInvoker(byte dstServiceId, MethodCallTaskCenter methodCallTaskCenter) : base(dstServiceId, methodCallTaskCenter)
        {
            Log.Debug("");
        }

        public IPEndPoint RemoteIPEndPoint { get; set; }

        internal override RpcMsg NewMsg()
        {
            Log.Debug("");
            RemoteRpcMsg rpcMsg = new RemoteRpcMsg();
            rpcMsg.DestAppId = (byte)WorkerJobID.OutputNetwork;
            rpcMsg.ForwardType = RemoteRpcMsg.ForwardEnum.Output;
            rpcMsg.RealDestAppId = DestAppId;
            rpcMsg.RemoteIPEndPoint = RemoteIPEndPoint;
            //可能是Task Thread访问的

            //TODO 这里做下DEBUG 校验 如果函数调用是需要返回值的并且 没有找不到当前服务ID， 那么抛个异常
            if (ThreadLocal.WorkerJob.IsValueCreated)
                rpcMsg.SourceAppId = ThreadLocal.WorkerJob.Value.JobID;
            else
                Log.Warn("cannot find source jobId");
            return rpcMsg;
        }
    }
}
