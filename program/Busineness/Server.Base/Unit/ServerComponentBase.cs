using Server.Common.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Common.Unit
{
    public abstract class ServerComponentBase 
    {
        protected readonly ServerService server;

        public ServerComponentBase(ServerService server)
        {
            this.server = server;
        }

    }
}
