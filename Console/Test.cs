using System;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApplication
{
    public partial class Program
    {

        public class Test : ITest
        {
            private static void WriteThread(string method, int count)
            {
                Console.WriteLine($"{method} #{count}: from thread {Thread.CurrentThread.ManagedThreadId}");
            }

            int delayWorldCount = 0;
            public async Task DelayWorld()
            {
                delayWorldCount++;
                const int delay = 2000;
                WriteThread($"{nameof(DelayWorld)} for {delay/1000.0} s", delayWorldCount);
                
                await Task.Delay(delay);
            }

            int resultCount = 0;
            public Task<MyResult> GetResult()
            {
                resultCount++;
                WriteThread(nameof(GetResult), resultCount);
                return Task.FromResult(new MyResult());
            }


            int helloCount = 0;
            public async Task<int> Hello()
            {
                helloCount++;
                WriteThread(nameof(Hello), helloCount);
                return await Task.FromResult(12);
            }

            int worldCount = 0;
            public async Task<string> World()
            {
                worldCount++;
                WriteThread(nameof(World), worldCount);
                return await Task.FromResult("World");
            }

        }
    }
}
