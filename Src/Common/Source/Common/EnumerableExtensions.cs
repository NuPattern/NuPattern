using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NuPattern
{
    /// <summary>
    /// General-purpose extensions over IEnumerable.
    /// </summary>
    [DebuggerStepThrough]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Runs the given action for each element in the source.
        /// </summary>
        public static IEnumerable<T> Add<T>(this IEnumerable<T> source, T item)
        {
            Guard.NotNull(() => source, source);
            Guard.NotNull(() => item, item);

            return source.Concat(new[] { item });
        }

        /// <summary>
        /// Runs the given action for each element in the source.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Guard.NotNull(() => source, source);
            Guard.NotNull(() => action, action);

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Traverse each item in the enumeration and execute the specified function.
        /// </summary>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recurseFunction)
        {
            foreach (T current in source)
            {
                yield return current;
                IEnumerable<T> enumerable = recurseFunction(current);
                if (enumerable != null)
                {
                    foreach (T current2 in enumerable.Traverse(recurseFunction))
                    {
                        yield return current2;
                    }
                }
            }
            yield break;
        }
    }
}