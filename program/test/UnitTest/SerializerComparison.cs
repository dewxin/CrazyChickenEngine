using MessagePack;
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
    public class SerializerComparison
    {
        [TestMethod]
        public void TestMessagePackInherit()
        {
            // Call Serialize/Deserialize, that's all.
            MyClass myClass = new() { Age = 1, FirstName = "ddd" };
            byte[] bytes = MessagePackSerializer.Serialize(myClass);
            MyClass mc2 = MessagePackSerializer.Deserialize<MyClass>(bytes);

            Assert.AreEqual(mc2.Age, 1);

        }

        [TestMethod]
        public void TestMessagePackCompress()
        {
            // Call Serialize/Deserialize, that's all.
            MyClass myClass = new() { Age = 1, FirstName = "ddd34511111111111111111111111111",LastName="wowowowo12313131313" };
            var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray).WithCompressionMinLength(10);
            byte[] CompressedBytes = MessagePackSerializer.Serialize(myClass, lz4Options);
            byte[] bytes = MessagePackSerializer.Serialize(myClass);
            Assert.IsTrue(CompressedBytes.Length < bytes.Length);
            MyClassPlain mc2 = MessagePackSerializer.Deserialize<MyClassPlain>(CompressedBytes, lz4Options);

            Assert.AreEqual(mc2.Age, 1);

        }
    }
}
