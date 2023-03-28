using FlatSharp.Attributes;
using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Serializer
{
    [ProtoContract]
    [FlatBufferTable]
    [MessagePackObject]
    public class MyClass
    {

        [ProtoMember(1)]
        [FlatBufferItem(1)]
        [Key(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        [FlatBufferItem(2)]
        [Key(2)]
        public string Name { get; set; }


        [ProtoMember(3)]
        [FlatBufferItem(3)]
        [Key(3)]
        public List<Item> ItemList { get; set; }

    }

    [ProtoContract]
    [FlatBufferTable]
    [MessagePackObject]
    public class Item 
    {
        [ProtoMember(1)]
        [FlatBufferItem(1)]
        [Key(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        [FlatBufferItem(2)]
        [Key(2)]
        public int TemplateId { get; set; }

        [ProtoMember(3)]
        [FlatBufferItem(3)]
        [Key(3)]
        public int Val1 { get; set; }

        [ProtoMember(4)]
        [FlatBufferItem(4)]
        [Key(4)]
        public int Val2 { get; set; }
    }
}
