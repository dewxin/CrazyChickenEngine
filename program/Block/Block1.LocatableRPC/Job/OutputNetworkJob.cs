﻿using Block.Assorted.Logging;
using Block.RPC.Emitter;
using Block.RPC.Task;
using Block0.Net;
using Block0.Rpc;
using Block0.Threading.Pipe;
using Block0.Threading.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Block1.LocatableRPC.Job
{
    internal class OutputNetworkJob : MsgWorkerJob, IUniqueJobID
    {
        public byte UniqueID => (byte)WorkerJobID.OutputNetwork;

        public override void Start()
        {
            Log.Debug("");
        }

        public override void ExecuteOneMsg(JobMsg item)
        {
            var rpcMsg = item as RemoteRpcJobMsg;
            if (rpcMsg == null)
            {
                Log.Warn($"msg is not Remote: srcJob{item.SourceJobId} destJob{item.DestJobId}");
                //记录一下未能处理的exception
                return;
            }

            rpcMsg.DestAppId = rpcMsg.RealDestAppId;

            if(rpcMsg.ForwardType == RemoteRpcJobMsg.ForwardEnum.Output)
            {
                ForwardOutputMsg(rpcMsg);
            }
        }


        //发送到网络
        private void ForwardOutputMsg(RemoteRpcJobMsg rpcMsg)
        {
            RpcMessage netMessage = new RpcMessage
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
