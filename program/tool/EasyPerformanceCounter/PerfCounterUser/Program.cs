// See https://aka.ms/new-console-template for more information

using EasyPerformanceCounter;

var publisher = PerfCounter.NewPub("perfCounter.test.1");
var publisher2 = PerfCounter.NewPub("perfCounter.test.2");

float counter1 = 0f;
while (true)
{
    publisher.Share(nameof(counter1), counter1);
    counter1 += 1.01f;

    Thread.Sleep(100);
}
