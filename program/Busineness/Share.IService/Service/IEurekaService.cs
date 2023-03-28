using Block.RPC.Attr;
using Block.RPC.Task;
using Protocol.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Service
{
    [RpcServiceGroup(500,1000)]
    public interface IEurekaService
    {

    }

    public interface IEurekaMasterService:IEurekaService
    {
        MethodCallTask<NodePubRet> PubHostInfoAndSub(NodeInfo serverInfo);

        MethodCallTask<List<NodeInfo>> GetNodesContainingService(ServiceTypeWrapper serviceTypeEnum);
    }


    public interface IEurekaClientService:IEurekaService
    {

    }
}
