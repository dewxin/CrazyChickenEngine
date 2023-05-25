using Block.Assorted;
using Block1.LocatableRPC;
using Share.Common.Unit;
using System.Collections.Generic;
using System.Net;
using Engine.Common.Unit;
using Engine.IService;
namespace Server.Game._ServerGetter
{
    public class GlobalInfoGetter : IGlobalInfoGetter
    {
        public GlobalConfig GetGlobalConfig()
        {

            var shareJson = ShareJson.Inst;

            IPAddress ipAddress = IPAddress.Parse(shareJson.EurekaMasterNodeIp);
            return new GlobalConfig()
            {
                EurekaMasterIPEndPoint = new IPEndPoint(ipAddress, shareJson.EurekaMasterNodePort),
            };
        }
    }
}
