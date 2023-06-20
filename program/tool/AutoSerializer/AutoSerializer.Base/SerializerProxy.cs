using System;
using System.Collections.Generic;
using System.IO;

namespace AutoSerializer
{
    public class SerializerProxy
    {

        private static Dictionary<Type, ISerializer> type2SerializerDict = new Dictionary<Type, ISerializer>();

        public static void AddSerializer(Type type, ISerializer serializer)
        {
            type2SerializerDict.Add(type, serializer);
        }
        public static ISerializer GetSerializer(Type type)
        {
            if (type2SerializerDict.TryGetValue(type, out var serializer))
            {
                return serializer;
            }

            throw new NotSupportedException("cannot find type serialzier");
        }

        public static byte[] Serialize(Type type, object obj)
        {
            if (type2SerializerDict.TryGetValue(type, out var serializer))
            {
                MemoryStream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream);
                serializer.Serialize(writer, obj);
                return stream.ToArray();
            }

            throw new NotSupportedException("cannot find type serialzier");
        }


        public static object Deserialize(Type type, ArraySegment<byte> buffer)
        {
            if (type2SerializerDict.TryGetValue(type, out var serializer))
            {
                var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count);
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                return serializer.Deserialize(binaryReader);
            }

            throw new NotSupportedException("cannot find type serialzier");
        }


    }
}