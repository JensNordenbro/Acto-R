using System;
using System.Threading;
using ActoR;

namespace ConsoleApplication
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            ITest testActorProxy = ActorFactory.Create<ITest, Test>(() => new Test(), ActorAffinity.LongRunningThread);
            int j = testActorProxy.Hello().Result;
            int count = 0;
            while (true)
            {
                //Console.WriteLine($"Start iteration {count} from Main Thread: {Thread.CurrentThread.ManagedThreadId}!");
                count++;
                new Thread(() =>
                {
                    MyResult result = testActorProxy.GetResult().Result;
                    Console.WriteLine($"Got result: {result.ToString()}");
                }).Start();

                new Thread(() =>
                {
                    int i = testActorProxy.Hello().Result;

                }).Start();
                new Thread(() =>
                {
                    string s = testActorProxy.World().Result;

                }).Start();

                if ((count % 5) == 0)
                {
                    new Thread(() =>
                    {
                        testActorProxy.DelayWorld().Wait();

                    }).Start();
                }

                Thread.Sleep(500);
            }
            

        }
    }
}
