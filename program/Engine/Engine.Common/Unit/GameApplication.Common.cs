using Block1.LocatableRPC;
using Engine.IService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.Unit
{
    public partial class GameApplication
    {
        public AppFinderEx FindApp { get; private set; }
        public ushort MyNodeId { get; set; }
        private Dictionary<ushort, NodeInfo> id2NodeInfoDict = new Dictionary<ushort, NodeInfo>();

        public event Action<NodeInfo> OnNodeAdded = delegate { };

        public void AddNodeInfo(NodeInfo nodeInfo)
        {
            id2NodeInfoDict.Add(nodeInfo.NodeId, nodeInfo);
            OnNodeAdded(nodeInfo);
        }

        public NodeInfo GetNode(ushort nodeId)
        {
            return id2NodeInfoDict[nodeId];
        }

        public NodeInfo GetMyNode()
        {
            return id2NodeInfoDict[MyNodeId];
        }
    }


}
