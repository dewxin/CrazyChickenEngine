// See https://aka.ms/new-console-template for more information

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
            var service = new ClientService();
            var config =new AllConfig() { 
                ServiceList = new List<HostService> { service },
            };
            node.Init(config);
            node.Run();

            service.Init();
            while (true)
            {
                if(service.NeedsHandleMsg)
                    service.Execute();
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
