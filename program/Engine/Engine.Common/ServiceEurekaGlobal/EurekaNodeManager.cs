using Block.Assorted;
using Engine.IService;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.ServiceEurekaGlobal
{
    public class EurekaNodeManager
    {
        //Id 如果从0开始容易引入bug ，这里从1开始
        private IDGenerator IDGenerator = new IDGenerator(1);
        private Dictionary<ushort, NodeInfo> nodeId2InfoDict = new Dictionary<ushort, NodeInfo>();


        public ushort RegisterNodeRetID(NodeInfo nodeInfo)
        {
            nodeInfo.NodeId = IDGenerator.GetUShortID();
            nodeId2InfoDict.Add(nodeInfo.NodeId, nodeInfo);
            return nodeInfo.NodeId;
        }

        public List<NodeInfo> GetHostList()
        {
            return nodeId2InfoDict.Values.ToList();
        }

        public List<NodeInfo> GetNodesByServiceType(ApplicationTypeEnum serviceTypeEnum)
        {
            var retList = new List<NodeInfo>();
            foreach (var nodeInfo in nodeId2InfoDict.Values)
            {
                foreach (var service in nodeInfo.ApplicationInfoList)
                {
                    if (service.ApplicationType.Equals(serviceTypeEnum))
                    {
                        retList.Add(nodeInfo);
                        break;
                    }
                }
            }

            return retList;
        }
    }
}
