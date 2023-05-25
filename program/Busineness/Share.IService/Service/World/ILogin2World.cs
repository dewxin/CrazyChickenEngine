using Block.RPC.Task;
using Share.IService.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.IService.Service.World
{
    public interface ILogin2World : IWorldService
    {
        MethodCallTask<TestRpcRecordResult> TestRpcRecord();
    }
}
