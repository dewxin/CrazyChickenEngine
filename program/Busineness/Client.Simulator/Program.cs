// See https://aka.ms/new-console-template for more information

using Block.Assorted.Logging;
using Client.Simulator.Logging;
using ClientSimulator;
using Engine.Common.AppCommon;
using Engine.Common.Unit;
using Share.Common.Unit;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ClientSimulator
{
    static class Program
    {
        static void Main(string[] args)
        {
            var node = new HostNode();
            node.Name = "Client";
            var application = new ClientApplication();
            var config =new AllConfig() {
                ApplicationList = new List<HostApplication> { application, new CommonApplication() },
            };

            Log.Init(new ConsoleImpl());
            LogExtension.Instance.SetLevel(LogLevel.Info);
            Share.Serializer.SerializerCenter.Init();
            node.Init(config);
            node.Run();
            application.Awake();

            while (true)
            {
                application.TryMatch();
                if (application.ExecutePriority > 0)
                    application.Execute();
            }

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }
            node.Stop();
        }
    }
}
