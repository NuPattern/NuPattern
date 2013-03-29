using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Comparers;

namespace NuPattern.Library.ValueComparers
{
    /// <summary>
    /// Compares the already established <see cref="IProductElement.InstanceOrder"/> of two elements.
    /// </summary>
    [DisplayNameResource("PreserveOrderComparer_DisplayName", typeof(Resources))]
    [DescriptionResource("PreserveOrderComparer_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    public class PreserveOrderComparer : ProductElementComparer
    {
        /// <summary>
        /// Compares the <see cref="IProductElement.InstanceOrder"/> of two elements.
        /// </summary>
        /// <param name="x">The first element</param>
        /// <param name="y">The second element</param>
        /// <returns>A value that determines if the first element is less than, greater than or equal to the second element</returns>
        public override int Compare(IProductElement x, IProductElement y)
        {
            if (x.InstanceOrder == y.InstanceOrder) return 0;
            if (x.InstanceOrder < y.InstanceOrder)
                return -1;
            else
                return 1;
        }
    }
}
