using Block.Rpc;
using Block0.Threading.Worker;
using Block1.LocatableRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.Unit
{
    public enum ServiceJobID : byte
    {
        None = WorkerJobID.UserNamed,
        EurekaMasterID,

    }

    public abstract class HostService : LPCServiceJob
    {

        public Action OnAfterMessage = delegate { };
        public Random GRandom { get; private set; }
        public ServiceFinder ServiceFinder { get; private set; }

        public virtual HostNode HostNode { get; set; }

        public HostService()
        {
            GRandom = new Random((int)DateTime.Now.Ticks);
            ServiceFinder = new ServiceFinder(MethodCallTaskCenter);
        }


        public override void Init()
        {
            base.Init();

            OnInit();
        }

        protected abstract void OnInit();

        public override void Execute()
        {
            base.Execute();

        }


        protected override void AfterHandleMessage()
        {
            OnAfterMessage.Invoke();
        }



        public void Stop()
        {
            //TODO
        }

    }
}
