using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ActoR
{
    static class ConversionService
    {
        public static object ChangeTaskType(Type dest, Task<object> input)
        {
            Type tResult = dest.GetGenericArguments().Single();
            MethodInfo castMethod = typeof(ConversionService).GetMethod(nameof(Cast)).MakeGenericMethod(tResult);
            object castedObject = castMethod.Invoke(null, new object[] { input });
            return castedObject;
        }

        public static Task<TReturnType> Cast<TReturnType>(Task<object> o) =>
            o.ContinueWith(prev => (TReturnType)prev.Result);
    }

}
