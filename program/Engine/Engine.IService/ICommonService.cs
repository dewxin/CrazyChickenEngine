using Block.RPC.Attr;
using Block.RPC.Emitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Engine.IService
{
    //为了避免隐藏bug, 不要使用[0-100]的msgId
    [RpcService(100, 500)]
    public interface ICommonService
    {

        void NotifyNodePubRet(NodePubRet nodePubRet);
        void NotifyNodeInfo(NodeInfo nodeInfo);

    }
}
