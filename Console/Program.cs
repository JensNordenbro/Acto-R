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
            Console.WriteLine($"Main Thread: {Thread.CurrentThread.ManagedThreadId}!");
            ITest testActorProxy = ActorFactory.Create<ITest, Test>(() => new Test(), ActorAffinity.LongRunningThread);
            int j = testActorProxy.Hello().Result;

            while (true)
            {
                new Thread(() =>
                {
                    MyResult result = testActorProxy.GetResult().Result;
                    Console.WriteLine($"Got result: {result.ToString()}");
                }).Start();

                new Thread(() =>
                {
                    int i = testActorProxy.Hello().Result;
                    Thread.Sleep(400);

                }).Start();
                new Thread(() =>
                {
                    string s = testActorProxy.World().Result;
                    Thread.Sleep(400);

                }).Start();
                Thread.Sleep(500);
            }

        }

        public interface ITest
        {

            Task<int> Hello();
            Task<string> World();

            Task<MyResult> GetResult();
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


            public async Task<MyResult> GetResult()
            {
                WriteThread(nameof(GetResult));
                await Task.Delay(2000);
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
