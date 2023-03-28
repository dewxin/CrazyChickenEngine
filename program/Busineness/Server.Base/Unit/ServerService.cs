global using Block.Assorted.Logging;
using System.Net;
using Server.Common.Unit;
using Block0.Threading.Worker;
using Block.Assorted;

namespace Server.Common.Unit
{
    public abstract partial class ServerService
    {
        public Random GRandom { get; private set; }


        public virtual ServerNode ServerNode { get; set; }


        #region Bootstrap Thread
        public ServerService()
        {
            GRandom = new Random((int)DateTime.Now.Ticks);
        }



        #endregion


        protected abstract void OnInit();



        public void Stop()
        {
            //TODO
        }

    }

}
