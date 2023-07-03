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
    public class WorldApplication : GameApplication
    {
        internal PlayerInfoManager PlayerInfoManager { get; set; } = new PlayerInfoManager();
        internal MatchManager MatchManager { get; set; } = new MatchManager();

        public WorldApplication()
        {
            ApplicationType = ApplicationTypeEnum.World;

            OnAfterMessage += MatchManager.CheckTaskState;
        }

        protected override void OnInitAddOn()
        {
            AddServiceHandler(new Client2WorldHandler(), new Logic2WorldHandler(), new Login2WorldHandler());
        }

    }
}
