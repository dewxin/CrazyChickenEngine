using Block.Assorted.Logging;
using Block.Assorted.Logging.ILogImpl;
using Block0.Net.Serialize;
using Engine.Common.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.ServerEnd
{
    public static class HostBoostrap
    {
        private static HostNode serverNode;

        static HostBoostrap()
        {
            Console.CancelKeyPress += OnExit;
            AppDomain.CurrentDomain.ProcessExit += OnExit;
        }

        public static void Start(Type getterType, IEnumerable<string> serviceFileNames)
        {
            var allConfig = ConfigHelper.GetAllConfigFromFiles(getterType, serviceFileNames);
            serverNode = allConfig.ServerNode;

            Log.Init(new Log4NetImpl());
            serverNode.Init(allConfig);

            serverNode.Run();

            //TODO Command can be delivered to ServerThread by Console
            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }

            Console.WriteLine();

            serverNode.Stop();

            Console.WriteLine("The server was stopped!");
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

