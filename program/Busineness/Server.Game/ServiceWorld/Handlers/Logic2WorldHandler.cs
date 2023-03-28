using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Service;
using Protocol.Service.World;
using Block.Assorted.Logging;

namespace GameServerBase.ServerWorld
{
    public class Logic2WorldHandler : GameServiceHandler, ILogic2World
    {

        public Logic2WorldHandler()
        {
        }

        public void Hello(string word)
        {
            Log.Debug(word);
        }

    }
}
