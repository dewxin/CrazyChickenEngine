using Block.Rpc;
using Block0.Threading.Worker;
using Block1.LocatableRPC;
using Block1.LocatableRPC.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.Unit
{
    public enum AppJobID : byte
    {
        None = WorkerJobID.UserNamed,
        EurekaMasterID,

    }

    public abstract class HostApplication : LpcAppJob
    {

        public Action OnAfterMessage = delegate { };
        public Random GRandom { get; private set; }
        public ApplicationFinder ApplicationFinder { get; private set; }

        public virtual HostNode HostNode { get; set; }

        public HostApplication()
        {
            GRandom = new Random((int)DateTime.Now.Ticks);
            ApplicationFinder = new ApplicationFinder(MethodCallTaskCenter);
        }


        public override void Awake()
        {
            base.Awake();

            OnInit();
        }

        protected abstract void OnInit();



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
