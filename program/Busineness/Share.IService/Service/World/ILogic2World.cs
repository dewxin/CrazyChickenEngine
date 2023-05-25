using Block.RPC.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.IService.Service.World
{
    public interface ILogic2World : IWorldService
    {
        //WorldLoginRet PlayerLogin(WorldLoginData worldLoginData);
        void Hello(string word);

    }
}
