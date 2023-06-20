using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Param
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
        public string string1 {get;set;} 
    }

    public class Class3
    {
        public int int1 { get; set; }
        public Struct1 struct1 { get; set; }
    }

    class Class4
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
        public int int1 { get; set;}
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


    public class Class12
    {
        public List<List<Class5>> ClassList { get; set; }
        public List<List<Class1>> ClassList2 { get; set; }
    }

    public enum ServiceTypeEnum : byte
    {
        None = 0,
        NodeEureka,
        GlobalEureka,
        Logic,
        Login,
        World,
    }

    public struct ServiceInfo
    {
        public ServiceTypeEnum ServiceType { get; set; }
        public byte ServiceID { get; set; }
    }

    public class NodeInfo
    {
        public ushort NodeId { get; set; } //0 表示还没分配

        public string ServerIP { get; set; } //TODO 看看类型换成IPEndPoint好不好使
        public int ServerPort { get; set; }

        public List<ServiceInfo> ServiceInfoList { get; set; } = new List<ServiceInfo>();


        public List<ServiceInfo> GetServiceByType(ServiceTypeEnum serviceTypeEnum)
        {
            var retList = new List<ServiceInfo>();
            foreach (var serviceInfo in ServiceInfoList)
            {
                if (serviceInfo.ServiceType.Equals(serviceTypeEnum))
                    retList.Add(serviceInfo);
            }
            return retList;
        }
    }

}
