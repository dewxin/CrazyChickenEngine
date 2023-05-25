using Block.Assorted.Logging;
using Block.Rpc;
using Engine.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.Unit
{
    public class CommonServiceHandler : GameServiceHandler, ICommonService
    {

        public CommonServiceHandler()
        {
        }

        public void NotifyNodeInfo(NodeInfo hostInfo)
        {
            Log.Debug($"{GameService.GetType().Name}");
            GameService.AddNodeInfo(hostInfo);
        }

        public void NotifyNodePubRet(NodePubRet nodePubRet)
        {
            Log.Debug($"{GameService.GetType().Name}");
            GameService.MyNodeId = nodePubRet.NodeId;
            foreach (var nodeInfo in nodePubRet.NodeInfoList)
            {
                GameService.AddNodeInfo(nodeInfo);
            }
        }
    }
}
