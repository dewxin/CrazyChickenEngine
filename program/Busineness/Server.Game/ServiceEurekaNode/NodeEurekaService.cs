using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Chunk.LocatableRPC;
using Protocol.Param;
using Block1.LocatableRPC;
using Block.Assorted.Logging;
using Server.Common;

namespace Server.Game.ServiceEurekaNode
{
    public class NodeEurekaService : GameService
    {
        public NodeEurekaService()
        {
            ServiceType = ServiceTypeEnum.EurekaClient;
        }

        protected override void OnInitAddOn()
        {
            AddHandler(new NodeEurekaHandler());
            RegisterOnEureka();

        }

        private void RegisterOnEureka()
        {
            var service = FindService.ByEndPoint(GlobalConfig.Inst.EurekaMasterIPEndPoint, (byte)ServiceTaskID.EurekaMasterID)
                .GetRpc<IEurekaMasterService>();

            var pubTask = service.PubHostInfoAndSub(ServerNode.GetNodeInfo());

            pubTask.ContinueWith(() =>
            {
                var ret = pubTask.MethodCallResult;
                foreach (var aServiceInfo in ServerNode.GetServiceInfoList())
                {
                    FindService.ByLocal(aServiceInfo.ServiceID).GetRpc<ICommonService>()
                        .NotifyNodePubRet(ret);
                }

                Log.Debug($"nodeId is {ret.NodeId}");
            });

        }
    }
}
