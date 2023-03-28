using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block.Assorted
{
    public class IDGenerator
    {
        //Id重复利用池
        HashSet<int> idSet = new HashSet<int>();
        // 当前ID索引
        private int idIndex = 0;

        public IDGenerator() { }
        public IDGenerator(int startIndex) { idIndex = startIndex; }

        public byte GetByteID()
        {
            checked
            {
                return (byte) GetIntID();
            }
        }

        public ushort GetUShortID()
        {
            checked
            {
                return (ushort)GetIntID();
            }
        }

        public int GetIntID()
        {

            if (idSet.Count != 0)
            {
                int retId = idSet.First();
                idSet.Remove(retId);
                return retId;
            }

            return idIndex++;
        }

        public void ReleaseID(int id)
        {
            idSet.Add(id);
        }

    }
}
