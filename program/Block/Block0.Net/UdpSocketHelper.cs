using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Block0.Net
{
    public static class UdpSocketHelper
    {
        public static int GetPortByConfig(SocketConfig config)
        {
            int port;
            if (config.Port > 0)
                port = config.Port;
            else if (config.PortRangeMax > config.PortRangeMin && config.PortRangeMin > 0)
                port = UdpSocketHelper.GetUnusedPort(config.PortRangeMin, config.PortRangeMax);
            else
                port = UdpSocketHelper.GetUnusedPort(2000, 65500);

            config.Port = port;
            return port;
        }

        private static int GetUnusedPort(int rangeMin, int rangeMax)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            while (true)
            {
                var port = random.Next(rangeMin, rangeMax);

                if (!PortIsUsed(port))
                    return port;
            }
        }

        private static bool PortIsUsed(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var udpInfoArray = ipGlobalProperties.GetActiveUdpListeners();
            foreach (var info in udpInfoArray)
            {
                if (info.Port == port)
                    return true;
            }

            return false;
        }


        public static void ConfigureSocket(Socket socket, SocketConfig socketConfig)
        {

            if (socketConfig.SendTimeOut > 0)
                socket.SendTimeout = socketConfig.SendTimeOut;

            if (socketConfig.ReceiveBufferSize > 0)
                socket.ReceiveBufferSize = socketConfig.ReceiveBufferSize;

            if (socketConfig.SendBufferSize > 0)
                socket.SendBufferSize = socketConfig.SendBufferSize;

            socket.NoDelay = true;
            //socket.Blocking = false; data may loss
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
        }


        public static bool TryGetLocalEndPoint(out IPEndPoint ipEndPoint)
        {
            try
            {
                string localIP = string.Empty;
                using (Socket socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Connect("www.baidu.com", 80);
                    ipEndPoint = socket.LocalEndPoint as IPEndPoint;
                    return true;
                }
            }
            catch (SocketException ex)
            {
                ipEndPoint = null;
                return false;
            }

        }


    }


}
