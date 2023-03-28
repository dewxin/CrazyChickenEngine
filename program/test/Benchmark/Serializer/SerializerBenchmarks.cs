using Benchmark.Serializer;
using BenchmarkDotNet.Attributes;
using FlatSharp;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{

    public class SerializerBenchmarks
    {
        MyClass myClass;
        byte[] myBuffer;

        [Params(10,100)]
        public int ItemCount = 10;


        [GlobalSetup]
        public void Setup()
        {
            myBuffer = new byte[1024*1024];
            myClass = new MyClass() { Id = 1, Name = "Player" };


            myClass.ItemList=new List<Item>();
            Random random = new Random();
            for(int i = 0; i < ItemCount; i++)
            {
                myClass.ItemList.Add(new Item() { Id=random.Next(), TemplateId=random.Next(), Val1 = random.Next(), Val2 = random.Next()});
            }

        }

        [Benchmark]
        public byte[] ProtoSerialize()
        {
            MemoryStream ms = new MemoryStream(myBuffer);
            ProtoBuf.Serializer.Serialize(ms, myClass);
            return myBuffer;
        }

        [Benchmark]
        public byte[] FlatsharpSerialize()
        {
            //ERROR FlatBuffer doesn't support List<> type
            FlatBufferSerializer.Default.Serialize(myClass, myBuffer) ;
            return myBuffer;
        }

        [Benchmark]
        public byte[] MessagePackSerialize()
        {
            return MessagePackSerializer.Serialize(myClass);
        }

        [Benchmark]
        public byte[] MessagePackCompressSerialize()
        {
            var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
            return MessagePackSerializer.Serialize(myClass, lz4Options);
        }

    }
}
