using System;
using System.Net;
using Block0.Threading.Worker;
using Engine.Common.Unit;
using Engine.IService;

namespace Engine.Common.ServiceEurekaGlobal
{
    public partial class GlobalEurekaApplication:IUniqueJobID
    {
        public byte UniqueID => (byte)AppJobID.EurekaMasterID;
    }

    public partial class GlobalEurekaApplication : GameApplication
    {
        public EurekaNodeManager NodeInfoManager { get; set; } = new EurekaNodeManager();

        public GlobalEurekaApplication()
        {
            ApplicationType = ApplicationTypeEnum.GlobalEureka;
        }

        protected override void OnInitAddOn()
        {
            AddServiceHandler(new GlobalEurekaHandler());
        }


    }
}
