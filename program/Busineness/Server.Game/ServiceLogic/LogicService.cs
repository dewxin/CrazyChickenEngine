using Block1.LocatableRPC;
using Engine.Common.Unit;
using Engine.IService;
using GameServerBase;
using Share.IService.Service.World;
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
            ServiceType = ServiceTypeEnum.Logic;
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
