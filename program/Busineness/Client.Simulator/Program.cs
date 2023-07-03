// See https://aka.ms/new-console-template for more information

using Block.Assorted.Logging;
using Client.Simulator.Logging;
using ClientSimulator;
using Engine.Common.Unit;
using Share.Common.Unit;
using System;
using System.Collections.Generic;

namespace ClientSimulator
{
    static class Program
    {
        static void Main(string[] args)
        {
            var node = new HostNode();
            var application = new ClientApplication();
            var config =new AllConfig() {
                ApplicationList = new List<HostApplication> { application },
            };

            Log.Init(new ConsoleImpl());
            Share.Serializer.SerializerCenter.Init();
            node.Init(config);
            node.Run();
            application.Awake();

            while (true)
            {
                if(application.NeedsHandleMsg)
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
