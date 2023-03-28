using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block0.Net
{
    internal class OrderSafeWriter: BinaryWriter
    {

        public OrderSafeWriter(Stream output) :base(output)
        {
        }

        public override void Write(short value)
        {
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            base.Write(value);
        }

        public override void Write(ushort value)
        {
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            base.Write(value);
        }

        public override void Write(int value)
        {
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            base.Write(value);
        }

        public override void Write(uint value)
        {
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            base.Write(value);
        }

        public override void Write(long value)
        {
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            base.Write(value);
        }

        public override void Write(ulong value)
        {
            value = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
            base.Write(value);
        }
    }
}
