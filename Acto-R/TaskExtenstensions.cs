using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ActoR
{
    static class TaskExtenstensions
    {
        public static Task SupressResult(this Task t)
        {
            return t.ContinueWith(task => { });
        }

        public static object ChangeTaskType(this Task<object> input, Type destinationTaskType)
        {
            Type resultType = destinationTaskType.GetGenericArguments().Single();
            MethodInfo castMethod = typeof(Internal).GetMethod(nameof(Internal.ChangeTaskType)).MakeGenericMethod(resultType);
            object castedObject = castMethod.Invoke(null, new object[] { input });
            return castedObject;
        }

        public static Task AsNonReturningTask(this Task<object> input)
        {
            return input;
        }

        private static class Internal
        {
            public static Task<TReturnType> ChangeTaskType<TReturnType>(Task<object> o) 
            {
                var tcs = new TaskCompletionSource<TReturnType>();
                o.ContinueWith(previous => {
                    if (previous.IsFaulted)
                        tcs.SetException(previous.Exception.InnerException);
                    else if (previous.IsCanceled)
                        tcs.SetCanceled();
                    else if (previous.IsCompleted)
                        tcs.SetResult((TReturnType)previous.Result);
                    else
                        tcs.SetException(previous.Exception);
                });
                return tcs.Task;
            }
                
        }

    }
}
