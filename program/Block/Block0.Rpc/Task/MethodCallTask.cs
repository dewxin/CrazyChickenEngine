using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.RPC.Task
{

    //TODO 拆分出 调用方 MethodCallerTask 和 被调用方 MethodCalleeTask 两个类
    public class MethodCallTask<TResult> : MethodCallTask
        where TResult : class
    {

        public TResult MethodCallResult { get; set; }

        public override object Result { get => MethodCallResult; set { MethodCallResult = (TResult)value; } }

        public static implicit operator MethodCallTask<TResult>(TResult val)
        {
            var ret = new MethodCallTask<TResult>();

            ret.MethodCallResult = val;
            return ret;
        }

        public static MethodCallTask<TResult> Start(ushort methodId)
        {
            var task = new MethodCallTask<TResult>();
            task.MethodId = methodId;
            MethodCallTaskCenter.ThreadLocal.Value.Add(task);
            return task;
        }

    }

    public abstract class MethodCallTask: BaseTask
    {
        public ushort TaskId { get; internal set; } 
        public ushort MethodId { get; internal set; }
        
        public abstract object Result { get; set; }
    }


}
