using Block.Rpc;
using Block.RPC.Emitter;
using Block.RPC.Task;
using Block0.Threading.Pipe;
using Block0.Threading.Worker;
using Block1.LocatableRPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chunk.LocatableRPC
{
    public class LPCServiceJob : WorkerJob
    {
        private ServiceHandlerList serviceHandlerList = new ServiceHandlerList();

		// 现在除了LPCServiceJob 其他地方不能调用MethodCallTask了也就是 ServieFinder
        public MethodCallTaskCenter MethodCallTaskCenter { get; set; } = new MethodCallTaskCenter();

        public LPCServiceJob()
        {
        }

        public void AddHandler(params MessageServiceHandler[] messageServiceHandlers)
        {
            serviceHandlerList.AddHandlerRange(messageServiceHandlers);
        }

        public override void Init()
        {
            MethodCallTaskCenter.ThreadLocal.Value = MethodCallTaskCenter;
        }

        public override void Execute()
        {
            MethodCallTaskCenter.ThreadLocal.Value = MethodCallTaskCenter;

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
                MethodCallTaskCenter.ThreadLocal.Value.MethodCallFinished(message.MethodCallTaskId, message.MethodParam);
                return null;
            }

            var handler = serviceHandlerList.FindHandler(message.MethodId);
            if(handler == null)
            {
                throw new ArgumentException($"Cannot find service handle methodID {message.MethodId}");
            }


            var methodCallTask = handler.HandleMethodCall(message.MethodId, message.MethodParam);
            return methodCallTask; 

        }

        private void SendResponse(RpcMsg message, MethodCallTask methodCallTask)
        {
            var remoteMsg = message as RemoteRpcMsg;
            IRpcInvoker rpcInvoker;
            if(remoteMsg != null)
            {
                rpcInvoker = new NetworkRpcInvoker()
                {
                    RemoteIPEndPoint = remoteMsg.RemoteIPEndPoint,
                    DestServiceId = remoteMsg.SourceServiceId
                };
            }
            else
            {
                rpcInvoker = new LocalRpcInvoker()
                {
                    DestServiceId = message.SourceServiceId,
                };
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
