using Block.Assorted;
using Server.Common;
using Server.Game.ServiceEurekaNode;
using Server.Game.ServiceLogic;
using System;
using System.Collections.Generic;
using Server.Common.Unit;
using GameServerBase.ServerLogin;
using GameServerBase.ServerWorld;
using Server.Game.ServiceEurekaGlobal;
using Block0.Net;

namespace Server.Game._ServerGetter
{
    public class LogicServerGetter : IServerInfoGetter
    {

        public ServerNode GetServer()
        {
            return new GameServerNode();
        }

        public SocketConfig GetSocketConfig()
        {
            return new SocketConfig()
            {
                IP = "127.0.0.1",
                Port = 24444,
            };
        }


        public List<ServerService> GetServiveList()
        {
            return new List<ServerService>()
            {
                new NodeEurekaService(),
                new LogicService(),
            };
        }
    }
}
