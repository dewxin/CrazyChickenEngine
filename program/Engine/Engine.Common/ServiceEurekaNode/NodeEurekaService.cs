﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Block1.LocatableRPC;
using Block.Assorted.Logging;
using Engine.Common.Unit;
using Engine.IService;

namespace Engine.Common.ServiceEurekaNode
{
    public class NodeEurekaService : GameService
    {
        public NodeEurekaService()
        {
            ServiceType = ServiceTypeEnum.NodeEureka;
        }

        protected override void OnInitAddOn()
        {
            AddHandler(new NodeEurekaHandler());
            RegisterOnEureka();

        }

        private void RegisterOnEureka()
        {
            var service = FindService.ByEndPoint(GlobalConfig.Inst.EurekaMasterIPEndPoint, (byte)ServiceJobID.EurekaMasterID)
                .GetRpc<IEurekaMasterService>();

            var pubTask = service.PubHostInfoAndSub(HostNode.GetNodeInfo());

            pubTask.ContinueWith(() =>
            {
                var ret = pubTask.MethodCallResult;
                foreach (var aServiceInfo in HostNode.GetServiceInfoList())
                {
                    FindService.ByLocal(aServiceInfo.ServiceID).GetRpc<ICommonService>()
                        .NotifyNodePubRet(ret);
                }

                Log.Debug($"nodeId is {ret.NodeId}");
            });

        }
    }
}