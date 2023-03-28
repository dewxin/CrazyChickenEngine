using MessagePack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class TestSerializerNull
    {
        [MessagePackObject]
        public class InnerClass
        {
            [Key(0)]
            public int I = 1;
        }

        [MessagePackObject]
        public class MyClass
        {
            [Key(0)]
            public InnerClass InnerClass { get; set; }   = new InnerClass();
        }

        [TestMethod]
        public void TestMethod1()
        {

            var myClass = new MyClass();
            byte[] bytes = MessagePackSerializer.Serialize(myClass);
            var myClassD = MessagePackSerializer.Deserialize<MyClass>(bytes);

            Assert.IsNotNull(myClassD);

            var myClass2 = new MyClass() { InnerClass = null };
            byte[] bytes2 = MessagePackSerializer.Serialize(myClass2);
            var myClassD2 = MessagePackSerializer.Deserialize<MyClass>(bytes2);

            Assert.IsNull(myClassD2.InnerClass);

        }
    }
}
