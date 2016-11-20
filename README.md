# Acto-R actors framework 

Actor-R is a simple, small, experimental proxy library that is meant to enable actors based programming model while still keeping the simple model of Task-based async-await programming.

It enables you to avoid locking mechanisms while still ensuring only one consuming thread starts method execution code in your actor-targets. Hence this is a way to avoid the problem of there being no proper reentrent locking mechanism avaiable for async-await based implementations in .NET Core/.NET Framework. 
Even though only one thread initiates work within your Actor, your implementation may spawn multiple threads and call other async operations. This means scalability is still in your hands.  

It is possible to configure Thread affinity using ActorAffinity.LongRunningThread-option. If this is not necessary, only use ActorAffinity.ThreadPoolThread. Regardless exactly one call is executing in your actor taget at a time.  

Basically: 
You define a class Test implementing an interface ITest of Task-typed return values: 

        public interface ITest
        {
            Task<int> GetGlobalSumAsync();
            Task<string> GetReportCVSAsync();
            Task<MyResult> GetOtherResultAsync();
        }
Notice, all return types in the interface must be Task<>:s. 

Use: 
       ITest testActorProxy = ActorFactory.Create<ITest, Test>(() => new Test(), ActorAffinity.LongRunningThread);
       
...to create a proxy where all calls to testActorProxy follows the ThreadAffinity rules

## Roadmap 0.1
* Unit tests 
* Decide target frameworks 
* Cleanup dependencies
* Push Nuget 0.1 version [Get the whole chain]

## Roadmap 1.0: 
* Think through API
* Lampda-based programming instead of interface based programming ?!
* Proper Interface validation
* Validate claims in Readme
* Stabilization
* Code quality

## Roadmap 1.0/2.0:

* Affintiy groups (many actors, same thread)
* ...
