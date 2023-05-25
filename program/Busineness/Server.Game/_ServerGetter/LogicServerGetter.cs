using Block.Assorted;
using Server.Game.ServiceLogic;
using System;
using System.Collections.Generic;
using GameServerBase.ServerLogin;
using GameServerBase.ServerWorld;
using Block0.Net;
using Share.Common.Unit;
using Engine.Common.Unit;
using Engine.Common.ServiceEurekaNode;
using Engine.Common.Unit;
using Engine.IService;
namespace Server.Game._ServerGetter
{
    public class LogicServerGetter : IServerInfoGetter
    {

        public HostNode GetServer()
        {
            return new HostNode();
        }

        public SocketConfig GetSocketConfig()
        {
            return new SocketConfig()
            {
                IP = "127.0.0.1",
                Port = 24444,
            };
        }


        public List<HostService> GetServiveList()
        {
            return new List<HostService>()
            {
                new NodeEurekaService(),
                new LogicService(),
            };
        }
    }
}
