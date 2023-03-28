using Block0.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Data;

namespace UnitTest
{
    [TestClass]
    public class TestSerializerHelper
    {
        [TestMethod]
        public void TestMethod1()
        {

            var arraySeg = SerializerHelper.Serialize(new RpcDataAgres() { Val1=3});

            byte[] byteArray = new byte[arraySeg.Count];
            arraySeg.CopyTo(byteArray);

            var rpcData =(RpcDataAgres)SerializerHelper.Deserialize(typeof(RpcDataAgres), byteArray);

            Assert.AreEqual(3, rpcData.Val1);

        }

        [TestMethod]
        public void TestMethod2()
        {

            var arraySeg = SerializerHelper.Serialize(new RpcDataNotAgres() { Val1 = 3 });

            byte[] byteArray = new byte[arraySeg.Count];
            arraySeg.CopyTo(byteArray);

            var rpcData = (RpcDataNotAgres)SerializerHelper.Deserialize(typeof(RpcDataNotAgres), byteArray);

            Assert.AreEqual(3, rpcData.Val1);

        }

    }
}
