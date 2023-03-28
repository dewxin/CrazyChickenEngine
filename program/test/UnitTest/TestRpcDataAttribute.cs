using Block.RPC.Emitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Data;

namespace UnitTest
{
    [TestClass]
    public class TestRpcDataAttribute
    {
        [TestMethod]
        public void Method1()
        {
            bool agressiveCompress = typeof(RpcDataAgres).IsAgressiveCompress();

            Assert.IsTrue(agressiveCompress);
        }
    }

}
