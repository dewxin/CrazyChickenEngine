using System;
using System.IO;
using System.Threading;

namespace EasyPerformanceCounter
{

    //TODO 需要一个domain的domain
    public class PerfCounter
    {
        private const string defaultDomain = "Easy.Performance.Counter";
        private const string defaultMutex = "Easy.Performance.Counter.Mutex";


        private static CounterPublisher _DefaultDomainPub;
        public static CounterPublisher DefaultDomainPub
        {
            get
            {
                if (_DefaultDomainPub == null)
                    _DefaultDomainPub = NewPub(defaultDomain);

                return _DefaultDomainPub;
            }
        }

        private static CounterSubscriber _DefaultDomainSub;
        public static CounterSubscriber DefaultDomainSub
        {
            get
            {
                if(_DefaultDomainSub == null)
                    _DefaultDomainSub= NewSub(defaultDomain);

                return _DefaultDomainSub;
            }
        }

        public static CounterPublisher NewPub(string domain)
        {
            if(domain != defaultDomain)
            {
                //TODO 将当前域名 注册到 Easy.Performance.Counter 域名
                Mutex mutex;
                if(!Mutex.TryOpenExisting(defaultMutex, out mutex))
                {
                    mutex = new Mutex(false, defaultMutex);
                }

                mutex.WaitOne();

                DefaultDomainPub.ShareAfterUpdate(domain, 1);

                mutex.ReleaseMutex();

            }

            CounterPublisher counterPublisher = new CounterPublisher(domain, 63);
            return counterPublisher;
        }


        public static CounterSubscriber NewSub(string domain)
        {
            try
            {
                CounterSubscriber counterPublisher = new CounterSubscriber(domain);
                return counterPublisher;
            }
            catch(FileNotFoundException ex)
            {
                throw new ArgumentException($"domain {domain} is not Open");
                return null;
            }
            finally
            {

            }

        }

    }
}