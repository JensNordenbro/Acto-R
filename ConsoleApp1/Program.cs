using System;
using System.Threading.Tasks;
using ActoR;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            Task.Run(async () =>
            {
                var actor = ActorFactory.Create(ActorAffinity.LongRunningThread);
                await actor.Do(() => Console.WriteLine($"hello from thread {Thread.CurrentThread.ManagedThreadId}"));
            }).Wait();
            Console.ReadLine();
            
        }
    }
}
