using System.Collections.Generic;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Defines a picker for items in the runtime model.
    /// </summary>
    public interface IProductPicker
    {
        /// <summary>
        /// The currently selected item.
        /// </summary>
        IInstanceBase SelectedItem
        {
            get;
        }

        /// <summary>
        /// Collection of products to display.
        /// </summary>
        ICollection<IProduct> Products
        {
            get;
        }

        /// <summary>
        /// Whether to show modally or not.
        /// </summary>
        /// <returns></returns>
        bool? ShowModal();
    }
}