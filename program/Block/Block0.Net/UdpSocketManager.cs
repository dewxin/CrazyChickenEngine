using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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
        private static Action<NetMessage, IPEndPoint> OnReceiveMessage;

        public static int ListenPort { get; private set; }

        public static UdpClient UdpClient { get; private set; }

        public static void Init(Action<NetMessage,IPEndPoint> onReceiveMessageAction)
        {
            OnReceiveMessage= onReceiveMessageAction;
            ListenPort = UdpSocketHelper.GetPortByConfig(SocketConfig.Inst);

            Thread thread = new Thread(StartListener);

            thread.Start();
        }

        private static void StartListener()
        {

            UdpClient = new UdpClient(ListenPort);
            //IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, ListenPort);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any,0);

            //try
            {
                while (true)
                {
                    byte[] bytes = UdpClient.Receive(ref remoteEndPoint);

                    NetMessage netMessage = NetMessage.Parse(new ArraySegment<byte>(bytes));
                    OnReceiveMessage?.Invoke(netMessage, remoteEndPoint);
                }
            }
            //catch (SocketException e)
            //{
            //    Console.WriteLine(e);
            //}
            //finally
            //{
            //    UdpClient.Close();
            //}
        }
    }
}
