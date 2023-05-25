using Engine.Common.Unit;
using Share.IService.Service.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.ServiceLogic
{
    public class Client2LogicHandler : GameServiceHandler, IClient2Logic
    {
        public Client2LogicHandler()
        {
        }

        //public GetPlayerInfoRet GetPlayerInfo()
        //{
        //    return new GetPlayerInfoRet { playerInfo = new PlayerInfo {PlayerId =1, Gold=1, NickName="goodBoy"} };
        //}
    }
}
