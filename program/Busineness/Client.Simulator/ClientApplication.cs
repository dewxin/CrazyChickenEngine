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
using System.Threading;
using System.Threading.Tasks;

namespace ClientSimulator
{
    internal class ClientApplication: HostApplication, IUnManagedJob
    {
        private int playerCount = 0;

        private int matchSuccessCount = 0;

        private IClient2World worldClient;

        private long timeRecord;

        private int playerId = -1;
        private string playerName;
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
                worldClient = ApplicationFinder.ByEndPoint(eurekaMasterEndPoint, worldInfo.AppID).GetService<IClient2World>();


                //while(true)
                {

                    TryLogin();

                    //Thread.Sleep(0);

                }
                

            });
                

        }


        public void TryLogin()
        {

            var random = new Random((int)DateTime.Now.Ticks);
            playerCount = random.Next();
            playerCount++;
            string name = "dd" + playerCount;

            var loginTask = worldClient.Login(new WorldLoginData { Name = name });

            loginTask.ContinueWith(() =>
            {
                var loginRet = loginTask.MethodCallResult;

                playerId = loginRet.PlayerId;
                playerName= loginRet.PlayerName;



            });
        }


        public void TryMatch()
        {
            if (playerId <= 0)
            {
                Log.Info("playerId < 0");
                return;
            }
            var matchTask = worldClient.EnqueueForMatch(new PlayerMatchRequest() { Id = playerId, Name = playerName });

            matchTask.ContinueWith(() =>
            {
                var result = matchTask.MethodCallResult;

                if(result.MatchSucceed)
                    Log.Debug($"{result.MatchSucceed}, {result.PlayerData[0].Name} vs {result.PlayerData[1].Name}");

                matchSuccessCount++;
                if (matchSuccessCount % 1000 == 0)
                {
                    var oldTick = timeRecord;
                    timeRecord = DateTime.Now.Ticks;
                    long intervalTick = timeRecord - oldTick;
                    Log.Info($"matchSuccessCount={matchSuccessCount} intervalTick={intervalTick}");

                }

            });
        }
    }
}
