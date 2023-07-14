using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPerformanceCounter
{
    public class CounterPublisher
    {
        public string Domain { get; private set; }
        public int CounterNum { get; private set; }

        private MappedFileManager MappedFileManager;

  
        public CounterPublisher(string domain, int counterNum)
        {
            this.Domain = domain;
            this.CounterNum = counterNum;
            MappedFileManager = new MappedFileManager();
            MappedFileManager.AllocFile(Domain, CounterNum);
        }


        public void Share(string name, float value)
        {
            MappedFileManager.Share(name, value);
        }

        public void ShareAfterUpdate(string name, float value)
        {
            MappedFileManager.TryParse();

            MappedFileManager.Share(name, value);
        }

    }
}
