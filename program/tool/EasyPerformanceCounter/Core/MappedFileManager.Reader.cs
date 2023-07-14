using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPerformanceCounter
{
    internal partial class MappedFileManager
    {
        private long timestamp = 0;

        private int GetVariableCount()
        {
            return ReadInt(0);
        }

        private long GetUpdateTimestamp()
        {
            return ReadLong(headerTimestamp);
        }

        public bool TryParse()
        {
            long newTime = GetUpdateTimestamp();
            if (newTime <= timestamp)
                return false;

            timestamp = newTime;
            int varCount = GetVariableCount();
            FullParse(varCount);
            return true;
        }

        public void FullParse(int varCount)
        {
            name2CounterDict.Clear();
            for (int varIndex = 0; varIndex < varCount; varIndex++)
            {
                int entryPointer = entryRegionPointer + varIndex*entrySize;
                var counter = FetchCounter(entryPointer);
                name2CounterDict.Add(counter.Name, counter);
            }
        }

        public MappedFileCounter FetchCounter(int entryPointer)
        {
            MappedFileCounter mappedFileVariable = new MappedFileCounter();

            mappedFileVariable.EntryPointer = entryPointer;

            mappedFileVariable.Value = ReadFloat(entryPointer);
            mappedFileVariable.StrPointer = ReadInt(entryPointer + 4);

            mappedFileVariable.Name = ReadString(mappedFileVariable.StrPointer);
            return mappedFileVariable;
        }


    }

}
