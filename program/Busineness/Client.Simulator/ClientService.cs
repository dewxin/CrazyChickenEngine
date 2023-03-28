using Chunk.LocatableRPC;
using Protocol.Param;
using Protocol.Service;
using Protocol.Service.Login;
using Protocol.Service.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientSimulator
{
    internal class ClientService: LPCServiceJob
    {

        public ClientNode ClientNode { get; set; }

        public override void Init()
        {
            base.Init();


            GetWorldNode();
        }

        public void GetWorldNode()
        {

            var eurekaMasterEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23333);

            var param = new ServiceTypeWrapper { ServiceType = ServiceTypeEnum.World };

            //现在是EurekaMasterService 是4
            var getServiceTask = ServiceFinder.ByEndPoint(eurekaMasterEndPoint, 4)
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


                var random = new Random((int)DateTime.Now.Ticks);
                string name = "dd" + random.Next();

                var loginTask = worldClient.Login(new WorldLoginData { Name = name });

                loginTask.ContinueWith(() =>
                {
                    var loginRet = loginTask.MethodCallResult;
                    var matchTask = worldClient.EnqueueForMatch(new PlayerMatchRequest() {Id = loginRet.PlayerId,Name = loginRet.PlayerName });

                    matchTask.ContinueWith(() =>
                    {
                        var result = matchTask.MethodCallResult;
                        Console.WriteLine($"{result.MatchSucceed}, {result.PlayerData[0].Name} vs {result.PlayerData[1].Name}");

                    });
                });
                


            });
                

        }
    }
}
