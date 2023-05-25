using Server.Game;
using Server.Game.ServiceWorld.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServerBase.ServerWorld
{
    public class WorldService : GameService
    {
        internal PlayerInfoManager PlayerInfoManager { get; set; } = new PlayerInfoManager();
        internal MatchManager MatchManager { get; set; } = new MatchManager();

        public WorldService()
        {
            ServiceType = ServiceTypeEnum.World;

            OnAfterMessage += MatchManager.CheckTaskState;
        }

        protected override void OnInitAddOn()
        {
            AddHandler(new Client2WorldHandler(), new Logic2WorldHandler(), new Login2WorldHandler());
        }

    }
}
