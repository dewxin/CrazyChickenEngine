using Block.RPC.Attr;
using Block.RPC.Task;
using Protocol.Param;
using Protocol.Service.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Service.Login
{
    public interface IClient2Login:ILoginService
    {
        MethodCallTask<RegisterResult> RegisterAccount(AccountData accountData);

        MethodCallTask<LoginResult> PlayerLogin(AccountData logicLoginData);

        MethodCallTask<TestRpcRecordResult> TestRpcRecord();

    }


}
