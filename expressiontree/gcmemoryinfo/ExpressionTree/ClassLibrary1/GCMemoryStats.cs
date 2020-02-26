using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace ClassLibrary1
{
    public class GCMemoryStats
    {
        static readonly ConcurrentDictionary<Type, Func<object, long>[]> getPropertyCache = new ConcurrentDictionary<Type, Func<object, long>[]>();
        public static Func<object> CreateGetGCMemoryInfoDelegateExpression()
        {
            // GOAL: Func<object> func = () => (object)GC.GetMemoryInfo();
            var method = typeof(GC).GetTypeInfo().GetMethod("GetGCMemoryInfo", BindingFlags.Public | BindingFlags.Static);

            // # ToString: 
            // () => Convert(GetGCMemoryInfo(), Object)}
            // # .debugView : 
            // .Lambda #Lambda1<System.Func`1[System.Object]>() {
            // (System.Object).Call System.GC.GetGCMemoryInfo()
            // }
            var lambda = Expression.Lambda<Func<object>>(
                    Expression.Convert(
                            Expression.Call(method)
                        , typeof(object))
                , null);
            return lambda.Compile();
        }

        public static (long highMemoryLoadThresholdBytes, long memoryLoadBytes, long totalAvailableMemoryBytes, long heapSizeBytes, long fragmentedBytes) GetGCMemoryInfoPropertieDelegateExpression(Type type, object instance)
        {
            var delegates = getPropertyCache.GetOrAdd(type, t => GetGCMemoryInfoPropertieDelegates(t));
            return (
                delegates[0].Invoke(instance), 
                delegates[1].Invoke(instance), 
                delegates[2].Invoke(instance), 
                delegates[3].Invoke(instance), 
                delegates[4].Invoke(instance)
            );
        }

        private static Func<object, long>[] GetGCMemoryInfoPropertieDelegates(Type type)
        {
            var highMemoryLoadThresholdBytes = CreateGetDelegate<object, long>(type, "HighMemoryLoadThresholdBytes");
            var memoryLoadBytes = CreateGetDelegate<object, long>(type, "MemoryLoadBytes");
            var totalAvailableMemoryBytes = CreateGetDelegate<object, long>(type, "TotalAvailableMemoryBytes");
            var heapSizeBytes = CreateGetDelegate<object, long>(type, "HeapSizeBytes");
            var fragmentedBytes = CreateGetDelegate<object, long>(type, "FragmentedBytes");

            return new[] {
                highMemoryLoadThresholdBytes,
                memoryLoadBytes,
                totalAvailableMemoryBytes,
                heapSizeBytes,
                fragmentedBytes
            };
        }

        static Func<T, U> CreateGetDelegate<T, U>(Type type, string memberName)
        {
            var target = Expression.Parameter(typeof(T), "target");
            var lambda = Expression.Lambda<Func<T, U>>(
                Expression.Convert(
                    Expression.PropertyOrField(
                        Expression.Convert(
                            target
                            , type)
                        , memberName)
                    , typeof(U))
                , target);

            // trarget => Convert(Convert(trarget, MyClass).MyProperty, Object)
            //lambda.Dump();

            return lambda.Compile();
        }
    }
}
