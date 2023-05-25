using System;
using System.Collections.Generic;
using System.Linq;
using Share.IService.Service.World;
using Share.IService.Param;
using Block.RPC.Task;

namespace GameServerBase.ServerWorld
{
    public class Login2WorldHandler : GameServiceHandler, ILogin2World
    {

        public Login2WorldHandler()
        {
        }

        public MethodCallTask<TestRpcRecordResult> TestRpcRecord()
        {
            return new TestRpcRecordResult { Id= 1 };
        }
    }
}
