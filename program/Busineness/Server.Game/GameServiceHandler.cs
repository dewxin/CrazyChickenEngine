using Block.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class GameServiceHandler : MessageServiceHandler
    {
        public GameService GameService { get; set; }

        public GameServiceHandler()
        {
            //Server = (RavenSingleton.Get<IContainer>().Resolve<ServiceActor>() as GameServer) !;
        }
    }
}
