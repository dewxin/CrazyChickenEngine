using AutoSerializer;
using ParamProject;
using ParamProject.Param;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UserProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ParamProject.SerializerCenter.Init();


            Class9 class9 = new Class9()
            {
                Color1 = ColorMini.Red,
                ColorList = new List<ColorMini>(),
            };

            var bytes = SerializerProxy.Serialize(typeof(Class9), class9);

            var arraySeg = new ArraySegment<byte>(bytes);
            var class9_2 =(Class9) SerializerProxy.Deserialize(typeof(Class9), arraySeg);

            Debug.Assert(class9_2.Color1 == ColorMini.Red);
        }
    }
}
