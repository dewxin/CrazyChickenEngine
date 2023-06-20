using System;
using System.Collections.Concurrent;
using System.IO;

namespace Block0.Net.Serialize
{
    public class SerializerStub
    {
        public static ISerializerStub SerializerImpl { get; private set; }

        public static void Init(ISerializerStub serializer)
        { 
            SerializerImpl = serializer; 
        }

        public static ArraySegment<byte> Serialize(object t)
        {
            return SerializerImpl.Serialize(t);
            //byte[] bytes = MessagePackSerializer.Serialize(t.GetType(), t);
            //return new ArraySegment<byte>(bytes);
        }


        public static object Deserialize(Type type, ArraySegment<byte> bytes)
        {
            return SerializerImpl.Deserialize(type, bytes);
            //return MessagePackSerializer.Deserialize(type, bytes);
        }

    }

}