global using Block.RPC.Emitter;
using Block.Assorted.Logging;
using Block0.Threading;
using GameServerBase.ServerWorld;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Protocol.Param;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class _TestForTmpUse
    {
        [TestMethod]
        public void TestMethod1()
        {
            Block0.Threading.PriorityQueue<string, long> pipePriorityQueue = new();

            pipePriorityQueue.Enqueue("a1", 88);
            pipePriorityQueue.Enqueue("a2", 100);
            pipePriorityQueue.Enqueue("a3", 2);
            pipePriorityQueue.Enqueue("a4", 55);
            pipePriorityQueue.Enqueue("a5", 222);
            pipePriorityQueue.Enqueue("a6", 66);
            pipePriorityQueue.Enqueue("a7", 5);

            var pipeName = pipePriorityQueue.Dequeue();
            Console.WriteLine(pipeName);
            pipeName = pipePriorityQueue.Dequeue();
            Console.WriteLine(pipeName);

        }

    }
}
