using System;
using System.Threading;
using ActoR;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var affinity = ActorAffinity.LongRunningThread;
            // the two different Actor:s have different threads if running LongRunningThread affinity mode. 
            // However ActorAffinity.ThreadPoolThread may cause same threads to be used throughout the scenario. 
            // The tests are executed in sequence
            TestInterfaceBasedApproach(affinity);
            TestLambdaBasedApproach(affinity);
        }

        private static void TestLambdaBasedApproach(ActorAffinity affinity)
        {
            ActoR.Actor actor = ActorFactory.Create(affinity);
            actor.Do(() => Console.WriteLine($"Just write from thread {Thread.CurrentThread.ManagedThreadId}"));
            int a = 1, b = 6;
            // simulate heavy calculation
            Task<int> calculation = actor.Do(() => {
                Console.WriteLine($"Timeconsuming (3s) calculation on thread {Thread.CurrentThread.ManagedThreadId} will halt other shores to be done!");
                Console.WriteLine("It is expected that the result, followed by other queued work is displayed after 3s...");
                Thread.Sleep(3000);
                return a * b;
            });

            actor.Do(() => Console.WriteLine($"Just write from thread {Thread.CurrentThread.ManagedThreadId}"));
            actor.Do(() => Console.WriteLine($"Just write from thread {Thread.CurrentThread.ManagedThreadId}"));
            actor.Do(() => Console.WriteLine($"Just write from thread {Thread.CurrentThread.ManagedThreadId}"));

           
            int result = calculation.Result;
            Console.WriteLine($"{a}*{b}={result}");
            Console.ReadLine();
        }

        private static void TestInterfaceBasedApproach(ActorAffinity affinity)
        {
            ITest testActorProxy = ActorFactory.Create<ITest, Test>(() => new Test(), affinity);
            int count = 0;
            while (count < 10)
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
