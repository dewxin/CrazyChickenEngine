using Block.Assorted;
using Block1.LocatableRPC;
using Chunk.LocatableRPC;
using Protocol.Param;
using Server.Common;
using Server.Common.Unit;
using System.Collections.Generic;
using System.Net;

namespace Server.Game._ServerGetter
{
    public class GlobalInfoGetter : IGlobalInfoGetter
    {
        public GlobalConfig GetGlobalConfig()
        {

            return new GlobalConfig()
            {
                EurekaMasterIPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23333),
            };
        }
    }
}
