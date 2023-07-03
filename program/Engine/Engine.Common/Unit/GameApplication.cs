using System;
using System.Collections.Generic;
using Block.Assorted.Logging;
using System.Diagnostics;
using Block0.Threading.Worker;
using Engine.Common.Unit;
using Engine.IService;

namespace Engine.Common.Unit
{

    public abstract partial class GameApplication : HostApplication 
    {
        public ApplicationTypeEnum ApplicationType { get; set; } = ApplicationTypeEnum.None;

        public GameApplication():base()
        {
            FindApp = new AppFinderEx(this);
        }

        protected abstract void OnInitAddOn();
        protected sealed override void OnInit()
        {
            Debug.Assert(ApplicationType != ApplicationTypeEnum.None);
            Log.Debug($"{ApplicationType}");

            AddServiceHandler(new CommonServiceHandler());

            OnInitAddOn();
        }


        public void AddServiceHandler(params GameServiceHandler[] messageServiceHandlers)
        {
            foreach(var handler in messageServiceHandlers)
                handler.Application = this;

            base.AddServiceHandler(messageServiceHandlers);
        }

    }
}
