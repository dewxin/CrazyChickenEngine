using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.Unit.TimerCore
{
    internal class TimerQueue
    {
        private HostApplication application;
        // 优先度是 timer的 dueTimestamp, 
        private PriorityQueue<Timer, long> timerQueue= new PriorityQueue<Timer, long>();
        public bool Empty => timerQueue.Count == 0;

        public TimerQueue(HostApplication application) 
        { 
            this.application = application; 
        }

        public Timer Peek() => timerQueue.Peek(); 
        public Timer Dequeue() => timerQueue.Dequeue();
        public void Enqueue(Timer timer) => timerQueue.Enqueue(timer, timer._dueTicks);

        public void TryFireCallback()
        {
            if (Empty)
                return;

            var timer = Peek();
            if (DateTime.UtcNow.Ticks < timer._dueTicks)
                return;

            Dequeue();
            timer.Elapsed(application);

            if(timer.AutoReset)
            {
                timer.UpdateTick();
                Enqueue(timer);
            }
        }
    }
}
