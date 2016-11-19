# Acto-R Actors framework 

Actor-R is a simple, very small experimenttal proxy library that is meant to enable actors based programming model while still keeping the simple model of Task-based async await programming. 
It enables you to avoid locking mechanisms while still ensuring only one consuming thread starts method execution code in your actor-targets. 
This is hence a way to avoid the problem of there being no proper reentrent locking mechanism  avaiable for async-await based implementations in .NET Core/.NET. 
Even though only one thread initiates work within your Actor, your implementation may spawn multiple threads and call other async operations. This means scalability is still in your hands.  
It is possible to configure Thread affinity using ActorAffinity.LongRunningThread-option. If this is not necessary, only use ActorAffinity.LongRunningThread. In this way you make sure eother to always use exactly one thread or to just use a thread. 

Basically: 
You define a class Test implementing an interface ITest of Task-typed return values: 

        public interface ITest
        {
            Task<int> GetGlobalSumAsync();
            Task<string> GetReportCVSAsync();
            Task<MyResult> GetOtherResult();
        }
Notice, all return types must be Task<>. 

Use: 
       ITest testActorProxy = ActorFactory.Create<ITest, Test>(() => new Test(), ActorAffinity.LongRunningThread);
       
...to create a proxy where all calls to testActorProxy follows the ThreadAffinity rules

## Roadmap 1.0: 
* Think through API
* Stabilization
* Interface validation
* Code quality
* Unit tests 

## Roadmap 2.0:
* Affintiy groups (many actors, same thread)
* ...
