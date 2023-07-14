// See https://aka.ms/new-console-template for more information

using EasyPerformanceCounter;

var subscriber = PerfCounter.DefaultDomainSub;
subscriber.FetchAllCounterNames(out var nameGroup);

List<CounterSubscriber> subList = new List<CounterSubscriber>();
foreach(var name in nameGroup)
{
    Console.WriteLine(name);
    subList.Add(PerfCounter.NewSub(name));
}


while (true)
{
    foreach(var sub in subList)
    {
        Thread.Sleep(1);
        var result = sub.FetchAllCounter(out var counterGroup);
        if (result)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(sub.Domain);
            Console.WriteLine("Counter number is " + counterGroup.Count);
            foreach (var counter in counterGroup)
            {
                Console.WriteLine($"counter: name={counter.Name}, value={counter.Value}");
            }
        }
    }

}
