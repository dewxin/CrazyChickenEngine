﻿using Block.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.Unit
{
    public class GameServiceHandler : MessageServiceHandler
    {
        public GameApplication Application { get; set; }

        public GameServiceHandler()
        {
            //Server = (RavenSingleton.Get<IContainer>().Resolve<ServiceActor>() as GameServer) !;
        }
    }
}
