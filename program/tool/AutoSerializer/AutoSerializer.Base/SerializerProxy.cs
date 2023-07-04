using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoSerializer
{
    public class SerializerProxy
    {

        //TODO 需要支持null字段
        private static Dictionary<Type, ISerializer> type2SerializerDict = new Dictionary<Type, ISerializer>();

        static SerializerProxy()
        {
            SerializerProxy.AddSerializer(typeof(ByteC), new ByteCSerializer());
            SerializerProxy.AddSerializer(typeof(Int16C), new Int16CSerializer());
            SerializerProxy.AddSerializer(typeof(Int32C), new Int32CSerializer());
            SerializerProxy.AddSerializer(typeof(Int64C), new Int64CSerializer());
            SerializerProxy.AddSerializer(typeof(string), new StringSerializer());
            SerializerProxy.AddSerializer(typeof(FloatC), new FloatCSerializer());
            SerializerProxy.AddSerializer(typeof(DoubleC), new DoubleCSerializer());

        }


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

            throw new NotSupportedException($"cannot find serialzier for type {type.Name}");
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

            throw new NotSupportedException($"cannot find serialzier for type {type.Name}");
        }


        public static object Deserialize(Type type, ArraySegment<byte> buffer)
        {
            if (type2SerializerDict.TryGetValue(type, out var serializer))
            {
                var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count);
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                return serializer.Deserialize(binaryReader);
            }

            throw new NotSupportedException($"cannot find serialzier for type {type.Name}");
        }

    }
}