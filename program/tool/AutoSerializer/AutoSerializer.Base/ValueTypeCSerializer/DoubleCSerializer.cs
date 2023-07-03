// Genereated at time: 28/06/2023 15:12:26

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSerializer
{
    internal class DoubleCSerializer : ISerializer
    {

        public void Serialize(BinaryWriter writer, object obj)
        {
            DoubleC val = (DoubleC)obj;
            //type.IsPrimitive
            writer.Write(val.Value);
        }

        public object Deserialize(BinaryReader reader)
        {
            DoubleC val = new DoubleC();
            //type.IsPrimitive
            val.Value = reader.ReadDouble();
            return val;
        }

    }

}
