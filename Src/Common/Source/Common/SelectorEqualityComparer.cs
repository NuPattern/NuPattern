using System;
using System.Collections.Generic;

namespace NuPattern
{
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
