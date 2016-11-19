using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace ActoR
{
    sealed class StandardInterceptor<T> : IInterceptor
    {
        private readonly T m_T;
        private readonly BufferBlock<Action> m_queue = new BufferBlock<Action>();

        public StandardInterceptor(T t, ActorAffinity affintiy)
        {
            m_T = t;
            if (affintiy == ActorAffinity.LongRunningThread)
            {
                Task.Factory.StartNew(() => RunOnCurrentThread(), TaskCreationOptions.LongRunning);
            }
            else
            {
                Task.Factory.StartNew(async () => await RunOnCurrentThreadAsync());
            }

        }


        public async Task RunOnCurrentThreadAsync()
        {
            while (true)
            {
                Action a = await m_queue.ReceiveAsync();
                a();
            }
        }

        public void RunOnCurrentThread()
        {
            while (true)
            {
                Action a = m_queue.Receive();
                a();
            }
        }


        public void Intercept(IInvocation invocation)
        {
            
            var tcs = new TaskCompletionSource<object>();
            var conversion = new ConversionService();

            //todo: remove hardcoded-ness of return type
            m_queue.Post(() =>
            {
                try
                {
                    var taskReturned = (Task)invocation.Method.Invoke(m_T, invocation.Arguments);

                    taskReturned.ContinueWith(previous =>
                    {
                        dynamic prevFlexible = previous;
                        tcs.SetResult(prevFlexible.Result);
                    });
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            Type wantedTask = invocation.Method.ReturnType;


            invocation.ReturnValue = conversion.ChangeTaskType(wantedTask, tcs.Task); 
                //conversion.ChangeTaskType(
                //    wantedTask,
                //    tcs.Task.ContinueWith(
                //        prev => conversion.ChangeReturnType(wantedTask, prev)
                //    )
                //);
        }


        private class ConversionService
        {
            public object ChangeTaskType(Type dest, Task<object> input)
            {
                Type tResult = dest.GetGenericArguments().Single();
                MethodInfo castMethod = this.GetType().GetMethod(nameof(Cast)).MakeGenericMethod(tResult);
                object castedObject = castMethod.Invoke(null, new object[] { input });
                return castedObject;
            }

            public object ChangeReturnType(Type dest, Task<object> input)
            {
                Type tResult = dest.GetGenericArguments().Single();
                MethodInfo setResultMethod = this.GetType().GetMethod(nameof(SetResult)).MakeGenericMethod(tResult);
                object castedObject = setResultMethod.Invoke(null, new object[] { input });
                return castedObject;
            }

            public static Task<TReturnType> Cast<TReturnType>(Task<object> o) 
            {
                return o.ContinueWith(prev => (TReturnType)prev.Result);
            }

            public static Task<TReturnType> SetResult<TReturnType>(Task<object> o)
            {
                TaskCompletionSource<TReturnType> tcs = new TaskCompletionSource<TReturnType>();
                tcs.SetResult((TReturnType)o.Result);
                return tcs.Task;
            }
        }
    }
}