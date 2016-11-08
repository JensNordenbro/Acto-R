using Castle.DynamicProxy;

class StandardInterceptor<T> : IInterceptor
{
    private readonly T m_T;
    public StandardInterceptor(T t)
    {
        m_T = t;
    }

    public void Intercept(IInvocation invocation)
    {
        //todo: fix thread stuff here,...
        invocation.Proceed();
    }
    /*
    private sealed class SingleThreadSynchronizationContext :  SynchronizationContext
    {
        private readonly BlockingCollection<KeyValuePair<SendOrPostCallback,object>> m_queue = new BlockingCollection<KeyValuePair<SendOrPostCallback,object>>();
    
        public override void Post(SendOrPostCallback d, object state)
        {
            m_queue.Add(new KeyValuePair<SendOrPostCallback,object>(d, state));
        }
    
        public void RunOnCurrentThread()
        {
            KeyValuePair<SendOrPostCallback, object> workItem;
            while(m_queue.TryTake(out workItem, Timeout.Infinite))
                workItem.Key(workItem.Value);
        }
    
        public void Complete() { m_queue.CompleteAdding(); }
    }
    */
}