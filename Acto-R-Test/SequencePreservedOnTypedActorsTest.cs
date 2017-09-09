using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ActoR;
using System.Collections.Generic;
using System.Threading;

namespace ActoRTest
{
    public class SequencePreservedOnTypedActorsTest
    {
        [Theory]
        [InlineData(ActorAffinity.LongRunningThread)]
        [InlineData(ActorAffinity.ThreadPoolThread)]
        public async Task EnsureOrder(ActorAffinity affinity)
        {
            int []given = {1,4};
            var actor = ActorFactory.Create<ITypedSequenceTest, TypedSequenceTestImpl>(() => new TypedSequenceTestImpl(), affinity);

            Task t1 = actor.Add(3000, given[0]);
            Task t2 = actor.Add(1000, given[1]);

            await Task.WhenAll(t1, t2);
            int[] result = await actor.GetElements();
            
            // after ActoR tasks have been executed the order should be ok
            Assert.Equal(given, result);
            
        }

        internal class TypedSequenceTestImpl : ITypedSequenceTest
        {
            private List<int> m_Elements = new List<int>();
            async Task ITypedSequenceTest.Add(int sleepTime, int i)
            {
                await Task.Delay(sleepTime);
                m_Elements.Add(i);
            }

            Task<int[]> ITypedSequenceTest.GetElements() => Task.FromResult(m_Elements.ToArray());
        }

        public interface ITypedSequenceTest 
        {
            Task Add(int sleepTime, int i);
            Task<int[]> GetElements();
        }

    }
}