using System;
using System.Collections.Concurrent;
using System.IO;

namespace Block0.Rpc.Serialize
{
    public interface ISerializerStub
    {

        ArraySegment<byte> Serialize(object t);

        object Deserialize(Type type, ArraySegment<byte> bytes);

    }

}