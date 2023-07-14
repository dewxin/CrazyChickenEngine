using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EasyPerformanceCounter
{
    //比如共享一个变量， 直接 Share(string name, float obj)就行
    //内部维护一个metaTable， 有几个共享变量了，他们的名称字符串的偏移，他们的存储位置以及类型是什么。

    // 头部 区域 64字节
    //[nVaribale:int][updateTimestamp:long]

    // 入口 区域 一个变量8字节
    //{[VarContent:float] [NamePointer变量名字符串的指针:int]}

    // 字符串区域 一个变量56字节
    //{string:56byte}{string}

    // 字符串为固定长度56字节（可配），如果发生变动，需要原地变更，由于变更不是原子性的，接收方可能暂时看到奇怪的字符串

    public class MappedFileCounter
    {
        public string Name { get; set; }
        public float Value { get; set; }
        public int EntryPointer { get; set; }
        public int StrPointer { get; set; }
    }

    internal partial class MappedFileManager:IDisposable
    {
        internal Dictionary<string, MappedFileCounter> name2CounterDict = new Dictionary<string, MappedFileCounter>();

        internal MemoryMappedFile memoryFile;

        internal MemoryMappedViewStream stream;

        internal BinaryWriter writer;
        internal BinaryReader reader;

        private const int headerCountPointer = 0;
        private const int headerTimestamp = 4;

        private const int headerSize = 64;
        private const int entrySize = 8;
        private const int strSize = 56;
        private int variableSize => entrySize+ strSize;

        private int entryRegionPointer => headerSize;
        private int stringRegionPointer => headerSize + counterNum * entrySize;

        private int counterNum;

        // 63个计数器刚好填满页面4096字节
        public void AllocFile(string fileName, int counterNum)
        {
            this.counterNum = counterNum;

            int capacity = headerSize + variableSize * counterNum;

            memoryFile = MemoryMappedFile.CreateOrOpen(fileName, capacity);
            Init();
        }

        public void OpenFile(string fileName)
        {
            memoryFile = MemoryMappedFile.OpenExisting(fileName);
            Init();
        }

        private void Init()
        {
            stream = memoryFile.CreateViewStream();
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);
            //string AllocPointer, 因为变量的大小和字符串的大小是1:7，所以字符串开始区域是整体大小的1/8
        }


        private int ReadInt(int offset)
        {
            stream.Position = offset;
            return reader.ReadInt32();
        }

        private long ReadLong(int offset)
        {
            stream.Position = offset;
            return reader.ReadInt64();
        }

        private void Write(int offset, int value)
        {
            stream.Position = offset;
            writer.Write(value);
        }

        private float ReadFloat(int offset)
        {
            stream.Position = offset;
            return reader.ReadSingle();
        }

        private void Write(int offset, long value)
        {
            stream.Position = offset;
            writer.Write(value);
        }

        private void Write(int offset, float value)
        {
            stream.Position = offset;
            writer.Write(value);
        }

        private string ReadString(int offset)
        {
            stream.Position = offset;
            return reader.ReadString();
        }

        private void Write(int offset, string value)
        {
            stream.Position = offset;
            writer.Write(value);
        }

        public void Dispose()
        {
            memoryFile?.Dispose();
            stream?.Dispose();
            writer?.Dispose();
            reader?.Dispose();
        }

    }

}
