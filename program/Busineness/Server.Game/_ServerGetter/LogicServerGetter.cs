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
                Port = 24444,
            };
        }


        public List<HostApplication> GetApplicationList()
        {
            return new List<HostApplication>()
            {
                new NodeEurekaApplication(),
                new LogicApplication(),
            };
        }
    }
}
