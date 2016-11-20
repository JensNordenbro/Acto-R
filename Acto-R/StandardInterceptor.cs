using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ActoR
{
    sealed class ActorInterceptor<T> : IInterceptor
    {
        private readonly T m_T;
        private readonly BufferBlock<Func<Task>> m_queue = new BufferBlock<Func<Task>>();

        public ActorInterceptor(T t, ActorAffinity affintiy)
        {
            m_T = t;
            if (affintiy == ActorAffinity.LongRunningThread)
            {
                Task.Factory.StartNew(() => RunOnCurrentThread(), TaskCreationOptions.LongRunning);
            }
            else
            {
                Task.Factory.StartNew(async () => await RunOnCurrentThreadAsync());
            }

        }


        public async Task RunOnCurrentThreadAsync()
        {
            while (true)
            {
                Func<Task> a = await m_queue.ReceiveAsync();
                Task t = a();
                await t.MuteResult();
            }
        }

        
        public void RunOnCurrentThread()
        {
            while (true)
            {
                Func<Task> a = m_queue.Receive();
                Task t = a();
                t.MuteResult().Wait();
            }
        }


        public void Intercept(IInvocation invocation)
        {
            var tcs = new TaskCompletionSource<object>();

            m_queue.Post(() =>
            {
                try
                {
                    var taskReturned = (Task)invocation.Method.Invoke(m_T, invocation.Arguments);

                    taskReturned.ContinueWith(previous =>
                    {
                        dynamic prevFlexible = previous;
                        tcs.SetResult(prevFlexible.Result);
                    });
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                return tcs.Task;
            });

            invocation.ReturnValue = tcs.Task.ChangeTaskType(invocation.Method.ReturnType);
        }
    }
}
