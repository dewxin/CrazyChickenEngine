// Genereated at time: 28/06/2023 15:12:26

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSerializer
{
    internal class Int32CSerializer : ISerializer
    {

        public void Serialize(BinaryWriter writer, object obj)
        {
            Int32C val = (Int32C)obj;
            //type.IsPrimitive
            writer.Write(val.Value);
        }

        public object Deserialize(BinaryReader reader)
        {
            Int32C val = new Int32C();
            //type.IsPrimitive
            val.Value = reader.ReadInt32();
            return val;
        }

    }

}
