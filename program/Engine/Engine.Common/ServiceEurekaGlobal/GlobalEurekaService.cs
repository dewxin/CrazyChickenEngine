using System;
using System.Net;
using Block0.Threading.Worker;
using Engine.Common.Unit;
using Engine.IService;

namespace Engine.Common.ServiceEurekaGlobal
{
    public partial class GlobalEurekaService:IUniqueJobID
    {
        public byte UniqueID => (byte)ServiceJobID.EurekaMasterID;
    }

    public partial class GlobalEurekaService : GameService
    {
        public EurekaNodeManager NodeInfoManager { get; set; } = new EurekaNodeManager();

        public GlobalEurekaService()
        {
            ServiceType = ServiceTypeEnum.GlobalEureka;
        }

        protected override void OnInitAddOn()
        {
            AddHandler(new GlobalEurekaHandler());
        }


    }
}
