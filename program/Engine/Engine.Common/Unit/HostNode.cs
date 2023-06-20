using System;
using System.Configuration;
using Block.Assorted;
using Block0.Net;
using Block1.LocatableRPC;
using Block0.Threading.Worker;
using Block.Assorted.Logging;
using System.Collections.Generic;
using Block1.ThreadLog;
using Engine.IService;
using Block0.Net.Serialize;
using AutoSerializer;
using Engine.Serializer;

namespace Engine.Common.Unit
{
    public class HostNode
    {
        protected List<HostService> serviceList = new List<HostService>();
        public GlobalConfig GlobalConfig { get; set; }
        public SocketConfig SocketConfig { get; set; }

        public bool IsGlobalEureka { get; set; }

        public virtual void Init(AllConfig allConfig)
        {
            GlobalConfig = allConfig.GlobalConfig;
            SocketConfig = allConfig.SocketConfig;
            GlobalConfig.Inst = GlobalConfig;
            SocketConfig.Inst = SocketConfig;

            SerializerCenter.Init();
            SerializerStub.Init(new DelegateSeriazlier());
            LogExtension.Instance.EnableAsync();
            ServiceFinder.Init();

            foreach (var service in allConfig.ServiceList)
            {
                AddService(service);
            }

        }

        public void Run()
        {
            WorkerManager.StartWork();
        }

        private void AddService(HostService serviceJob)
        {
            serviceJob.HostNode = this;
            serviceList.Add(serviceJob);

            WorkerJobManager.AddJob(serviceJob);
        }


        public void Stop()
        {
            foreach (var service in serviceList)
            {
                service.Stop();
            }

        }


        public NodeInfo GetNodeInfo()
        {

            return new NodeInfo()
            {
                ServerIP = SocketConfig.IP,
                ServerPort = SocketConfig.Port,
                ServiceInfoList = GetServiceInfoList(),
            };

        }

        public List<ServiceInfo> GetServiceInfoList()
        {
            var ret = new List<ServiceInfo>();

            foreach (var service in serviceList)
            {
                if (service is GameService gameService)
                {
                    var serviceInfo = new ServiceInfo()
                    {
                        ServiceID = gameService.JobID,
                        ServiceType = gameService.ServiceType,
                    };

                    ret.Add(serviceInfo);
                }
            }

            return ret;

        }

    }
}
