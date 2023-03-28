using Block.Rpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Block.RPC.Task
{
    public class MethodCallTaskCenter
    {
        public static ThreadLocal<MethodCallTaskCenter> ThreadLocal = new ThreadLocal<MethodCallTaskCenter>();

        private ushort idGenerator = 1;

        private Dictionary<ushort, MethodCallTask> id2TaskDict = new Dictionary<ushort, MethodCallTask>();
        public event Action<MethodCallTask> OnMethodCallAdded = delegate { };


        public MethodCallTaskCenter()
        {
        }

        public void Add(MethodCallTask methodCallTask)
        {
            methodCallTask.TaskId = idGenerator++;
            var id = methodCallTask.TaskId;
            id2TaskDict[id] = methodCallTask;

            OnMethodCallAdded(methodCallTask);
        }

        public bool HasTask(ushort taskId)
        {
            return id2TaskDict.ContainsKey(taskId);
        }

        public MethodCallTask RemoveTask(ushort taskId)
        {
            var task = id2TaskDict[taskId];
            id2TaskDict.Remove(taskId);
            return task;
        }

        public MethodCallTask GetTask(ushort taskId)
        {
            var task = id2TaskDict[taskId];
            return task;
        }


        public bool MethodCallFinished(ushort taskId, object result)
        {
            if (!id2TaskDict.ContainsKey(taskId))
                return false;

            var task = RemoveTask(taskId);
            task.Result = result;

            task.OnFinish();
            return true;
        }


    }
}
