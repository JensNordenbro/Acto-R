using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ActoR
{
    sealed class ActorEngine
    {
        private readonly BufferBlock<Func<Task>> m_queue = new BufferBlock<Func<Task>>();

        public ActorEngine(ActorAffinity affintiy)
        {
            if (affintiy == ActorAffinity.LongRunningThread)
            {
                Task.Factory.StartNew(() => StartInvokesOnDedicatedThread(), TaskCreationOptions.LongRunning);
            }
            else
            {
                Task.Factory.StartNew(async () => await StartInvokesOnArbitraryThread());
            }
        }

        public void Post(Func<Task> f)
        {
            m_queue.Post(f);
        }

        public async Task StartInvokesOnArbitraryThread()
        {
            while (true)
            {
                Func<Task> a = await m_queue.ReceiveAsync();
                Task t = a();
                await t.SupressResult();
            }
        }


        public void StartInvokesOnDedicatedThread()
        {
            while (true)
            {
                Func<Task> a = m_queue.Receive();
                Task t = a();
                t.SupressResult().Wait();
            }
        }

    }

        
}
