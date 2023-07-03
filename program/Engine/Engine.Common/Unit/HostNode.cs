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

namespace Engine.Common.Unit
{
    public class HostNode:IDisposable
    {
        protected List<HostApplication> applicationList = new List<HostApplication>();

        public SocketConfig SocketConfig => SocketConfig.Inst;
        public GlobalConfig GlobalConfig => GlobalConfig.Inst;

        public bool IsGlobalEureka { get; set; }

        public virtual void Init(AllConfig allConfig)
        {
            GlobalConfig.Inst = allConfig.GlobalConfig; 
            SocketConfig.Inst = allConfig.SocketConfig; 

            Engine.Serializer.SerializerCenter.Init();
            SerializerStub.Init(new DelegateSeriazlier());
            LogExtension.Instance.EnableAsync();
            ApplicationFinder.Init();

            foreach (var application in allConfig.ApplicationList)
            {
                AddApplication(application);
            }

        }

        public void Run()
        {
            WorkerManager.StartWork();
        }

        private void AddApplication(HostApplication application)
        {
            application.HostNode = this;
            applicationList.Add(application);

            Log.Info($"Application {application.GetType().Name} Added");

            WorkerJobManager.AddJob(application);
        }

        public NodeInfo GetNodeInfo()
        {

            return new NodeInfo()
            {
                ServerIP = SocketConfig.Inst.IP,
                ServerPort = SocketConfig.Inst.Port,
                ApplicationInfoList = GetApplicationInfoList(),
            };

        }

        public List<ApplicationInfo> GetApplicationInfoList()
        {
            var ret = new List<ApplicationInfo>();

            foreach (var app in applicationList)
            {
                if (app is GameApplication gameApp)
                {
                    var serviceInfo = new ApplicationInfo()
                    {
                        AppID = gameApp.JobID,
                        ApplicationType = gameApp.ApplicationType,
                    };

                    ret.Add(serviceInfo);
                }
            }

            return ret;

        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            foreach (var application in applicationList)
            {
                application.Stop();
            }
        }

    }
}
