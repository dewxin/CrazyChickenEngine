using System;
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
using EasyPerformanceCounter;

namespace Engine.Common.ServiceEurekaNode
{
    public class NodeEurekaApplication : GameApplication
    {
        public NodeEurekaApplication()
        {
            ApplicationType = ApplicationTypeEnum.NodeEureka;
        }

        protected override void OnInitAddOn()
        {
            AddServiceHandler(new NodeEurekaHandler());
            RegisterOnEureka();

        }

        public override void Update()
        {

        }

        private void RegisterOnEureka()
        {
            var service = FindApp.ByEndPoint(GlobalConfig.Inst.EurekaMasterIPEndPoint, (byte)AppJobID.EurekaMasterID)
                .GetService<IEurekaMasterService>();

            var pubTask = service.PubHostInfoAndSub(HostNode.GetNodeInfo());

            pubTask.ContinueWith(() =>
            {
                var ret = pubTask.MethodCallResult;
                foreach (var aServiceInfo in HostNode.GetApplicationInfoList())
                {
                    FindApp.ByLocal(aServiceInfo.AppID).GetService<ICommonService>()
                        .NotifyNodePubRet(ret);
                }

                Log.Debug($"nodeId is {ret.NodeId}");
            });

        }
    }
}
