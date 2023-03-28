using System;
using System.Net;
using Block0.Threading.Worker;
using Chunk.LocatableRPC;

namespace Server.Game.ServiceEurekaGlobal
{
    public partial class GlobalEurekaService:IUniqueTaskID
    {
        public byte UniqueID => (byte)ServiceTaskID.EurekaMasterID;
    }

    public partial class GlobalEurekaService : GameService
    {
        public EurekaNodeManager NodeInfoManager { get; set; } = new EurekaNodeManager();

        public GlobalEurekaService()
        {
            ServiceType = Protocol.Param.ServiceTypeEnum.EurekaMaster;
        }

        protected override void OnInitAddOn()
        {
            //启动时拿到的ID肯定是初始id，也就是1
            AddHandler(new GlobalEurekaHandler());
        }


    }
}
