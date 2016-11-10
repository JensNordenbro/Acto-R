using Castle.DynamicProxy;

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

class StandardInterceptor<T> : IInterceptor
{
    private readonly T m_T;
    private SynchronizationContext m_ActiveContext = new SingleThreadSynchronizationContext();

    public StandardInterceptor(T t)
    {
        m_T = t;
        Task.Factory.StartNew(RunOnCurrentThread, TaskCreationOptions.LongRunning);
    }


    public async void RunOnCurrentThread()
    {
        while (true)
        {
            Action a = m_queue.Take();
            a();
        }
    }

    private readonly BlockingCollection<Action> m_queue = new BlockingCollection<Action>();

    public async void Intercept(IInvocation invocation)
    {
        var tcs = new TaskCompletionSource<Task<int>>();

        //todo: remove hardcoded-ness of return type
        m_queue.Add(() => {
            var returnValue = (Task<int>)invocation.Method.Invoke(m_T, invocation.Arguments);
            tcs.SetResult(returnValue);
        });

        invocation.ReturnValue = tcs.Task.Unwrap();
    }
}