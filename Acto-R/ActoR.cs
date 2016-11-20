using System;
using System.Threading.Tasks;

namespace ActoR
{
    //todo: needs error handling...
    public sealed class Actor
    {
        private readonly ActorEngine m_ActorEngine;

        public Actor(ActorAffinity affinity)
        {
            m_ActorEngine = new ActorEngine(affinity);
        }

        public Task Do(Action actionToRun)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            m_ActorEngine.Post(() => {
                actionToRun();
                tcs.SetResult(1);
                return Task.CompletedTask;
            });
            return tcs.Task.AsNonReturningTask();
        }

        public Task<T> Do<T>(Func<T> funcToRun)
        {
            var tcs = new TaskCompletionSource<T>();
            m_ActorEngine.Post(() =>
            {
                T result = funcToRun();
                tcs.SetResult(result);
                return Task.FromResult(result);
            });
            return tcs.Task;
        }

    }
}
