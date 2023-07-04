using Block.Assorted.Logging;
using Block.RPC.Emitter;
using Block.RPC.Task;
using Block0.Net;
using Block0.Net.Serialize;
using Block0.Threading.Pipe;
using Block0.Threading.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Block1.LocatableRPC
{
    internal class OutputNetworkJob : WorkerJob, IUniqueJobID
    {
        public byte UniqueID => (byte)WorkerJobID.OutputNetwork;

        public override void Start()
        {
            Log.Debug("");
        }

        public override void Execute()
        {
            while (TryGetMsg(out JobMsg item))
            {

                var rpcMsg = item as RemoteRpcMsg;
                if (rpcMsg == null)
                {
                    Log.Warn($"msg is not Remote: srcJob{item.SourceJobId} destJob{item.DestJobId}");
                    //记录一下未能处理的exception
                    continue;
                }

                rpcMsg.DestAppId = rpcMsg.RealDestAppId;

                if(rpcMsg.ForwardType == RemoteRpcMsg.ForwardEnum.Output)
                {
                    ForwardOutputMsg(rpcMsg);
                }

            }
        }


        //发送到网络
        private void ForwardOutputMsg(RemoteRpcMsg rpcMsg)
        {
            NetMessage netMessage = new NetMessage
                (
                sourceAppId: rpcMsg.SourceAppId,
                destAppId: rpcMsg.RealDestAppId,
                methodId: rpcMsg.MethodId,
                methodCallTaskId: rpcMsg.MethodCallTaskId,
                isReply: rpcMsg.IsMethodCallDoneReply
                );
            netMessage.ipEndPoint = rpcMsg.RemoteIPEndPoint;
            netMessage.SetParam(rpcMsg.MethodParam);

            Log.Debug($"DestAppId={netMessage.DestAppId} SourceAppId={netMessage.SourceAppId}");
            var dataBytes = netMessage.GetData();
            UdpSocketManager.Send(dataBytes, netMessage.ipEndPoint);
        }

    }

   
}
