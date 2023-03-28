using Block.Rpc;
using Block0.Threading.Worker;
using Chunk.LocatableRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.Common.Unit
{
    public abstract partial class ServerService:LPCServiceJob
    {
        public override void Init()
        {
            base.Init();

            OnInit();
        }

        public override void Execute()
        {
            base.Execute();

        }

        public Action OnAfterMessage = delegate { };

        protected override void AfterHandleMessage()
        {
            OnAfterMessage.Invoke();
        }

    }
}
