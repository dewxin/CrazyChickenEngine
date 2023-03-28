using Block.Assorted;
using Protocol.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.ServiceEurekaGlobal
{
    public class EurekaNodeManager
    {
        //Id 如果从0开始容易引入bug ，这里从1开始
        private IDGenerator IDGenerator = new IDGenerator(1); // 自己需要占用
        private Dictionary<ushort, NodeInfo> nodeId2InfoDict = new ();


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

        public List<NodeInfo> GetNodesByServiceType(ServiceTypeEnum serviceTypeEnum)
        {
            var retList = new List<NodeInfo>();
            foreach(var nodeInfo in nodeId2InfoDict.Values)
            {
                foreach(var service in nodeInfo.ServiceInfoList)
                {
                    if(service.ServiceType.Equals(serviceTypeEnum))
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
