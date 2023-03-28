using Chunk.LocatableRPC;
using Protocol.Param;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public partial class GameService
    {
        public ServiceFinderWrapper FindService { get; private set; }
        public ushort MyNodeId { get; set; }
        private Dictionary<ushort, NodeInfo> id2NodeInfoDict = new();

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

    public class ServiceFinderWrapper
    {
        public ServiceFinderWrapper(GameService gameService) { service = gameService; }
        private GameService service;

        public ServiceRpcFinder ByLocal(byte destServiceId) => ServiceFinder.ByLocal(destServiceId);

        public ServiceRpcFinder ByEndPoint(IPEndPoint ipEndPoint, byte destServiceId)
            => ServiceFinder.ByEndPoint(ipEndPoint, destServiceId);

        public ServiceRpcFinder ByNodeId(ushort destNodeId, byte destServiceId)
        {
            if (destNodeId == service.MyNodeId)
                return ByLocal(destServiceId);


            var node = service.GetNode(destNodeId);
            var endpoint = new IPEndPoint(IPAddress.Parse(node.ServerIP), node.ServerPort);
            var procedureCall = new ServiceRpcFinder
            {
                RemoteEndPoint = endpoint,
                DestServiceId = destServiceId
            };
            return procedureCall;
        }

        public ServiceRpcFinder ByLocal(ServiceTypeEnum serviceTypeEnum)
        {
            var servieInfo = service.GetMyNode().GetServiceByType(serviceTypeEnum).Single();

            return ByLocal(servieInfo.ServiceID);

        }
    }
}
