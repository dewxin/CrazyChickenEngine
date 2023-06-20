#region _Generate_Time_
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSerializer;

namespace AutoSerializer //namespace会被替换
{
    public class SerializerCenter
    {
        public static void Init()
        {
#if _AddSerializer_
            SerializerProxy.AddSerializer(typeof(_TypeFullName_), new _TypeSerializerName_());
#endif

        }

    }
}
