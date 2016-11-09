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
    public StandardInterceptor(T t)
    {
        m_T = t;
    }

    public async void Intercept(IInvocation invocation)
    {

        SynchronizationContext.SetSynchronizationContext( new SingleThreadSynchronizationContext());
        //todo: fix thread stuff here,...
        try
        {
            await Task.FromResult(0);
            var result = await (Task<object>)invocation.Method.Invoke(m_T, invocation.Arguments);
            invocation.ReturnValue = result;
            invocation.Proceed();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(null);
        }

    }

    private sealed class SingleThreadSynchronizationContext :  SynchronizationContext
    {
        private readonly BlockingCollection<KeyValuePair<SendOrPostCallback,object>> m_queue = new BlockingCollection<KeyValuePair<SendOrPostCallback,object>>();

        public SingleThreadSynchronizationContext()
        {
            Task.Factory.StartNew(RunOnCurrentThread, TaskCreationOptions.LongRunning);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            m_queue.Add(new KeyValuePair<SendOrPostCallback,object>(d, state));
        }
    
        public void RunOnCurrentThread()
        {
            KeyValuePair<SendOrPostCallback, object> workItem;
            while (true)
            {
                while (m_queue.TryTake(out workItem, Timeout.Infinite))
                    workItem.Key(workItem.Value);
            }
        }
    
        public void Complete() { m_queue.CompleteAdding(); }
    }
}