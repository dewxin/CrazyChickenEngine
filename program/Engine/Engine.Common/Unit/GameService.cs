using System;
using System.Collections.Generic;
using Block.Assorted.Logging;
using System.Diagnostics;
using Block0.Threading.Worker;
using Engine.Common.Unit;
using Engine.IService;

namespace Engine.Common.Unit
{

    public abstract partial class GameService : HostService 
    {
        public ServiceTypeEnum ServiceType { get; set; } = ServiceTypeEnum.None;

        public GameService() 
        {
            FindService = new ServiceFinderWrapper(this);
        }

        protected abstract void OnInitAddOn();
        protected sealed override void OnInit()
        {
            Debug.Assert(ServiceType != ServiceTypeEnum.None);
            Log.Debug($"{ServiceType}");

            AddHandler(new CommonServiceHandler());

            OnInitAddOn();
        }


        public void AddHandler(params GameServiceHandler[] messageServiceHandlers)
        {
            foreach(var handler in messageServiceHandlers)
                handler.GameService = this;

            base.AddHandler(messageServiceHandlers);
        }

    }
}
