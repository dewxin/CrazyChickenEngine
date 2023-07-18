using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.Unit.TimerCore
{
    public delegate void TimerAppCallback(HostApplication application);

    public class Timer
    {
        //utc now tick
        internal long _dueTicks;
        internal TimerAppCallback _callback;
        public TimerAppCallback Elapsed { get => _callback; set => _callback = value; }

        //millisecond
        public int Interval { get; set; }

        public bool AutoReset { get; set; }

        public Timer(int interval)
        {
            Interval = interval;
            UpdateTick(true);
        }

        public Timer(TimerAppCallback callback, int interval)
        {
            _callback = callback;
            Interval = interval;
            //TODO Stopwatch.GetTimestamp(); 会不会更快？
            UpdateTick(true);
        }

        internal void UpdateTick(bool init = false)
        {
            if(init)
            {
                _dueTicks = DateTime.UtcNow.Ticks + TimeSpan.TicksPerMillisecond * Interval;
            }
            else
            {
                _dueTicks = _dueTicks + TimeSpan.TicksPerMillisecond * Interval;
            }
        }
    }
}
