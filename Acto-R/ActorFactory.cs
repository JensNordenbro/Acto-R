using System;
using Castle.DynamicProxy;

public static class ActorFactory
{
    public static TInterface Create<TInterface, TConcrete>(Func<TConcrete> instanceFactory) 
        where TConcrete : TInterface
        where TInterface : class
    {
        var generator = new ProxyGenerator();

        TInterface proxy = generator.CreateInterfaceProxyWithoutTarget<TInterface>( 
            new StandardInterceptor<TConcrete>(instanceFactory(), ActorAffinity.OneCallSameThread));

        return proxy;

    } 


}