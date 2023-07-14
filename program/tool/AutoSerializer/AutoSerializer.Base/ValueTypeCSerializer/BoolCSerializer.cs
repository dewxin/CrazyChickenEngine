// Genereated at time: 28/06/2023 15:12:26

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSerializer
{
    internal class BoolCSerializer : ISerializer
    {

        public void Serialize(BinaryWriter writer, object obj)
        {
            BoolC val = (BoolC)obj;
            //type.IsPrimitive
            writer.Write(val.Value);
        }

        public object Deserialize(BinaryReader reader)
        {
            BoolC val = new BoolC();
            //type.IsPrimitive
            val.Value = reader.ReadBoolean();
            return val;
        }

    }

}
