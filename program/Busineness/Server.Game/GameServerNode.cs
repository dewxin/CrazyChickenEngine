using Block1.LocatableRPC;
using GameServerBase;
using Protocol.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class GameServerNode: ServerNode
    {

        public NodeInfo GetNodeInfo()
        {

            return new NodeInfo()
            {
                ServerIP = SocketConfig.IP,
                ServerPort = SocketConfig.Port,
                ServiceInfoList = GetServiceInfoList(),
            };

        }

        public List<ServiceInfo> GetServiceInfoList()
        {
            var ret = new List<ServiceInfo>();

            foreach (var service in serviceList)
            {
                if(service is GameService gameService)
                {
                    var serviceInfo = new ServiceInfo()
                    {
                        ServiceID = gameService.JobID,
                        ServiceType = gameService.ServiceType,
                    };

                    ret.Add(serviceInfo);
                }
            }

            return ret;

        }

    }
}
