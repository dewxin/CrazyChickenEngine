using Block1.LocatableRPC;
using Engine.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.Unit
{
    public class AppFinderEx
    {
        public AppFinderEx(GameApplication gameApplication) { application = gameApplication; }
        private GameApplication application;

        public ApplicationServiceFinder ByLocal(byte destServiceId) => application.ApplicationFinder.ByLocal(destServiceId);

        public ApplicationServiceFinder ByEndPoint(IPEndPoint ipEndPoint, byte destServiceId)
            => application.ApplicationFinder.ByEndPoint(ipEndPoint, destServiceId);

        public ApplicationServiceFinder ByNodeId(ushort destNodeId, byte destServiceId)
        {
            if (destNodeId == application.MyNodeId)
                return ByLocal(destServiceId);


            var node = application.GetNode(destNodeId);
            var endpoint = new IPEndPoint(IPAddress.Parse(node.ServerIP), node.ServerPort);

            return application.ApplicationFinder.ByEndPoint(endpoint, destServiceId);
        }

        public ApplicationServiceFinder ByLocal(ApplicationTypeEnum serviceTypeEnum)
        {
            var servieInfo = application.GetMyNode().GetServiceByType(serviceTypeEnum).Single();

            return ByLocal(servieInfo.AppID);

        }
    }
}
