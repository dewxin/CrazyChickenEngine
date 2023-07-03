
using GameServerBase;

namespace GameServerBase.ServerLogin
{
    public class LoginApplication : GameApplication
    {
        public LoginApplication()
        {
            ApplicationType = ApplicationTypeEnum.Login;
        }

        protected override void OnInitAddOn()
        {
            AddServiceHandler(new Client2LoginHandler());
        }
    }
}
