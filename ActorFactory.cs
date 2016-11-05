using Castle.Core;

public static class ActorFactory
{

    public TInterface Create<TInterface>(Func<TConcrete> instanceFactory) where TConcrete : TInterface 
    {
        if(typeof(TInterface).GetMembers().Any(member => member.ReturnType != typeof(Task<>)))
        {
            throw new NotSupportedException("All members in the interface must be functions returning a Task or Task<T>");
        }
        
        return new Wrapper(instanceFactory());

    } 


}