using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ActoR
{
    static class TaskExtenstensions
    {
        public static Task MuteResult(this Task t)
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

        private static class Internal
        {
            public static Task<TReturnType> ChangeTaskType<TReturnType>(Task<object> o) =>
                o.ContinueWith(prev => (TReturnType)prev.Result);
        }

    }
}
