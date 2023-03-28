// See https://aka.ms/new-console-template for more information

using ClientSimulator;
using System;

namespace ClientSimulator
{
    static class Program
    {
        static void Main(string[] args)
        {
            var node = new ClientNode();
            node.Init();
            node.Run();

            //TODO Command can be delivered to ServerThread by Console
            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }
            node.Stop();
        }
    }
}
