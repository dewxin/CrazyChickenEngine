using Block1.LocatableRPC;
using Chunk.LocatableRPC;
using GameServerBase;
using Protocol.Param;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.ServiceLogic
{
    public class LogicService : GameService
    {
        private ushort worldServiceNodeId;
        private byte worldServiceId;

        public LogicService()
        {
            ServiceType = Protocol.Param.ServiceTypeEnum.Logic;
        }

        protected override void OnInitAddOn()
        {
            AddHandler(new Client2LogicHandler());
            OnNodeAdded += LogicServiceActor_OnHostAdded;
        }

        private void LogicServiceActor_OnHostAdded(NodeInfo hostInfo)
        {
            foreach(var serviceInfo in hostInfo.ServiceInfoList)
            {
                if (serviceInfo.ServiceType == ServiceTypeEnum.World)
                {
                    worldServiceId = serviceInfo.ServiceID;
                    worldServiceNodeId = hostInfo.NodeId;
                    FindService.ByNodeId(worldServiceNodeId, worldServiceId).
                        GetRpc<ILogic2World>().Hello($"from node{MyNodeId} logic");
                }
            }

        }
    }
}
