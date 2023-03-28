using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block0.Net
{
    internal class OrderSafeReader : BinaryReader
    {
        public OrderSafeReader(Stream input) : base(input)
        {
        }

        public override short ReadInt16()
        {
            var value = base.ReadInt16();
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            return value;
        }

        public override ushort ReadUInt16()
        {
            var value = base.ReadUInt16();
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            return value;
        }

        public override uint ReadUInt32()
        {
            var value = base.ReadUInt32();
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            return value;
        }

        public override int ReadInt32()
        {
            var value = base.ReadInt32();
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            return value;
        }

        public override ulong ReadUInt64()
        {
            var value = base.ReadUInt64();
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            return value;
        }

        public override long ReadInt64()
        {
            var value = base.ReadInt64();
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            return value;
        }
    }
}
