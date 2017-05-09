using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace ActoR
{
    partial class ActorInterceptor<T> : IInterceptor
    {
        private readonly T m_T;
        private readonly ActorEngine m_ActorEngine;

        public ActorInterceptor(T t, ActorAffinity affintiy)
        {
            m_T = t;
            m_ActorEngine = new ActorEngine(affintiy);
        }

        public void Intercept(IInvocation invocation)
        {
            bool containsReturnValue = invocation.Method.ReturnType.IsConstructedGenericType;
            var tcs = new TaskCompletionSource<object>();

            m_ActorEngine.Post(() =>
            {
                try
                {
                    var taskReturned = (Task)invocation.Method.Invoke(m_T, invocation.Arguments);


                    taskReturned.ContinueWith(previous =>
                    {
                        if (containsReturnValue)
                        {
                            dynamic prevFlexible = previous;
                            if (taskReturned.IsFaulted)
                                tcs.SetException(taskReturned.Exception);
                            else if (taskReturned.IsCanceled)
                                tcs.SetCanceled();
                            else if (taskReturned.IsCompleted)
                                tcs.SetResult(prevFlexible.Result);
                            else
                                tcs.SetException(taskReturned.Exception);
                        }
                        else
                        {
                            if (taskReturned.IsFaulted)
                                tcs.SetException(taskReturned.Exception);
                            else if (taskReturned.IsCanceled)
                                tcs.SetCanceled();
                            else if (taskReturned.IsCompleted)
                                tcs.SetResult("No result");
                            else
                                tcs.SetException(taskReturned.Exception);
                        }
                    });
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                return tcs.Task;
            });

            if (containsReturnValue)
                invocation.ReturnValue = tcs.Task.ChangeTaskType(invocation.Method.ReturnType);
            else
                invocation.ReturnValue = tcs.Task.AsNonReturningTask();
        }

        
    }
}
