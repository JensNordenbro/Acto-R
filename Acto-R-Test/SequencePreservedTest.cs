using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ActoR;
using System.Collections.Generic;
using System.Threading;

namespace ActoRTest
{
    public class SequencePreservedTest
    {
        [Theory]
        [InlineData(ActorAffinity.LongRunningThread)]
        [InlineData(ActorAffinity.ThreadPoolThread)]
        public void Do(ActorAffinity affinity)
        {

            int []providedSequence = { 1, 7, 8, 6, 6, 5, 67};
            var sequence = new List<int>();
            var r = new Actor(affinity);
            Task.WaitAll( providedSequence.Select(item => r.Do(() => sequence.Add(item))).ToArray());

            Assert.Equal(providedSequence, sequence.ToArray());
            
        }


        [Theory]
        [InlineData(ActorAffinity.LongRunningThread)]
        [InlineData(ActorAffinity.ThreadPoolThread)]
        public void DoRangeWithRandomSleep(ActorAffinity affinity)
        {

            int []providedSequence = Enumerable.Range(0, 100).ToArray();
            var sequence = new List<int>();
            var r = new Actor(affinity);
            Action<int> f = (item) => {
                Thread.Sleep(new Random().Next(100));
                sequence.Add(item);
            };
            Task.WaitAll( providedSequence.Select(item => r.Do(() => f(item))).ToArray());
            Assert.Equal(providedSequence, sequence.ToArray());
            
        }


        [Theory]
        [InlineData(ActorAffinity.LongRunningThread)]
        [InlineData(ActorAffinity.ThreadPoolThread)]
        public void EnsureOrder(ActorAffinity affinity)
        {
            List<int> result = new List<int>();

            int []given = {1,4};
            var r = new Actor(affinity);

            Task t1 =  r.Do(() => {
                Thread.Sleep(3000);
                result.Add(given[0]);
            });
            Task t2 = r.Do(() => {
                Thread.Sleep(1000);
                result.Add(given[1]);
            });
         
            Assert.True(!result.Any()); // since we are pausing all tasks initially we should have 0. This test is however a little insecure

            Task.WaitAll(t1, t2);
            // after ActoR tasks have been executed the order should be ok
            Assert.Equal(given, result.ToArray());
            
        }

    }
}