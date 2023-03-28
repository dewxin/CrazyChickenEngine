using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Service;
using Protocol.Service.World;
using Block.Assorted.Logging;
using Protocol.Param;

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
