using System;
using System.Threading.Tasks;
using Xunit;

namespace ActoR
{
    public class ExceptionTestsOnActorDo
    {
        [Theory]
        [InlineData(ActorAffinity.LongRunningThread)]
        [InlineData(ActorAffinity.ThreadPoolThread)]
        public async void GivenThrowsAlwaysExpectExceptionToBubbleThrough(ActorAffinity affinity)
        {
            var a = new Actor(affinity);

            Task delay = Task.Delay(10000);
            Task expected = Assert.ThrowsAsync<Exception>(async () => await a.Do(() => Throw()));


            Task result = await Task.WhenAny(delay, expected);
            Assert.Equal(result, expected);
        }

        private static void Throw()
        {
            throw new Exception("Error inside actor");
        }
    }
}
