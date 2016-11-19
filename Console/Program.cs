using System;
using System.Threading.Tasks;
using System.Threading;
using ActoR;

namespace ConsoleApplication
{
    public class Program
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

        public interface ITest
        {
            Task<int> Hello();
            Task<string> World();

            Task<MyResult> GetResult();

            Task<int> DelayWorld();
        }

        public class MyResult
        {
            public int i = new Random().Next();
            public int j = new Random().Next();

            public string text = "info";

            string internalText = "internal_info";

            public override string ToString() => $"(i:{i}, j:{j}, text:{text}, internalText:{internalText})";
        }


        public class Test : ITest
        {

            private static void WriteThread(string method)
            {
                Console.WriteLine($"{method}: from thread {Thread.CurrentThread.ManagedThreadId}");
            }

            public async Task<int> DelayWorld()
            {
                const int delay = 2000;
                WriteThread($"{nameof(DelayWorld)} for {delay/1000.0} s");
                
                await Task.Delay(delay);
                return delay;
                
            }

            public async Task<MyResult> GetResult()
            {
                WriteThread(nameof(GetResult));
                return new MyResult();
            }


            public async Task<int> Hello()
            {
                WriteThread(nameof(Hello));
                return await Task.FromResult(12);
            }

            public async Task<string> World()
            {
                WriteThread(nameof(World));
                return await Task.FromResult("World");
            }

        }
    }
}
