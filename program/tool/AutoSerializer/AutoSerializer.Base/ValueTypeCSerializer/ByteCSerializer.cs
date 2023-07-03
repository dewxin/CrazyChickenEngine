// Genereated at time: 28/06/2023 15:12:26

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSerializer
{
    internal class ByteCSerializer : ISerializer
    {

        public void Serialize(BinaryWriter writer, object obj)
        {
            ByteC val = (ByteC)obj;
            //type.IsPrimitive
            writer.Write(val.Value);
        }

        public object Deserialize(BinaryReader reader)
        {
           ByteC val = new ByteC();
            //type.IsPrimitive
            val.Value = reader.ReadByte();
            return val;
        }

    }

}
