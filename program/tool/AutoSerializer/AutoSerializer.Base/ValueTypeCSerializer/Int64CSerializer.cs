// Genereated at time: 28/06/2023 15:12:26

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSerializer
{
    internal class Int64CSerializer : ISerializer
    {

        public void Serialize(BinaryWriter writer, object obj)
        {
            Int64C val = (Int64C)obj;
            //type.IsPrimitive
            writer.Write(val.Value);
        }

        public object Deserialize(BinaryReader reader)
        {
            Int64C val = new Int64C();
            //type.IsPrimitive
            val.Value = reader.ReadInt64();
            return val;
        }

    }

}
