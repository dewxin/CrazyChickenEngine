using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.IService.Param
{
    #region Client to Login

    //TODO 这部分数据加密
    public class AccountData
    {
        public string Account { get; set; }

        public string Password { get; set; }
    }

    public enum RegisterResultEnum
    {
        None = 0,
        Succeed = 1,
        AccountAlreadyRegistered = 2,
        FailDueToDbError = 3,
    }

    public class RegisterResult
    {
        public RegisterResultEnum Result { get; set; } = RegisterResultEnum.Succeed;
    }


    public enum LoginResultEnum
    {
        None = 0,
        Succeed = 1,
        AccountNotExist,
        PasswordWrong,
        ServerNotAvail,
    }

    //TODO 现在Property会生成注解，但是Field不会。。。是不是有问题？
    public class LoginResult
    {
        public LoginResultEnum Result { get; set; } = LoginResultEnum.Succeed;
        public string GateServerIP { get; set; }
        public int GateServerPort { get; set; }
        public string Token { get; set; }
    }


    public class TestRpcRecordResult
    {
        public int Id { get; set; }
    }

    #endregion
}
