using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPerformanceCounter
{
    public class CounterSubscriber
    {
        public string Domain { get; private set; }
        private MappedFileManager MappedFileManager;
        public CounterSubscriber(string domain)
        {
            this.Domain = domain;
            MappedFileManager = new MappedFileManager();
            MappedFileManager.OpenFile(Domain);
        }

        public bool FetchAllCounter(out ICollection<MappedFileCounter> counterCollection)
        {
            var result = MappedFileManager.TryParse();
            counterCollection = MappedFileManager.name2CounterDict.Values;
            return result;
        }

        public bool FetchAllCounterNames(out ICollection<string> nameCollection)
        {
            var result = MappedFileManager.TryParse();

            nameCollection = new List<string>();
            foreach(var counter in MappedFileManager.name2CounterDict.Values)
                nameCollection.Add(counter.Name);
            return result;
        }

    }
}
