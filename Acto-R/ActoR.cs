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
                try
                {
                    actionToRun();
                    tcs.SetResult(1);
                    return Task.CompletedTask;
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                    return tcs.Task;
                }
            });
            return tcs.Task.AsNonReturningTask();
        }

        public Task<T> Do<T>(Func<T> funcToRun)
        {
            var tcs = new TaskCompletionSource<T>();
            m_ActorEngine.Post(() =>
            {
                try
                {
                    T result = funcToRun();
                    tcs.SetResult(result);
                    return tcs.Task;
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                    return tcs.Task;
                }
                
            });
            return tcs.Task;
        }

    }
}
