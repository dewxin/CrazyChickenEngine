using Block.RPC.Task;
using Protocol.Param;
using Protocol.Service.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Service.World
{
    public interface ILogin2World: IWorldService
    {
        MethodCallTask<TestRpcRecordResult> TestRpcRecord();
    }
}
