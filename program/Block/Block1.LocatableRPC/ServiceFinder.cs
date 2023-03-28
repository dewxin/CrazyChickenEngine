using Block.RPC.Emitter;
using Block0.Net;
using Block0.Threading.Worker;
using Block1.LocatableRPC;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Chunk.LocatableRPC
{
    public class ServiceRpcFinder
    {
        public bool IsLocalNode { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public byte DestServiceId { get; set; }


        // Get Service inteface
        // 获取 服务提供的接口 (函数)
        public IRpc GetRpc<IRpc>() where IRpc : class
        {
            if (DestServiceId == 0)
                throw new ArgumentNullException($"{nameof(DestServiceId)} cannot be 0");
            //TODO 这里Emitter每次都要创建类开销比较大，但先这样

            if (IsLocalNode)
            {
                var rpcInvoker = new LocalRpcInvoker();
                rpcInvoker.DestServiceId = DestServiceId;
                var serviceProxy = RpcClientEmitter.Resolve<IRpc>(rpcInvoker);
                return serviceProxy;
            }
            else
            {
                var rpcInvoker = new NetworkRpcInvoker();
                rpcInvoker.RemoteIPEndPoint = RemoteEndPoint;
                rpcInvoker.DestServiceId = DestServiceId;
                var serviceProxy = RpcClientEmitter.Resolve<IRpc>(rpcInvoker);
                return serviceProxy;
            }


        }
    }



    //locatable procedure call
    public class ServiceFinder
    {
        public static void Init()
        {
            WorkerJobManager.AddJob(new NetworkJob());
        }


        public static ServiceRpcFinder ByLocal(byte destServiceId)
        {
            var procedureCall = new ServiceRpcFinder
            {
                IsLocalNode = true,
                DestServiceId = destServiceId
            };
            return procedureCall;
        }

        public static ServiceRpcFinder ByEndPoint(IPEndPoint ipEndPoint, byte destServiceId)
        {
            var procedureCall = new ServiceRpcFinder
            {
                RemoteEndPoint = ipEndPoint,
                DestServiceId = destServiceId
            };
            return procedureCall;
        }



    }
}