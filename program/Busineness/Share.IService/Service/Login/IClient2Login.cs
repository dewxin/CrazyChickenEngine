using Block.RPC.Attr;
using Block.RPC.Task;
using Share.IService.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.IService.Service.Login
{
    public interface IClient2Login : ILoginService
    {
        MethodCallTask<RegisterResult> RegisterAccount(AccountData accountData);

        MethodCallTask<LoginResult> PlayerLogin(AccountData logicLoginData);

        MethodCallTask<TestRpcRecordResult> TestRpcRecord();

    }


}
