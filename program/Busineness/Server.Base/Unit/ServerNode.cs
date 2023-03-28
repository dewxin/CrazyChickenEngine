using System;
using System.Configuration;
using Chunk.ThreadLog;
using Block.Assorted;
using Block0.Net;
using Block1.LocatableRPC;
using Block0.Threading.Worker;
using Microsoft.CodeAnalysis.Operations;
using Chunk.LocatableRPC;

namespace Server.Common.Unit
{
    public class ServerNode 
    {
        protected List<ServerService> serviceList = new List<ServerService>();
        public GlobalConfig GlobalConfig { get; set; }
        public SocketConfig SocketConfig { get; set; }

        public bool IsGlobalEureka { get; set; }

        public void Init(AllConfig allConfig)
        {
            GlobalConfig = allConfig.GlobalConfig;
            SocketConfig = allConfig.SocketConfig;
            GlobalConfig.Inst = GlobalConfig;
            SocketConfig.Inst = SocketConfig;


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

        private void AddService(ServerService serviceJob)
        {
            serviceJob.ServerNode = this;
            serviceList.Add(serviceJob);

            WorkerJobManager.AddJob(serviceJob);
        }


        public void Stop()
        {
            foreach (var server in serviceList)
            {
                server.Stop();
            }

        }

    }
}
