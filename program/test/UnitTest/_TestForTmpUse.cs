global using Block.RPC.Emitter;
using Block.Assorted.Logging;
using Block0.MessagePack;
using Block0.MessagePack.Compiled;
using Block0.Threading;
using GameServerBase.ServerWorld;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
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
        public class JsonObj
        {
            public string EurekaMasterNodeIp { get; set; }
            public int EurekaMasterNodePort { get; set; }

            private const string shareJsonPath = @"config/share.json";

        }

        [TestMethod]
        public void TestMethod1()
        {

        }


    }
}
