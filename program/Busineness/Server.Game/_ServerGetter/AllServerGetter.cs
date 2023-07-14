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
using Engine.Common.AppCommon;

namespace Server.Game._ServerGetter
{

    public class AllServerGetter : IServerInfoGetter
    {

        public HostNode GetServer()
        {
            var hasGlobalEurekaService = GetApplicationList().Any(service => service is GlobalEurekaApplication);
            return new HostNode()
            {
                Name = "EurekaMaster",
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


        public List<HostApplication> GetApplicationList()
        {
            return new List<HostApplication>()
            {
                new CommonApplication(),
                new GlobalEurekaApplication(),
                new NodeEurekaApplication(),
                new LoginApplication(),
                new LogicApplication(),
                new WorldApplication(),
            };
        }
    }
}
