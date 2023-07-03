using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Block.Assorted.Logging;
using System.Runtime.CompilerServices;

namespace Block0.Net
{

    public partial class UdpSocketManager
    {
        public static void Send(byte[] bytes, IPEndPoint ipEndPoint)
        {
            UdpClient.Send(bytes, bytes.Length, ipEndPoint);
        }
    }

    public partial class UdpSocketManager
    {
        public static int ListenPort { get; private set; }
        public static UdpClient UdpClient { get; private set; }
        public static bool NeedHandle => UdpClient.Client.Available>0;

        private static IPEndPoint point = new IPEndPoint(IPAddress.Any, 0);

        public static void Init()
        {
            InitSocktConfig();
            Log.Info($"Ip:{SocketConfig.Inst.IP} Port: {SocketConfig.Inst.Port}");

            UdpClient = new UdpClient(ListenPort);
            UdpClient.Client.Blocking = false;
        }

        private static void InitSocktConfig()
        {
            ListenPort = UdpSocketHelper.GetPortByConfig(SocketConfig.Inst);
            if (UdpSocketHelper.TryGetLocalEndPoint(out var ipEndPoint))
            {
                SocketConfig.Inst.IP = ipEndPoint.Address.ToString();
            }
        }

        public static bool TryGetMessage(out NetMessage netMessage, out IPEndPoint remoteEndPoint)
        {
            netMessage = null;
            remoteEndPoint = point;

            try
            {
                if (UdpClient.Client.Available == 0)
                    return false;

                byte[] bytes = UdpClient.Receive(ref remoteEndPoint);
                Log.Debug($"Received Data lenth:{bytes.Length}");

                //TODO 如果同时到两个数据包 会出bug
                netMessage = NetMessage.Parse(new ArraySegment<byte>(bytes));
                return true;
            }
            catch (SocketException e)
            {
                Log.Error(e.ToString());
                throw e;
            }

        }
    }
}
