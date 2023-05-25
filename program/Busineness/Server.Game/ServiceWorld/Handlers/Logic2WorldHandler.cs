using Block.Assorted.Logging;
using Share.IService.Service.World;

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
