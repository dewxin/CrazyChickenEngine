using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSerializer
{
    internal class RefTypeCode
    {
        public Type Type { get; set; }

        public string SerializerName { get; set; }

        public List<string> CodeLines { get; set; }

    }

    internal class RefTypeCodeCache
    {

        public static Dictionary<Type, RefTypeCode> Dict = new Dictionary<Type, RefTypeCode>();

        public static RefTypeCode SerializerCenter { get; set; }


        public static void Reset()
        {
            Dict.Clear();
        }
    }

}
