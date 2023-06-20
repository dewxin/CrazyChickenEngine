using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParamProject.Param
{
    public struct Struct1
    {
        public int int1 { get; set; }
        public string string1 { get; set; }
    }

    public class Class1
    {
        public int int1 { get; set; }
    }

    public class Class2
    {
        public string string1 { get; set; }
    }

    public class Class3
    {
        public int int1 { get; set; }
        public Struct1 struct1 { get; set; }
    }

    public class Class4
    {
        public List<int> list1 { get; set; }
    }

    public class Class5
    {
        public List<Class1> list1 { get; set; }
        public List<Class7> list2 { get; set; }
    }

    public class Class7
    {
        public int int1 { get; set; }
    }

    public class Class6
    {
        public List<Struct1> list1 { get; set; }
    }


    public enum Color
    {
        Red = 0,
        Green = 1,
        Blue = 2,
    }

    public class Class8
    {
        public Color Color1 { get; set; }
        public List<Color> ColorList { get; set; }
    }

    public enum ColorMini : byte
    {
        Red = 0,
        Green = 1,
        Blue = 2,
    }

    public class Class9
    {
        public ColorMini Color1 { get; set; }
        public List<ColorMini> ColorList { get; set; }
    }

    public struct Struct2
    {
        public Struct1 struct1_1 { get; set;}
        public Struct1 struct1_2 { get; set;}
    }

    public class Class10
    {
        public Struct2 struct2 { get; set;}
    }

    public struct Struct3
    {
        public Struct2 struct2_1{get;set; }
        public Struct2 struct2_2{get;set; }
    }

    public class Class11
    {
        public Struct3 struct3 { get; set; }
    }

    public class Class12
    {
        public List<List<Class5>> ClassList { get; set; }
        public List<List<Class1>> ClassList2 { get; set; }
    }

}
