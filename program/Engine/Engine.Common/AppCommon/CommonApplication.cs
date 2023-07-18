using Block0.Threading.Worker;
using EasyPerformanceCounter;
using Engine.Common.Unit;
using Engine.Common.Unit.TimerCore;
using Engine.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Common.AppCommon
{
    public class CommonApplication : GameApplication
    {
        private CounterPublisher counterPublisher;

        private int TickPriority { get; set; } = 1;
        public override int ExecutePriority => base.ExecutePriority + TickPriority;
        public override float EstimatedTimeCost => 0;

        private int shareCount = 0;
        public CommonApplication()
        {
            ApplicationType = ApplicationTypeEnum.Common;
        }

        protected override void OnInitAddOn()
        {
            counterPublisher = PerfCounter.NewPub(HostNode.Name);
            counterPublisher.Share(nameof(WorkerJobManager.ManagedJobCount), WorkerJobManager.ManagedJobCount);
            counterPublisher.Share(nameof(WorkerManager.WorkerCount), WorkerManager.WorkerCount);

            var timer = new Timer(SyncData, 1000);
            timer.AutoReset = true;
            AddTimer(timer);
        }

        public void SyncData(HostApplication hostApplication)
        {
            var app = hostApplication as CommonApplication;

            shareCount ++;
            //检测出来的数值感觉有问题。。应该是哪里有bug
            app.counterPublisher.Share(nameof(WorkerManager.WaitWorkerCount), WorkerManager.WaitWorkerCount);
            app.counterPublisher.Share(nameof(WorkerJobManager.EstimatedJobCount), WorkerJobManager.EstimatedJobCount);

            app.counterPublisher.Share(nameof(shareCount), shareCount);
        }

    }
}
