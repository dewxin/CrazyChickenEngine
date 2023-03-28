using Block0.Threading.Worker;
using Chunk.LocatableRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSimulator
{
    internal class ClientNode
    {
        protected List<ClientService> serviceList = new List<ClientService>();


        public void Init()
        {

            ServiceFinder.Init();

            AddService(new ClientService());

        }

        public void Run()
        {
            WorkerManager.StartWork();
        }

        private void AddService(ClientService serviceJob)
        {
            serviceJob.ClientNode = this;
            serviceList.Add(serviceJob);

            WorkerJobManager.AddJob(serviceJob);
        }


        public void Stop()
        {

        }


    }
}
