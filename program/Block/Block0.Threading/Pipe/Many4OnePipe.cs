using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block0.Threading.Pipe
{
    public class Many4OnePipe<T> : One4OnePiepe<T>
        where T : class
    {
        internal int SenderCount;


        //TODO 这里需要注意如果 已经占有SenderCount的线程处于休眠状态,
        //这里会一直自旋。
        public bool SpinEnqueue(T msg)
        {
            while (Interlocked.CompareExchange(ref SenderCount, 1, 0) != 0)
                ;

            bool ret = base.TryEnqueue(msg);
            Volatile.Write(ref SenderCount, 0);
            return ret;
        }
    }
}
