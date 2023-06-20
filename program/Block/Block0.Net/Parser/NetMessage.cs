using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Block0.Net.Serialize;

namespace Block0.Net
{
    [Flags]
    public enum HeaderBit:byte
    {
        None = 0,
        IsReply = 0b_1000_0000,
        HasTaskID = 0b_0100_0000,
        HasMethodID = 0b_0010_0000,
    }


    //[header:byte][serviceId:byte][taskID:opt:ushort][methodID:opt:ushort][methodParamBytes:opt:n bytes]
    public class NetMessage
    {
        /// <see cref="HeaderBit"/>
        private byte headerFlag = 0;
        public bool IsReply { get; set; }
        public byte SourceServiceId { get; private set; }
        public byte DestServiceId { get; private set; }
        public ushort MethodCallTaskID { get; private set; } = 0;
        public ushort MethodID { get; private set; } = 0;
        public ArraySegment<byte> MethodParamBytes { get; set; }

        public IPEndPoint ipEndPoint { get; set; }


        public NetMessage() { }

        public NetMessage(byte sourceServiceId, byte destServiceId, ushort methodID = 0, ushort methodCallTaskID = 0, bool isReply = false)
        {
            SourceServiceId = sourceServiceId;
            DestServiceId = destServiceId;
            MethodID = methodID;
            MethodCallTaskID = methodCallTaskID;
            IsReply = isReply;

            if (isReply)
                HeaderTool.Or(ref headerFlag, HeaderBit.IsReply);
            if (methodCallTaskID > 0)
                HeaderTool.Or(ref headerFlag, HeaderBit.HasTaskID);
            if (methodID > 0)
                HeaderTool.Or(ref headerFlag, HeaderBit.HasMethodID);
        }

        public static NetMessage Parse(ArraySegment<byte> buffer)
        {
            var ret = new NetMessage();
            ret.FlatBytesToMessage(buffer);
            return ret;
        }

        public void SetParam(object param)
        {
            if(param != null)
                MethodParamBytes = SerializerStub.Serialize(param);
        }

        private byte[] HeaderToFlatBytes()
        {
            var memoryStream = new MemoryStream(10);
            BinaryWriter writer = new BinaryWriter(memoryStream);

            writer.Write(headerFlag);
            writer.Write(SourceServiceId);
            writer.Write(DestServiceId);
            if(MethodCallTaskID>0)
                writer.Write(MethodCallTaskID);
            if(MethodID>0)
                writer.Write(MethodID);

            return memoryStream.ToArray();
        }

        
        public void FlatBytesToMessage(ArraySegment<byte> buffer)
        {
            var memoryStream = new MemoryStream(buffer.Array, buffer.Offset, buffer.Count);
            BinaryReader reader = new BinaryReader(memoryStream);

            headerFlag = reader.ReadByte();
            SourceServiceId= reader.ReadByte();
            DestServiceId = reader.ReadByte();
            if (HeaderTool.BitSetted(ref headerFlag, HeaderBit.HasTaskID))
                MethodCallTaskID = reader.ReadUInt16();
            if(HeaderTool.BitSetted(ref headerFlag, HeaderBit.HasMethodID))
                MethodID = reader.ReadUInt16();
            if (HeaderTool.BitSetted(ref headerFlag, HeaderBit.IsReply))
                IsReply = true;

            MethodParamBytes = buffer.Slice((int)memoryStream.Position);
        }


        public byte[] GetData()
        {
            var headerBytes = HeaderToFlatBytes();

            var retBytes = new byte[headerBytes.Length + MethodParamBytes.Count];

            Array.Copy(headerBytes,retBytes, headerBytes.Length);

            if(MethodParamBytes.Count > 0)
            {
                Array.Copy(MethodParamBytes.Array, MethodParamBytes.Offset, retBytes, headerBytes.Length, MethodParamBytes.Count);
            }

            return retBytes;
        }

    }

    internal static class HeaderTool
    {

        public static void Or(ref byte header, HeaderBit headerBit)
        {
            header |= (byte)headerBit;
        }

        public static bool BitSetted(ref byte header, HeaderBit headerBit)
        {
            return (header & (byte)headerBit) > 0;
        }
    }
}