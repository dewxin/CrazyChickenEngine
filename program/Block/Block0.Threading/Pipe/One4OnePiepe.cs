using Block.Assorted.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block0.Threading.Pipe
{
    public class One4OnePiepe<T>
    where T : class
    {
        private T[] array;
        private int head;       // The index from which to dequeue if the queue isn't empty.

        //tail只跟生产者有关
        private int tail;       // The index at which to enqueue if the queue isn't full.
        private volatile int size;       // Number of elements.

        public int Count => size;
        public bool IsEmpty => size == 0;

        public One4OnePiepe():this(1024)
        {
        }

        public One4OnePiepe(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "ArgumentOutOfRange_NeedNonNegNum");
            array = new T[capacity];
        }


        //生产者线程
        public virtual bool TryEnqueue(T item)
        {
            if (size == array.Length)
            {
                //throw new Exception("queue is full");
                //TODO 应该允许扩容，但需要打日志
                return false;
            }

            array[tail] = item;
            MoveNext(ref tail);
            Interlocked.Increment(ref size);
            return true;
        }

        public bool TryPeek(out T result)
        {
            if (size == 0)
            {
                result = default;
                return false;
            }
            result = array[head];
            return true;

        }


        public bool TryDequeue(out T result)
        {
            if (size == 0)
            {
                result = default;
                return false;
            }

            result = array[head];
            //if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            //{
            //    array[head] = default;
            //}
            MoveNext(ref head);
            Interlocked.Decrement(ref size);
            return true;
        }


        // Increments the index wrapping it if necessary.
        private void MoveNext(ref int index)
        {
            // It is tempting to use the remainder operator here but it is actually much slower
            // than a simple comparison and a rarely taken branch.
            // JIT produces better code than with ternary operator ?:
            int tmp = index + 1;
            if (tmp == array.Length)
            {
                tmp = 0;
            }
            index = tmp;
        }

    }
}
