using Block.Assorted.Logging;
using Protocol;
using Protocol.Param;
using Protocol.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.ServiceEurekaNode
{
    public class NodeEurekaHandler : GameServiceHandler, IEurekaClientService
    {

        //public void NotifyOnlineServerAdded(ServiceInfo newServer)
        //{
        //    Log.Debug($"{JsonConvert.SerializeObject(newServer)}");
        //    Server.OnServerInfoAddedNotice(newServer);
        //}

        //public void NotifyOnlineServerRemoved(ServiceInfo newServer)
        //{
        //    Log.Debug($"{JsonConvert.SerializeObject(newServer)}");
        //    Server.OnServerInfoRemovedNotice(newServer);
        //}
    }
}
