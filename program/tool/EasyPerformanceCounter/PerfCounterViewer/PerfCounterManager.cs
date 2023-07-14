﻿using EasyPerformanceCounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfCounterViewer
{
    class PerfCounterManager
    {
        private Dictionary<string, CounterSubscriber> name2CounterSubDict = new Dictionary<string, CounterSubscriber>();

        public bool UpdateDomain()
        {
            var subscriber = PerfCounter.DefaultDomainSub;
            bool result =subscriber.FetchAllCounterNames(out var nameGroup);

            foreach (var name in nameGroup)
            {
                if(!name2CounterSubDict.ContainsKey(name))
                    name2CounterSubDict.Add(name, PerfCounter.NewSub(name));
            }

            return result;
        }

        public List<CounterSubscriber> GetCounterSubscribers()
        {
            return name2CounterSubDict.Values.ToList();
        }
    }
}
