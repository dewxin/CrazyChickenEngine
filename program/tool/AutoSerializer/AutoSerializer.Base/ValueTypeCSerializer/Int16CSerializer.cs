// Genereated at time: 28/06/2023 15:12:26

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSerializer
{
    internal class Int16CSerializer : ISerializer
    {

        public void Serialize(BinaryWriter writer, object obj)
        {
            Int16C val = (Int16C)obj;
            //type.IsPrimitive
            writer.Write(val.Value);
        }

        public object Deserialize(BinaryReader reader)
        {
            Int16C val = new Int16C();
            //type.IsPrimitive
            val.Value = reader.ReadInt16();
            return val;
        }

    }

}
