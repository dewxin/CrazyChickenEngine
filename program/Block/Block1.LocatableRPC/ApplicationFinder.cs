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
    //TODO 需要缓存
    public class ApplicationServiceFinder
    {
        public bool IsLocalNode { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public byte DestAppId { get; set; }
        public MethodCallTaskCenter CallTaskCenter {get;set;}

        // Get Service inteface
        // 获取 服务提供的接口 (函数)
        public IService GetService<IService>() where IService : class
        {
            if (DestAppId == 0)
                throw new ArgumentNullException($"{nameof(DestAppId)} cannot be 0");
            //TODO 这里Emitter每次都要创建类开销比较大，需要缓存

            if (IsLocalNode)
            {
                var rpcInvoker = new LocalRpcInvoker(DestAppId, CallTaskCenter);
                var serviceProxy = RpcClientEmitter.Resolve<IService>(rpcInvoker);
                return serviceProxy;
            }
            else
            {
                var rpcInvoker = new NetworkRpcInvoker(DestAppId, CallTaskCenter);
                rpcInvoker.RemoteIPEndPoint = RemoteEndPoint;
                var serviceProxy = RpcClientEmitter.Resolve<IService>(rpcInvoker);
                return serviceProxy;
            }


        }
    }



    public class ApplicationFinder
    {
        private MethodCallTaskCenter methodCallTaskCenter;

        public static void Init()
        {
            WorkerJobManager.AddJob(new OutputNetworkJob());
            WorkerJobManager.AddJob(new InputNetworkJob());
        }

        public ApplicationFinder(MethodCallTaskCenter methodCallTaskCenter)
        {
            this.methodCallTaskCenter = methodCallTaskCenter;
        }


        public ApplicationServiceFinder ByLocal(byte destAppId)
        {
            var procedureCall = new ApplicationServiceFinder
            {
                IsLocalNode = true,
                DestAppId = destAppId,
                CallTaskCenter = methodCallTaskCenter,
            };
            return procedureCall;
        }

        public ApplicationServiceFinder ByEndPoint(IPEndPoint ipEndPoint, byte destAppId)
        {
            var procedureCall = new ApplicationServiceFinder
            {
                RemoteEndPoint = ipEndPoint,
                DestAppId = destAppId,
                CallTaskCenter = methodCallTaskCenter,
            };
            return procedureCall;
        }

    }
}