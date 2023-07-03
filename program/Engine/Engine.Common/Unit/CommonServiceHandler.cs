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
            Log.Debug($"{Application.GetType().Name}");
            Application.AddNodeInfo(hostInfo);
        }

        public void NotifyNodePubRet(NodePubRet nodePubRet)
        {
            Log.Debug($"{Application.GetType().Name}");
            Application.MyNodeId = nodePubRet.NodeId;
            foreach (var nodeInfo in nodePubRet.NodeInfoList)
            {
                Application.AddNodeInfo(nodeInfo);
            }
        }
    }
}
