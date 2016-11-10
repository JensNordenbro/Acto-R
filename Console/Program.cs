using System;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ITest test = ActorFactory.Create<ITest, Test>(() => new Test());

            while (true)
            {
                new Thread(() =>
                {
                    int i = test.Hello().Result;
                    Thread.Sleep(400);

                }).Start();
                Thread.Sleep(500);
            }

        }


        public interface ITest
        {

            Task<int> Hello();
        }

        public class Test : ITest
        {
            public async Task<int> Hello()
            {
                Console.WriteLine($"Hello from thread {Thread.CurrentThread.ManagedThreadId}");
                return await Task.FromResult(12);
            }

        }
    }
}
