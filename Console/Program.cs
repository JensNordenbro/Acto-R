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
            Console.WriteLine($"Hello World from {Thread.CurrentThread.ManagedThreadId}!");
            ITest test = ActorFactory.Create<ITest, Test>(() => new Test(), ActorAffinity.ThreadPoolThread);
            int j = test.Hello().Result;

            while (true)
            {
                new Thread(() =>
                {
                    int i = test.Hello().Result;
                    Thread.Sleep(400);

                }).Start();
                new Thread(() =>
                {
                    int i = test.World().Result;
                    Thread.Sleep(400);

                }).Start();
                Thread.Sleep(500);
            }

        }


        public interface ITest
        {

            Task<int> Hello();
            Task<int> World();
        }

        public class Test : ITest
        {
            public async Task<int> Hello()
            {
                Console.WriteLine($"Hello from thread {Thread.CurrentThread.ManagedThreadId}");
                return await Task.FromResult(12);
            }

            public async Task<int> World()
            {
                Console.WriteLine($"World from thread {Thread.CurrentThread.ManagedThreadId}");
                return await Task.FromResult(12);
            }

        }
    }
}
