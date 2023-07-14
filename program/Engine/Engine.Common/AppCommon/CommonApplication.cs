using Block0.Threading.Worker;
using EasyPerformanceCounter;
using Engine.Common.Unit;
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

        public CommonApplication()
        {
            ApplicationType = ApplicationTypeEnum.Common;
        }

        protected override void OnInitAddOn()
        {
            counterPublisher = PerfCounter.NewPub(HostNode.Name);
            counterPublisher.Share(nameof(WorkerJobManager.ManagedJobCount), WorkerJobManager.ManagedJobCount);
            counterPublisher.Share(nameof(WorkerManager.WorkerCount), WorkerManager.WorkerCount);
        }

        public override void Update()
        {
            //检测出来的数值感觉有问题。。应该是哪里有bug
            counterPublisher.Share(nameof(WorkerManager.WaitWorkerCount), WorkerManager.WaitWorkerCount);

        }

    }
}
