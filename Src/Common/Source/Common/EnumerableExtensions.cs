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
        /// Traverses the given source, using the specified recurse function and 
        /// stop condition for traversal.
        /// </summary>
        /// <returns>The first item that matches the condition, or <see langword="null"/></returns>
        public static T Traverse<T>(this T source, Func<T, T> recurseFunction, Predicate<T> stopCondition)
        {
            if (source == null)
                return default(T);

            if (stopCondition(source))
                return source;

            return recurseFunction(source).Traverse(recurseFunction, stopCondition);
        }

        /// <summary>
        /// Traverses the given source, using the specified recurse function and 
        /// stop condition for traversal.
        /// </summary>
        /// <returns>The first item that matches the condition, or <see langword="null"/></returns>
        public static T Traverse<T>(this T node, Func<T, IEnumerable<T>> recurseFunction, Predicate<T> stopCondition)
        {
            foreach (var parentNode in recurseFunction(node))
            {
                if (stopCondition(parentNode))
                {
                    return parentNode;
                }
                else
                {
                    return parentNode.Traverse(recurseFunction, stopCondition);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Traverse each item in the enumeration and execute the specified function.
        /// </summary>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recurseFunction)
        {
            foreach (T current in source)
            {
                yield return current;
                var enumerable = recurseFunction(current);
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
        /// Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items that the collection contains.</typeparam>
        /// <param name="source">The collection to add the elements.</param>
        /// <param name="collection">The collection whose elements should be added to the end of the
        /// <see cref="ICollection{T}"/>.</param>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                source.Add(item);
            }
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
    }
}