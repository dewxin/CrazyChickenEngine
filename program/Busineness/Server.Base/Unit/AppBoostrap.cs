using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Common.Unit
{
    public static class AppBoostrap
    {
        private static ServerNode serverNode;

        static AppBoostrap()
        {
            Console.CancelKeyPress += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;
        }

        public static void Start(IEnumerable<string> serviceFileNames)
        {

            var allConfig = ServerStartHelper.GetAllConfigFromFiles(serviceFileNames);
            serverNode = allConfig.ServerNode;
            serverNode.Init(allConfig);
            serverNode.Run();

            //TODO Command can be delivered to ServerThread by Console
            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }

            Console.WriteLine();

            //Stop the appServer
            serverNode.Stop();

            Console.WriteLine("The server was stopped!");
            //Console.ReadKey();
        }

        static void OnExit(object sender, ConsoleCancelEventArgs e)
        {
            serverNode.Stop();
        }

        static void OnExit(object sender, EventArgs e)
        {
            serverNode.Stop();
        }
    }
}
