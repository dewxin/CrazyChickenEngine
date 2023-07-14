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
using System.Diagnostics;

namespace Block0.Net
{

    public partial class UdpSocketManager
    {
        public static void Send(byte[] bytes, IPEndPoint ipEndPoint)
        {
            //远程主机可能会关闭连接，异常会在接收时抛出
            int sent = UdpClient.Send(bytes, bytes.Length, ipEndPoint);
			//TODO 如果不相等踢掉几个流量异常大的客户端。
            Debug.Assert(sent == bytes.Length);
        }
    }

    //TODO 新来一个客户端，就创建一个新的UdpClient?。这样客户端之间不会相互影响。
    public partial class UdpSocketManager
    {
        public static int ListenPort { get; private set; }
        public static UdpClient UdpClient { get; private set; }
        public static int AvailableData => UdpClient.Client.Available;

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

        public static bool TryGetMessage(out ArraySegment<byte> netMessage, out IPEndPoint remoteEndPoint)
        {
            netMessage = default;
            remoteEndPoint = point;

            try
            {
                if (UdpClient.Client.Available == 0)
                {
                    return false;
                }

                byte[] bytes = UdpClient.Receive(ref remoteEndPoint);
                Log.Debug($"Received Data lenth:{bytes.Length}");

                netMessage = new ArraySegment<byte>(bytes);
                return true;
            }
            catch (SocketException e)
            {
                Log.Error(e.ToString());
                return false;
                //throw e;
            }

        }
    }
}
