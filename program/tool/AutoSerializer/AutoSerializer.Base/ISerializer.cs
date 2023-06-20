using System;
using System.Collections.Concurrent;
using System.IO;

namespace AutoSerializer
{
    public interface ISerializer
    {

        void Serialize(BinaryWriter writer, object obj);

        object Deserialize(BinaryReader reader);

    }

}