using System;
using System.Threading.Tasks;
using Xunit;

namespace ActoR
{

    /// <summary>
    /// todo: Actually the exceptions have the wrong types but this will do for now
    /// </summary>
    public class ExceptionTestsOnTypedActorMethodInv
    {

        [Theory]
        [InlineData(ActorAffinity.LongRunningThread)]
        [InlineData(ActorAffinity.ThreadPoolThread)]
        public async void TestTask(ActorAffinity affinity)
        {
            ITypedThrowing actor = ActorFactory.Create<ITypedThrowing, TypedThrowing>(() => new TypedThrowing(), affinity);
            await Assert.ThrowsAsync<System.AggregateException>(async () => await actor.A());

        }

        [Theory]
        [InlineData(ActorAffinity.LongRunningThread)]
        [InlineData(ActorAffinity.ThreadPoolThread)]
        public async void TestTaskOfT(ActorAffinity affinity)
        {
            ITypedThrowing actor = ActorFactory.Create<ITypedThrowing, TypedThrowing>(() => new TypedThrowing(), affinity);
            await Assert.ThrowsAsync<TypedThrowing.BException>(async () => await actor.B());
        }

        public interface ITypedThrowing
        {
            Task<bool> A();
            Task B();
        }

        class TypedThrowing : ITypedThrowing
        {
            internal class AException : Exception {}
            public Task<bool> A()
            {
                throw new Exception(nameof(A));
            }

            internal class BException : Exception {}
            public Task B()
            {
                throw new BException();
            }
        }
    }
}
