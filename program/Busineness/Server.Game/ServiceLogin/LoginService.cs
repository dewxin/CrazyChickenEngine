
using GameServerBase;

namespace GameServerBase.ServerLogin
{
    public class LoginService : GameService
    {
        public LoginService()
        {
            ServiceType = ServiceTypeEnum.Login;
        }

        protected override void OnInitAddOn()
        {
            AddHandler(new Client2LoginHandler());
        }
    }
}
