using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Block.RPC.Task
{
    public class BaseTask
    {
        //todo 怎么禁止调用  使用Event?
        public Action Action { private get; set; } = delegate { };

        //当前任务完成后 立刻执行continuelist中的任务
        private List<BaseTask> continueTaskList = new List<BaseTask> { };

        public int WaitCount { get; private set; } = 0;
        //当前任务完成后, waitTaskList 中任务的 waitCount -1， 如果为0，则执行那个任务
        private List<BaseTask> waitTaskList = new List<BaseTask>();


        public void ContinueWith(Action action)
        {
            var nextTask = New(action);

            ContinueWith(nextTask);
        }
        public void ContinueWith(BaseTask nextTask)
        {
            continueTaskList.Add(nextTask);
        }

        public void OnFinish()
        {
            this.Action();

            foreach (var task in continueTaskList)
            {
                task.OnFinish();
            }
            continueTaskList = null;

            foreach (var task in waitTaskList)
            {
                task.WaitCount--;
                if (task.WaitCount == 0)
                    task.OnFinish();
            }
            waitTaskList = null;
        }

        public static BaseTask New(Action action)
        {
            var task = new BaseTask();
            task.Action = action;
            return task;
        }

        public void Wait(BaseTask nextTask)
        {
            WaitCount++;
            nextTask.waitTaskList.Add(this);
        }

        public static implicit operator BaseTask(Action action) => New(action);
    }


}
