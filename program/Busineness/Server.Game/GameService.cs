global using Block.RPC.Task;
global using Protocol.Service.Logic;
global using Protocol.Service.World;
global using Protocol.Service.Login;
global using Protocol.Service;
global using Server.Common.Unit;

using Protocol.Param;
using System;
using System.Collections.Generic;
using Block.Assorted.Logging;
using System.Diagnostics;
using Block0.Threading.Worker;

namespace Server.Game 
{
    public enum ServiceTaskID:byte
    {
        None = WorkerJobID.SelfDefineMin,
        EurekaMasterID,

        Max = WorkerJobID.SelfDefineMax,
    }

    public abstract partial class GameService : ServerService 
    {
        public ServiceTypeEnum ServiceType { get; set; } = ServiceTypeEnum.None;

        public override GameServerNode ServerNode => base.ServerNode as GameServerNode;

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
