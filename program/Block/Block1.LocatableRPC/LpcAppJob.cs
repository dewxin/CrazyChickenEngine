using Block.Rpc;
using Block.RPC.Emitter;
using Block.RPC.Task;
using Block0.Threading.Pipe;
using Block0.Threading.Worker;
using Block1.LocatableRPC;
using Block1.LocatableRPC.RpcInvoker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Block1.LocatableRPC
{
    public class LpcAppJob : WorkerJob
    {
        private ServiceHandlerList serviceHandlerList = new ServiceHandlerList();

		// 现在除了LPCServiceJob 其他地方不能调用MethodCallTask了也就是 ServieFinder
        public MethodCallTaskCenter MethodCallTaskCenter { get; set; } = new MethodCallTaskCenter();

        public LpcAppJob()
        {
        }

        public void AddServiceHandler(params MessageServiceHandler[] messageServiceHandlers)
        {
            serviceHandlerList.AddHandlerRange(messageServiceHandlers);
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Execute()
        {
            base.Execute();

            while (TryGetMsg(out JobMsg item))
            {
                var rpcMsg = item as RpcMsg;
                if (rpcMsg == null)
                {
                    //TODO 记录一下未能处理的异常
                    continue;
                }


                var retVal = HandleMessage(rpcMsg);
                //没有返回值 不需要处理
                if(retVal != null) 
                    SendResponse(rpcMsg, retVal);

                AfterHandleMessage();

            }
        }


        protected virtual void AfterHandleMessage()
        {

        }


        private MethodCallTask HandleMessage(RpcMsg message)
        {
            if (message.IsMethodCallDoneReply)
            {
                MethodCallTaskCenter.MethodCallFinished(message.MethodCallTaskId, message.MethodParam);
                return null;
            }

            var handler = serviceHandlerList.FindHandler(message.MethodId);
            if(handler == null)
            {
                throw new ArgumentException($"Cannot find service handle methodID {message.MethodId}");
            }


            var remoteMsg = message as RemoteRpcMsg;
            handler.RemoteEndPoint = null;
            if (remoteMsg != null)
                handler.RemoteEndPoint = remoteMsg.RemoteIPEndPoint;

            var methodCallTask = handler.HandleMethodCall(message.MethodId, message.MethodParam);

            return methodCallTask; 

        }

        private void SendResponse(RpcMsg message, MethodCallTask methodCallTask)
        {
            var remoteMsg = message as RemoteRpcMsg;
            IRpcInvoker rpcInvoker;
            if(remoteMsg != null)
            {
                rpcInvoker = new NetworkRpcInvoker(remoteMsg.SourceAppId, this.MethodCallTaskCenter)
                {
                    RemoteIPEndPoint = remoteMsg.RemoteIPEndPoint,
                };
            }
            else
            {
                rpcInvoker = new LocalRpcInvoker(message.SourceAppId, this.MethodCallTaskCenter);
            }


            if (methodCallTask.WaitCount > 0)
            {
                var sendResponseTask = 
                    BaseTask.New(() => rpcInvoker.SendResponse(methodCallTask.Result, message.MethodCallTaskId));
                methodCallTask.ContinueWith(sendResponseTask);
            }
            else
            {
                rpcInvoker.SendResponse(methodCallTask.Result, message.MethodCallTaskId);
            }
        }
    }
}
