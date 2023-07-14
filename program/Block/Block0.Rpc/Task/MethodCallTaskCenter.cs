using Block.Assorted;
using Block.Assorted.Logging;
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
    //TODO如果是面对客户端，需要一个Client一个TaskCenter，避免一个客户端影响到其他客户端
    public class MethodCallTaskCenter
    {
        public enum CapacityLowSolution
        {
            None = 0,
            ThreadSleep,
        }

        private IDGenerator idGenerator = IDGenerator.Create(capacity:65535, startIndex:1);

        private Dictionary<ushort, MethodCallTask> taskId2TaskDict = new Dictionary<ushort, MethodCallTask>();

        public CapacityLowSolution CapacitySolution { get; set; } = CapacityLowSolution.ThreadSleep;

        public MethodCallTaskCenter()
        {
        }

        public void Add(MethodCallTask methodCallTask)
        {
            methodCallTask.TaskId = idGenerator.GetUShortID();
            var id = methodCallTask.TaskId;

            taskId2TaskDict.Add(id, methodCallTask);
            HandleCapcityShortage();
        }


        private void HandleCapcityShortage()
        {
            if (taskId2TaskDict.Count < (ushort.MaxValue * 2 / 3f))
                return;

            if (CapacitySolution == CapacityLowSolution.ThreadSleep)
            {
                Thread.Sleep(1);
            }
        }


        public bool HasTask(ushort taskId)
        {
            return taskId2TaskDict.ContainsKey(taskId);
        }

        public MethodCallTask RemoveTask(ushort taskId)
        {
            var task = taskId2TaskDict[taskId];
            idGenerator.ReturnID(taskId);
            bool removeResult = taskId2TaskDict.Remove(taskId);
            Debug.Assert(removeResult);
            return task;
        }

        public MethodCallTask GetTask(ushort taskId)
        {
            var task = taskId2TaskDict[taskId];
            return task;
        }


        public bool MethodCallFinished(ushort taskId, object result)
        {
            if (!taskId2TaskDict.ContainsKey(taskId))
            {
                Log.Warn($"cannot find task{taskId}");
                return false;
            }

            var task = RemoveTask(taskId);
            task.Result = result;

            task.OnFinish();
            return true;
        }


    }
}
