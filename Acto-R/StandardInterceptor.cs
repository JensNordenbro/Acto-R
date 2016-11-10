using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

public enum ActorAffinity
{
    OneCall,
    OneCallSameThread
}

class StandardInterceptor<T> : IInterceptor
{
    private readonly T m_T;
    private readonly BlockingCollection<Action> m_queue = new BlockingCollection<Action>();

    public StandardInterceptor(T t, ActorAffinity affintiy)
    {
        m_T = t;
        Task.Factory.StartNew(RunOnCurrentThread, TaskCreationOptions.LongRunning);
    }


    public void RunOnCurrentThread()
    {
        while (true)
        {
            Action a = m_queue.Take();
            a();
        }
    }

    public void Intercept(IInvocation invocation)
    {
        var tcs = new TaskCompletionSource<Task<int>>();

        //todo: remove hardcoded-ness of return type
        m_queue.Add(() => {
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