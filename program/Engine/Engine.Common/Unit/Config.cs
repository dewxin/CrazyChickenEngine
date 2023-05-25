using Block0.Net;

using Engine;
using Engine.Common;
using Engine.Common.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.Unit
{
    public interface IServerInfoGetter
    {
        HostNode GetServer();
        SocketConfig GetSocketConfig();
        List<HostService> GetServiveList();
    }

    public sealed class GlobalConfig
    {
        public static GlobalConfig Inst = new GlobalConfig();
        public IPEndPoint EurekaMasterIPEndPoint { get; set; }
    }

    public interface IGlobalInfoGetter
    {
        GlobalConfig GetGlobalConfig();
    }

    public class AllConfig
    {
        public GlobalConfig GlobalConfig { get; set; } = new GlobalConfig();
        public SocketConfig SocketConfig { get; set; } = new SocketConfig();
        public HostNode ServerNode { get; set; }
        public List<HostService> ServiceList { get; set; }
    }
}
