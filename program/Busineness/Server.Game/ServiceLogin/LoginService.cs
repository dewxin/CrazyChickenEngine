using GameServerBase;
using Protocol;

namespace GameServerBase.ServerLogin
{
    public class LoginService : GameService
    {
        public LoginService()
        {
            ServiceType = Protocol.Param.ServiceTypeEnum.Login;
        }

        protected override void OnInitAddOn()
        {
            AddHandler(new Client2LoginHandler());
        }
    }
}
