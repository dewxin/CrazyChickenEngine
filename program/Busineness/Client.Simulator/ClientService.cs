using Block0.Threading.Worker;
using Block1.LocatableRPC;
using Engine.Common.Unit;
using Engine.IService;
using Share.Common.Unit;
using Share.IService.Param;
using Share.IService.Service.Login;
using Share.IService.Service.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientSimulator
{
    internal class ClientService: HostService, IUnManagedJob
    {

        protected override void OnInit()
        {

            GetWorldNode();
        }

        public void GetWorldNode()
        {

            var shareJson = ShareJson.Inst;
            var eurekaMasterEndPoint = new IPEndPoint(IPAddress.Parse(shareJson.EurekaMasterNodeIp), shareJson.EurekaMasterNodePort);

            //var eurekaMasterEndPoint = new IPEndPoint(IPAddress.Parse("39.184.9.18"), 8030);

            var param = new ServiceTypeWrapper { ServiceType = ServiceTypeEnum.World };

            var getServiceTask = 
                ServiceFinder.ByEndPoint(eurekaMasterEndPoint, (byte)ServiceJobID.EurekaMasterID)
                .GetRpc<IEurekaMasterService>().GetNodesContainingService(param);


            getServiceTask.ContinueWith(() =>
            {
                var ret = getServiceTask.MethodCallResult;
                Console.WriteLine($"world node count is {ret.Count}");

                var loginInfo = ret.Single().GetServiceByType(ServiceTypeEnum.Login).Single();

                var testTask = ServiceFinder.ByEndPoint(eurekaMasterEndPoint, loginInfo.ServiceID).GetRpc<IClient2Login>().TestRpcRecord();


                testTask.ContinueWith(() =>
                {
                    var obj = testTask.MethodCallResult;
                    Console.WriteLine($"login test rpc result is {obj.Id}");
                });
            });


            getServiceTask.ContinueWith(() =>
            {
                var ret = getServiceTask.MethodCallResult;
                Console.WriteLine($"world node count is {ret.Count}");


                var worldInfo = ret.Single().GetServiceByType(ServiceTypeEnum.World).Single();
                var worldClient = ServiceFinder.ByEndPoint(eurekaMasterEndPoint, worldInfo.ServiceID).GetRpc<IClient2World>();


                TryLogin(worldClient);
                TryLogin(worldClient);
                

            });
                

        }


        private void TryLogin(IClient2World worldClient)
        {

            var random = new Random((int)DateTime.Now.Ticks);
            string name = "dd" + random.Next();

            var loginTask = worldClient.Login(new WorldLoginData { Name = name });

            loginTask.ContinueWith(() =>
            {
                var loginRet = loginTask.MethodCallResult;
                var matchTask = worldClient.EnqueueForMatch(new PlayerMatchRequest() { Id = loginRet.PlayerId, Name = loginRet.PlayerName });

                matchTask.ContinueWith(() =>
                {
                    var result = matchTask.MethodCallResult;
                    Console.WriteLine($"{result.MatchSucceed}, {result.PlayerData[0].Name} vs {result.PlayerData[1].Name}");

                });
            });
        }
    }
}
