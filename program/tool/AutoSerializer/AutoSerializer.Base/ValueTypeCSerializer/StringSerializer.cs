// Genereated at time: 28/06/2023 15:12:26

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSerializer
{
    internal class StringSerializer : ISerializer
    {

        public void Serialize(BinaryWriter writer, object obj)
        {
            string val = (string)obj;
            //type.IsPrimitive
            writer.Write(val);
        }

        public object Deserialize(BinaryReader reader)
        {
            //type.IsPrimitive
            string val = reader.ReadString();
            return val;
        }

    }

}
