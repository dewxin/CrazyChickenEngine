using Block.RPC.Emitter;
using Block0.Net;
using GameServerBase.ServerWorld;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Protocol.Service.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class TestMsgHeader
    {
        [TestMethod]
        public void TestMethod1()
        {
            var methodId = GetMethodId(typeof(Client2WorldHandler), nameof(IClient2World.Login));
            Assert.IsTrue(methodId > 0);

            var isReply = true;
            ushort peerId = 2;
            ushort invokeId = 2;
            NetMessage msgHeaderBody = 
                new NetMessage(0,0,methodId, methodCallTaskID:invokeId, isReply:isReply);

            Assert.AreEqual(msgHeaderBody.IsReply, isReply);
            Assert.AreEqual(msgHeaderBody.MethodID, methodId);
        }

        private ushort GetMethodId(Type typeWhereMethodIn, string methodName)
        {
            var rpcServiceEntity = RpcServerCodeEmitter.GetRpcServiceEntity(typeWhereMethodIn);

            foreach (var kv in rpcServiceEntity.id2ProcedureInfoDict)
            {
                if (kv.Value.MethodName == methodName)
                    return kv.Key;
            }

            return 0;
        }

    }
}
