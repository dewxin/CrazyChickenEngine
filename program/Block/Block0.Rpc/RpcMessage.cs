using Block0.Rpc.Serialize;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Block.Assorted;

namespace Block0.Rpc
{
    [Flags]
    public enum HeaderBit:byte
    {
        None = 0,
        IsReply = 0b_1000_0000,
        HasTaskID = 0b_0100_0000,
        HasMethodID = 0b_0010_0000,
    }


    //[header:byte][srcAppId:byte][dstAppId:byte][taskID:opt:ushort][methodID:opt:ushort][methodParamBytes:opt:n bytes]
    public class RpcMessage
    {
        /// <see cref="HeaderBit"/>
        private byte headerFlag = 0;
        public bool IsReply { get; set; }
        public byte SourceAppId { get;  set; }
        public byte DestAppId { get;  set; }
        public ushort MethodCallTaskID { get;  set; } = 0;
        public ushort MethodID { get;  set; } = 0;
        public ArraySegment<byte> MethodParamBytes { get; set; }

        public IPEndPoint ipEndPoint { get; set; }


        public RpcMessage() { }

        public RpcMessage(byte sourceAppId, byte destAppId, ushort methodId = 0, ushort methodCallTaskId = 0, bool isReply = false)
        {
            SourceAppId = sourceAppId;
            DestAppId = destAppId;
            MethodID = methodId;
            MethodCallTaskID = methodCallTaskId;
            IsReply = isReply;
            InitHeader();
        }

        public void InitHeader()
        {
            if (IsReply)
                HeaderTool.Or(ref headerFlag, HeaderBit.IsReply);
            if (MethodCallTaskID > 0)
                HeaderTool.Or(ref headerFlag, HeaderBit.HasTaskID);
            if (MethodID > 0)
                HeaderTool.Or(ref headerFlag, HeaderBit.HasMethodID);
        }


        public static RpcMessage Parse(ArraySegment<byte> buffer)
        {
            var ret = new RpcMessage();
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
            writer.Write(SourceAppId);
            writer.Write(DestAppId);
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
            SourceAppId= reader.ReadByte();
            DestAppId = reader.ReadByte();
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