using Block.RPC.Attr;
using Block.RPC.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Engine.IService
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
