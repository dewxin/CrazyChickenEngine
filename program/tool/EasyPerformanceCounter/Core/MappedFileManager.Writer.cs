using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPerformanceCounter
{
    internal partial class MappedFileManager
    {
        public void Share(string name, float value)
        {
            if (name2CounterDict.ContainsKey(name))
            {
                UpdateOldCounter(name, value);
                return;
            }

            StoreNewCounter(name, value);
        }

        public int NextEntryPointer()
        {
            return entryRegionPointer + name2CounterDict.Count * entrySize;
        }

        public int NextStrPointer()
        {
            return stringRegionPointer + name2CounterDict.Count * strSize;
        }

        public void UpdateOldCounter(string name, float value)
        {
            var counter = name2CounterDict[name];
            if (counter.Value == value)
                return;

            counter.Value = value;
            StoreCounter(counter);
        }

        public void StoreNewCounter(string name, float value)
        {
            int entryPointer = NextEntryPointer();
            int strPointer = NextStrPointer();

            MappedFileCounter mappedFileVariable = new MappedFileCounter
            {
                Name = name,
                Value = value,
                EntryPointer = entryPointer,
                StrPointer = strPointer,
            };
            name2CounterDict.Add(name, mappedFileVariable);

            UpdateVaribaleCount();
            StoreCounter(mappedFileVariable);

        }

        public void StoreCounter(MappedFileCounter variable)
        {
            UpdateTimestamp();

            Write(variable.EntryPointer, variable.Value);
            //写入NamePointer
            Write(variable.EntryPointer + 4, variable.StrPointer);
            Write(variable.StrPointer, variable.Name);
        }


        private void UpdateVaribaleCount()
        {
            int count = name2CounterDict.Count;
            Write(headerCountPointer, count);
        }

        private void UpdateTimestamp()
        {
            long time = DateTime.Now.Ticks;
            Write(headerTimestamp, time);
        }

    }
}
