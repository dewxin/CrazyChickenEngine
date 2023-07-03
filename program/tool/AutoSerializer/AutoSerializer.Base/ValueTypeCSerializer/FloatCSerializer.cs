// Genereated at time: 28/06/2023 15:12:26

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSerializer
{
    internal class FloatCSerializer : ISerializer
    {

        public void Serialize(BinaryWriter writer, object obj)
        {
            FloatC val = (FloatC)obj;
            //type.IsPrimitive
            writer.Write(val.Value);
        }

        public object Deserialize(BinaryReader reader)
        {
            FloatC val = new FloatC();
            //type.IsPrimitive
            val.Value = reader.ReadSingle();
            return val;
        }

    }

}
