
using AutoSerializer;
using AutoSerializer.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Param;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SerializerGenTest
{

    [TestClass]
    [AutoSerializer.Ignore]
    public class TestSerializerGen
    {
        [TestMethod]
        public void TestMethod1()
        {
            AutoSerializer.GenCore.Instance.Work(typeof(TestSerializerGen).Assembly, "Param");
        }


        [TestMethod]
        public void Test()
        {
            Type list_int_Type = typeof(List<int>);
            Type list_string_Type = typeof(List<string>);
            Type list_list_int_Type = typeof(List<List<string>>);

            var typeList = new List<Type>() { list_int_Type, list_string_Type, list_list_int_Type };

            foreach (Type type in typeList)
            {
                Console.WriteLine(type.FullName);
                Console.WriteLine(type.Name);
                Console.WriteLine(TypeHelper.GetTypeName(type));
            }

        }

        [TestMethod]
        public void Test2()
        {
            Type type = typeof(MethodTask<Class1>);
            Console.WriteLine(type.Name);
            Console.WriteLine(type.Namespace);

            type = typeof(Dictionary<int, string>);
            Console.WriteLine(type.Name);
            Console.WriteLine(type.Namespace);

        }

    }



}
