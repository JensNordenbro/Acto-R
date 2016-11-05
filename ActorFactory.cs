using System;
using Castle.DynamicProxy;

public static class ActorFactory
{
    public static TInterface Create<TInterface, TConcrete>(Func<TConcrete> instanceFactory) where TConcrete : TInterface 
    {
        var generator = new ProxyGenerator();

        TInterface proxy = (TInterface) generator.CreateProxy( 
            typeof(TInterface), new StandardInterceptor(), instanceFactory() );

        return proxy;

    } 


}