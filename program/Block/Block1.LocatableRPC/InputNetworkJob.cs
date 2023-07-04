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
    internal class InputNetworkJob : WorkerJob, IUniqueJobID
    {
        public byte UniqueID => (byte)WorkerJobID.InputNetwork;
        public override int Priority => UdpSocketManager.AvailableData;

        public override void Start()
        {
            Log.Debug("");
            UdpSocketManager.Init();
        }

        public override void Execute()
        {
            while (UdpSocketManager.TryGetMessage(out var netMessage, out var iPEndPoint))
            {
                OnReceiveNetMessage(netMessage, iPEndPoint);
            }
        }



        //来自网络的数据包，转发到对应的服务
        private void OnReceiveNetMessage(NetMessage netMessage, IPEndPoint ipEndPoint)
        {
            Log.Debug($"DestAppId={netMessage.DestAppId} SourceAppId={netMessage.SourceAppId} MethodId={netMessage.MethodID} TaskId={netMessage.MethodCallTaskID} IsReply={netMessage.IsReply}");
            RemoteRpcMsg remoteRpcMsg = new RemoteRpcMsg()
            {
                RemoteIPEndPoint = ipEndPoint,
                SourceAppId = netMessage.SourceAppId,
                DestAppId = netMessage.DestAppId,
                MethodId = netMessage.MethodID,
                MethodCallTaskId = netMessage.MethodCallTaskID,
                IsMethodCallDoneReply = netMessage.IsReply,
            };


            //DeserializeParam
            if (remoteRpcMsg.IsMethodCallDoneReply)
            {
                Log.Debug("enter reply");
                var destAppJob = WorkerJobManager.GetJob(remoteRpcMsg.DestAppId) as LpcAppJob;
                var methodId = destAppJob.MethodCallTaskCenter.GetTask(remoteRpcMsg.MethodCallTaskId).MethodId;
                Type retType = RpcClientEmitter.GetMethodRetType(methodId);
                remoteRpcMsg.MethodParam = SerializerStub.Deserialize(retType, netMessage.MethodParamBytes);
            }
            else
            {
                Log.Debug("enter call");
                Type paramType = RpcServerCodeEmitter.GetMethodParamType(remoteRpcMsg.MethodId);

                if(paramType!=null && paramType != typeof(void)) 
                    remoteRpcMsg.MethodParam = SerializerStub.Deserialize(paramType, netMessage.MethodParamBytes);
            }


            WorkerJob.SendMsgToJob(remoteRpcMsg);
        }



    }

   
}
