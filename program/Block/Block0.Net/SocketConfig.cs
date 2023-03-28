using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Block0.Net
{

    public sealed class SocketConfig
    {
        public static SocketConfig Inst = new SocketConfig();

        public string IP { get; set; } = "127.0.0.1";

        public ProtocolType ProtocolType { get; set; } //TCP or UDP
        public int Port { get; set; }
        public ushort PortRangeMin { get; set; }
        public ushort PortRangeMax { get; set; }
        public int SendBufferSize { get; set; }
        public int ReceiveBufferSize { get; set; }

        public int SendTimeOut { get; set; }

    }
}
