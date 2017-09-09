# Acto-R actors framework 

Acto-R is a simple, small, experimental proxy library that is meant to enable actors based programming model while still keeping the simple model of Task-based async-await programming.

It enables you to avoid locking mechanisms while still ensuring only one consuming thread starts method execution code in your actor-targets. Hence this is a way to avoid the problem of there being no proper reentrent locking mechanism available for async-await based implementations in .NET Core/.NET Framework. 
Even though only one thread initiates work within your Actor, your implementation may spawn multiple threads and call other async operations. This means scalability is still in your hands.  

It is possible to configure Thread affinity using ActorAffinity.LongRunningThread-option. If this is not necessary, only use ActorAffinity.ThreadPoolThread. Regardless exactly one call is executing in your actor taget at a time.  

### Howto manage a service implementation by an actor 
You define a class Test implementing an interface ITest of Task-typed return values: 

        public interface ITest
        {
            Task<int> GetGlobalSumAsync();
            Task<string> GetReportCVSAsync();
            Task<MyResult> GetOtherResultAsync();
        }
Notice, all return types in the interface must be Task<>:s. 

Later create an Actor that services the calls to the interface methods: 
 
        ITest testActorProxy = ActorFactory.Create<ITest, Test>(() => new Test(), ActorAffinity.LongRunningThread);

All calls on testActorProxy will be serviced in order, by the same actor, respecting the ActorAffinity-rules stated during construction. 

### Howto manage lambdas by an actor 

        Actor actor = ActorFactory.Create(ActorAffinity.LongRunningThread);
        Task t = actor.Do( () => ... );
        
        Task<int> t2 = actor.Do ( () => { ...; return intValue; } );

The two different lambdas will be serviced in order, by the same actor, respecting the ActorAffinity-rules stated during construction. 


## Working with Acto-R

Developed using Visual Studio Code, .NET Core 2.0, targeting netstandard2.0.


## Roadmap 1.0.0: 

* Correct exeception propagation
* Unit tests 
* Cleanup dependencies
* Create and Push a Nuget 0.1 version [Get the whole chain]
* Think through API
* netstandard2.0

## Roadmap 1.1.0:

* Cancellationtoken propagation
* Proper Interface validation
* Validate claims in Readme
* Code quality (FxCop, de-duplication, ...)

## Roadmap 2.0:

* Affintiy groups (many actors, same thread)
* ...
