using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace ActoR
{
    sealed class StandardInterceptor<T> : IInterceptor
    {
        private readonly T m_T;
        private readonly BufferBlock<Action> m_queue = new BufferBlock<Action>();

        public StandardInterceptor(T t, ActorAffinity affintiy)
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
                Action a = await m_queue.ReceiveAsync();
                a();
            }
        }

        public void RunOnCurrentThread()
        {
            while (true)
            {
                Action a = m_queue.Receive();
                a();
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
            });

            invocation.ReturnValue = ConversionService.ChangeTaskType(invocation.Method.ReturnType, tcs.Task);
        }
    }
}
