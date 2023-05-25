using Block.Assorted;
using System.Collections.Generic;
using Server.Game.ServiceLogic;
using Block0.Net;
using System.Linq;
using Share.Common.Unit;
using System.Net;
using Engine.Common.ServiceEurekaNode;
using GameServerBase.ServerLogin;
using GameServerBase.ServerWorld;
using Engine.Common.Unit;
using Engine.IService;
using Engine.Common.ServiceEurekaGlobal;

namespace Server.Game._ServerGetter
{

    public class AllServerGetter : IServerInfoGetter
    {

        public HostNode GetServer()
        {
            var hasGlobalEurekaService = GetServiveList().Any(service => service is GlobalEurekaService);
            return new HostNode()
            {
                IsGlobalEureka = hasGlobalEurekaService,
            };
        }

        public SocketConfig GetSocketConfig()
        {
            return new SocketConfig()
            {
                IP = ShareJson.Inst.EurekaMasterNodeIp,
                Port = ShareJson.Inst.EurekaMasterNodePort
            };
        }


        public List<HostService> GetServiveList()
        {
            return new List<HostService>()
            {
                new GlobalEurekaService(),
                new NodeEurekaService(),
                new LoginService(),
                new LogicService(),
                new WorldService(),
            };
        }
    }
}
