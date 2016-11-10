using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Threading;
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
            var tcs = new TaskCompletionSource<Task<int>>();

            //todo: remove hardcoded-ness of return type
            m_queue.Post(() =>
            {
                try
                {
                    var returnValue = (Task<int>)invocation.Method.Invoke(m_T, invocation.Arguments);
                    tcs.SetResult(returnValue);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            invocation.ReturnValue = tcs.Task.Unwrap();
        }
    }
}