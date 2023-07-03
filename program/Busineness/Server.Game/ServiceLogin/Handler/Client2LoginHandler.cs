using Block.RPC.Task;
using Share.IService.Param;
using Share.IService.Service.Login;
using Share.IService.Service.World;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameServerBase.ServerLogin
{
    public class Client2LoginHandler : GameServiceHandler, IClient2Login
    {
        public Client2LoginHandler()
        {
        }

        public LoginApplication LoginService => base.Application as LoginApplication;


        public MethodCallTask<LoginResult> PlayerLogin(AccountData loginData)
        {
            throw new NotImplementedException();
        }


        public MethodCallTask<RegisterResult> RegisterAccount(AccountData accountData)
        {
            return new RegisterResult { Result = RegisterResultEnum.Succeed };
        }

        public MethodCallTask<TestRpcRecordResult> TestRpcRecord(string str)
        {
            var worldClient = LoginService.FindApp.ByLocal(ApplicationTypeEnum.World).GetService<ILogin2World>();
            
            var task1 = worldClient.TestRpcRecord();
            var task2 = worldClient.TestRpcRecord();



            var ret = new MethodCallTask<TestRpcRecordResult>();
            ret.Action = () =>
            {
                ret.MethodCallResult = new() { Id = task1.MethodCallResult.Id + task2.MethodCallResult.Id };
            };

            ret.Wait(task1);
            ret.Wait(task2);


            return ret;

        }
    }
}
