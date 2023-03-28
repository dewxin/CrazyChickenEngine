using Block.RPC.Attr;
using Protocol.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: AssemblyMetadataAttribute("NagiAsmType", "IService")]
namespace Protocol.Service
{
    //为了避免隐藏bug, 不要使用[0-100]的msgId
    [RpcService(100,500)]
    public interface ICommonService
    {

        void NotifyNodePubRet(NodePubRet nodePubRet);
        void NotifyNodeInfo(NodeInfo nodeInfo);

    }
}
