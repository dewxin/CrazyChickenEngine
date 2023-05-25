using Block.Assorted.Logging;
using Engine.Common.Unit;
using Engine.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.ServiceEurekaNode
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
