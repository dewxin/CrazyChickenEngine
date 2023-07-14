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
        Queue<int> idPool = new Queue<int>();
        // 当前ID索引
        private int idIndex = 0;

        public IDGenerator() { }
        public IDGenerator(int startIndex) { idIndex = startIndex; }


        public static IDGenerator Create(int capacity = 4, int startIndex = 1)
        {
            IDGenerator generator = new IDGenerator();
            generator.idPool = new Queue<int>(capacity);
            generator.idIndex= startIndex;
            return generator;
        }



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

            if (idPool.Count != 0)
            {
                int retId = idPool.Dequeue();
                return retId;
            }

            return idIndex++;
        }

        public void ReturnID(int id)
        {
            idPool.Enqueue(id);
        }

    }
}
