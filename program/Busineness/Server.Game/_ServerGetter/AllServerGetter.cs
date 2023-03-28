using Block.Assorted;
using Server.Common;
using System.Collections.Generic;
using Server.Common.Unit;
using GameServerBase.ServerLogin;
using GameServerBase.ServerWorld;
using Server.Game.ServiceEurekaGlobal;
using Server.Game.ServiceLogic;
using Block0.Net;
using Server.Game.ServiceEurekaNode;
using System.Linq;

namespace Server.Game._ServerGetter
{

    public class AllServerGetter : IServerInfoGetter
    {

        public ServerNode GetServer()
        {
            var hasGlobalEurekaService = GetServiveList().Any(service => service is GlobalEurekaService);
            return new GameServerNode()
            {
                IsGlobalEureka = hasGlobalEurekaService,
            };
        }

        public SocketConfig GetSocketConfig()
        {
            return new SocketConfig()
            {
                IP = "127.0.0.1",
                Port = 23333
            };
        }


        public List<ServerService> GetServiveList()
        {
            return new List<ServerService>()
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
