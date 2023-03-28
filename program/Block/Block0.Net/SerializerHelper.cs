using MessagePack;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Block0.Net
{
    public class SerializerHelper
    {

        public static ArraySegment<byte> Serialize(object t)
        {
            byte[] bytes = MessagePackSerializer.Serialize(t.GetType(),t);
            return new ArraySegment<byte>(bytes);
        }

        public static ArraySegment<byte> Serialize(Type type, object obj)
        {
            byte[] bytes = MessagePackSerializer.Serialize(type, obj);
            return new ArraySegment<byte>(bytes);
        }

        public static ArraySegment<byte> SerializeCompress(object t)
        {
            var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
            byte[] array = MessagePackSerializer.Serialize(t.GetType(), lz4Options);
            return new ArraySegment<byte>(array);
        }


        public static object Deserialize(Type type, ReadOnlyMemory<byte> bytes)
        {
            return MessagePackSerializer.Deserialize(type, bytes);
        }

        public static object DeserializeCompress(Type type, ReadOnlyMemory<byte> bytes)
        {
            var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
            return MessagePackSerializer.Deserialize(type, bytes, lz4Options);
        }

    }

}