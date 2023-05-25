using Block.Assorted.Logging;
using Block.RPC.Emitter;
using Block.RPC.Task;
using Block0.Net;
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
    internal class NetworkJob : WorkerJob, IUniqueJobID
    {
        public byte UniqueID => (byte)WorkerJobID.Network;

        public override void Init()
        {
            UdpSocketManager.Init(OnReceiveNetMessage);
        }

        public override void Execute()
        {
            while (TryGetMsg(out JobMsg item))
            {
                var rpcMsg = item as RemoteRpcMsg;
                if (rpcMsg == null)
                {
                    //记录一下未能处理的exception
                    continue;
                }

                rpcMsg.DestServiceId = rpcMsg.RealDestServiceId;

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
                sourceServiceId: rpcMsg.SourceServiceId,
                destServiceId: rpcMsg.RealDestServiceId,
                methodID: rpcMsg.MethodId,
                methodCallTaskID: rpcMsg.MethodCallTaskId,
                isReply: rpcMsg.IsMethodCallDoneReply
                );
            netMessage.ipEndPoint = rpcMsg.RemoteIPEndPoint;
            netMessage.SetParam(rpcMsg.MethodParam);

            Log.Debug($"destServiceId={netMessage.DestServiceId} srcServiceId={netMessage.SourceServiceId}");
            var dataBytes = netMessage.GetData();
            UdpSocketManager.Send(dataBytes, netMessage.ipEndPoint);
        }


        //来自网络的数据包，转发到对应的服务
        private void OnReceiveNetMessage(NetMessage netMessage, IPEndPoint ipEndPoint)
        {
            Log.Debug($"destServiceId={netMessage.DestServiceId} srcServiceId={netMessage.SourceServiceId}");
            RemoteRpcMsg remoteRpcMsg = new RemoteRpcMsg()
            {
                RemoteIPEndPoint = ipEndPoint,
                SourceServiceId = netMessage.SourceServiceId,
                DestServiceId = netMessage.DestServiceId,
                MethodId = netMessage.MethodID,
                MethodCallTaskId = netMessage.MethodCallTaskID,
                IsMethodCallDoneReply = netMessage.IsReply,
            };

            //DeserializeParam
            if (remoteRpcMsg.IsMethodCallDoneReply)
            {
                var destServiceJob = WorkerJobManager.GetJob(remoteRpcMsg.DestServiceId) as LPCServiceJob;
                var methodId = destServiceJob.MethodCallTaskCenter.GetTask(remoteRpcMsg.MethodCallTaskId).MethodId;
                Type retType = RpcClientEmitter.GetMethodRetType(methodId);
                remoteRpcMsg.MethodParam = SerializerHelper.Deserialize(retType, netMessage.MethodParamBytes);
            }
            else
            {
                Type paramType = RpcServerCodeEmitter.GetMethodParamType(remoteRpcMsg.MethodId);

                if(paramType!=null && paramType != typeof(void)) 
                    remoteRpcMsg.MethodParam = SerializerHelper.Deserialize(paramType, netMessage.MethodParamBytes);
            }


            WorkerJob.SendMsgToJob(remoteRpcMsg);
        }



    }

   
}
