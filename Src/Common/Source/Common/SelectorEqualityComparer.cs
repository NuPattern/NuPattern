using System;
using System.Collections.Generic;

namespace NuPattern
{
    /// <summary>
    /// Factory class for <see cref="SelectorEqualityComparer{TSource, TResult}"/>.
    /// </summary>
    public static class SelectorEqualityComparer
    {
        /// <summary>
        /// Creates a comparer from the given selector function, typically inferring 
        /// automatically the generic parameters from the received function.
        /// </summary>
        public static SelectorEqualityComparer<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> selector)
        {
            return new SelectorEqualityComparer<TSource, TResult>(selector);
        }
    }

    /// <summary>
    /// Allows comparisons (and Distinct in linq) by using 
    /// a simple selector function that projects a value 
    /// from a source, to use for comparison.
    /// </summary>
    public class SelectorEqualityComparer<TSource, TResult> : IEqualityComparer<TSource>
    {
        Func<TSource, TResult> selector;

        /// <summary>
        /// Creates a new instance of the <see cref="SelectorEqualityComparer{TSource,TResult}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        public SelectorEqualityComparer(Func<TSource, TResult> selector)
        {
            Guard.NotNull(() => selector, selector);

            this.selector = selector;
        }

        /// <summary>
        /// Determines of the two sources are equal
        /// </summary>
        public bool Equals(TSource x, TSource y)
        {
            if (Object.Equals(null, x) || Object.Equals(null, y))
                return false;

            return Object.Equals(selector(x), selector(y));
        }

        /// <summary>
        /// Returns the hash code for the source
        /// </summary>
        public int GetHashCode(TSource obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return selector(obj).GetHashCode();
        }
    }
}
