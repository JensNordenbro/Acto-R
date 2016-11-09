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
    }

    public async void Intercept(IInvocation invocation)
    {

        SynchronizationContext.SetSynchronizationContext(m_ActiveContext);
        //todo: fix thread stuff here,...
        try
        {
            // return from synchronization context
            await Task.FromResult(0).ConfigureAwait(true);
            //todo: remove hardcoded-ness
            var result = await (Task<int>)invocation.Method.Invoke(m_T, invocation.Arguments);
            invocation.ReturnValue = Task.FromResult(result);
            //invocation.Proceed();
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