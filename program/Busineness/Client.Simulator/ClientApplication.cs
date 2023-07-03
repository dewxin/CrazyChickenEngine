using Block.Assorted.Logging;
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
    internal class ClientApplication: HostApplication, IUnManagedJob
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

            var param = new ServiceTypeWrapper { ServiceType = ApplicationTypeEnum.World };

            var getServiceTask = 
                ApplicationFinder.ByEndPoint(eurekaMasterEndPoint, (byte)AppJobID.EurekaMasterID)
                .GetService<IEurekaMasterService>().GetNodesContainingService(param);


            getServiceTask.ContinueWith(() =>
            {
                var ret = getServiceTask.MethodCallResult;
                Log.Debug($"world node count is {ret.Count}");

                var loginInfo = ret.Single().GetServiceByType(ApplicationTypeEnum.Login).Single();

                var testTask = ApplicationFinder.ByEndPoint(eurekaMasterEndPoint, loginInfo.AppID).GetService<IClient2Login>().TestRpcRecord("str");


                testTask.ContinueWith(() =>
                {
                    var obj = testTask.MethodCallResult;
                    Log.Debug($"login test rpc result is {obj.Id}");
                });
            });


            getServiceTask.ContinueWith(() =>
            {
                var ret = getServiceTask.MethodCallResult;
                Log.Debug($"world node count is {ret.Count}");


                var worldInfo = ret.Single().GetServiceByType(ApplicationTypeEnum.World).Single();
                var worldClient = ApplicationFinder.ByEndPoint(eurekaMasterEndPoint, worldInfo.AppID).GetService<IClient2World>();


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
                    Log.Debug($"{result.MatchSucceed}, {result.PlayerData[0].Name} vs {result.PlayerData[1].Name}");

                });
            });
        }
    }
}
