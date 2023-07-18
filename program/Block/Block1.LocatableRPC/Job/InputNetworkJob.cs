using Block.Assorted.Logging;
using Block.RPC.Emitter;
using Block0.Net;
using Block0.Rpc;
using Block0.Rpc.Serialize;
using Block0.Threading.Pipe;
using Block0.Threading.Worker;
using System;
using System.Net;

namespace Block1.LocatableRPC.Job
{
    internal class InputNetworkJob : WorkerJob, IUniqueJobID
    {
        public byte UniqueID => (byte)WorkerJobID.InputNetwork;
        public override int ExecutePriority => UdpSocketManager.AvailableData;

        public override float EstimatedTimeCost => WorkerJob.ExpectedTimeCostPerWorker;

        public override void Start()
        {
            Log.Debug("");
            UdpSocketManager.Init();
        }

        public override void Execute()
        {
            while (UdpSocketManager.TryGetMessage(out var byteSeg, out var iPEndPoint))
            {
                var netMessage = RpcMessage.Parse(byteSeg);
                OnReceiveNetMessage(netMessage, iPEndPoint);
            }
        }



        //来自网络的数据包，转发到对应的服务
        private void OnReceiveNetMessage(RpcMessage netMessage, IPEndPoint ipEndPoint)
        {
            Log.Debug($"DestAppId={netMessage.DestAppId} SourceAppId={netMessage.SourceAppId} MethodId={netMessage.MethodID} TaskId={netMessage.MethodCallTaskID} IsReply={netMessage.IsReply}");
            RemoteRpcJobMsg remoteRpcMsg = new RemoteRpcJobMsg()
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

                if (paramType != null && paramType != typeof(void))
                    remoteRpcMsg.MethodParam = SerializerStub.Deserialize(paramType, netMessage.MethodParamBytes);
            }


            MsgWorkerJob.SendMsgToJob(remoteRpcMsg);
        }


    }


}
