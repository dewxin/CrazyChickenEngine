using Block.RPC.Emitter;
using Block.RPC.Task;
using Block0.Net;
using Block0.Threading.Worker;
using Block1.LocatableRPC;
using Block1.LocatableRPC.RpcInvoker;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Block1.LocatableRPC
{
    public class ServiceRpcFinder
    {
        public bool IsLocalNode { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public byte DestServiceId { get; set; }
        public MethodCallTaskCenter CallTaskCenter {get;set;}

        // Get Service inteface
        // 获取 服务提供的接口 (函数)
        public IRpc GetRpc<IRpc>() where IRpc : class
        {
            if (DestServiceId == 0)
                throw new ArgumentNullException($"{nameof(DestServiceId)} cannot be 0");
            //TODO 这里Emitter每次都要创建类开销比较大，但先这样

            if (IsLocalNode)
            {
                var rpcInvoker = new LocalRpcInvoker(DestServiceId, CallTaskCenter);
                var serviceProxy = RpcClientEmitter.Resolve<IRpc>(rpcInvoker);
                return serviceProxy;
            }
            else
            {
                var rpcInvoker = new NetworkRpcInvoker(DestServiceId, CallTaskCenter);
                rpcInvoker.RemoteIPEndPoint = RemoteEndPoint;
                var serviceProxy = RpcClientEmitter.Resolve<IRpc>(rpcInvoker);
                return serviceProxy;
            }


        }
    }



    public class ServiceFinder
    {
        private MethodCallTaskCenter methodCallTaskCenter;

        public static void Init()
        {
            WorkerJobManager.AddJob(new NetworkJob());
        }

        public ServiceFinder(MethodCallTaskCenter methodCallTaskCenter)
        {
            this.methodCallTaskCenter = methodCallTaskCenter;
        }


        public ServiceRpcFinder ByLocal(byte destServiceId)
        {
            var procedureCall = new ServiceRpcFinder
            {
                IsLocalNode = true,
                DestServiceId = destServiceId,
                CallTaskCenter = methodCallTaskCenter,
            };
            return procedureCall;
        }

        public ServiceRpcFinder ByEndPoint(IPEndPoint ipEndPoint, byte destServiceId)
        {
            var procedureCall = new ServiceRpcFinder
            {
                RemoteEndPoint = ipEndPoint,
                DestServiceId = destServiceId,
                CallTaskCenter = methodCallTaskCenter,
            };
            return procedureCall;
        }

    }
}