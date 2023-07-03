using Block1.LocatableRPC;
using Engine.Common.Unit;
using Engine.IService;
using GameServerBase;
using Share.IService.Service.World;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.ServiceLogic
{
    public class LogicApplication : GameApplication
    {
        private ushort worldServiceNodeId;
        private byte worldServiceId;

        public LogicApplication()
        {
            ApplicationType = ApplicationTypeEnum.Logic;
        }

        protected override void OnInitAddOn()
        {
            AddServiceHandler(new Client2LogicHandler());
            OnNodeAdded += LogicServiceActor_OnHostAdded;
        }

        private void LogicServiceActor_OnHostAdded(NodeInfo hostInfo)
        {
            foreach(var serviceInfo in hostInfo.ApplicationInfoList)
            {
                if (serviceInfo.ApplicationType == ApplicationTypeEnum.World)
                {
                    worldServiceId = serviceInfo.AppID;
                    worldServiceNodeId = hostInfo.NodeId;
                    FindApp.ByNodeId(worldServiceNodeId, worldServiceId).
                        GetService<ILogic2World>().Hello($"from node{MyNodeId} logic");
                }
            }

        }
    }
}
