using Castle.DynamicProxy;

class StandardInterceptor : IInterceptor
{
    public static TInterface Wrap<TInterface>(T objToWrap) where T : TInterface
    {
        ProxyGenerator generator = new ProxyGenerator();

        TInterface proxy = (TInterface) generator.CreateProxy( 
            typeof(TInterface), new StandardInterceptor(), objToWrap );

        return proxy;
    }

    public object Intercept(IInvocation invocation, params object[] args)
    {
        DoSomeWorkBefore(invocation, args);

        object retValue = invocation.Proceed( args );

        DoSomeWorkAfter(invocation, retValue, args);

        return retValue;
    }

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

}