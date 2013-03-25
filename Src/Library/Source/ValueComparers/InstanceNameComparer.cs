using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Comparers;

namespace NuPattern.Library.Comparers
{
    /// <summary>
    /// Compares the <see cref="IProductElement.InstanceName"/> of two elements alphabetically.
    /// </summary>
    [DisplayNameResource("InstanceNameComparer_DisplayName", typeof(Resources))]
    [DescriptionResource("InstanceNameComparer_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    public class InstanceNameComparer : ProductElementComparer
    {
        /// <summary>
        /// Compares the <see cref="IProductElement.InstanceName"/> of two elements.
        /// </summary>
        /// <param name="x">The first element</param>
        /// <param name="y">The second element</param>
        /// <returns>A value that determines if the first element is less than, greater than or equal to the second element</returns>
        public override int Compare(IProductElement x, IProductElement y)
        {
            return string.Compare(x.InstanceName, y.InstanceName, System.StringComparison.InvariantCulture);
        }
    }
}
