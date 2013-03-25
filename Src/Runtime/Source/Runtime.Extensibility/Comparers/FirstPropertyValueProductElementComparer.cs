using System;
using System.Linq;

namespace NuPattern.Runtime.Comparers
{
    /// <summary>
    /// A comparer that compares the string value of the first property of an element. 
    /// </summary>
    public class FirstPropertyValueProductElementComparer : ProductElementComparer
    {
        /// <summary>
        /// Compares the string value of the first property of the two elements for equality.
        /// </summary>
        public override int Compare(IProductElement x, IProductElement y)
        {
            var value1 = string.Empty;
            var value2 = string.Empty;

            var property1 = x.Properties.FirstOrDefault();
            if (property1 != null)
            {
                value1 = property1.Value.ToString();
            }

            var property2 = y.Properties.FirstOrDefault();
            if (property2 != null)
            {
                value2 = property2.Value.ToString();
            }

            return String.Compare(value1, value2, true);
        }
    }
}
