using System.Collections.Generic;
using System.Drawing;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Base interface for composite containers in a hierarchy 
    /// that can contain child items.
    /// </summary>
    public interface IItemContainer
    {
        /// <summary>
        /// Unique identifier for the item or container.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Name of the item or container.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The item icon, if available.
        /// </summary>
        Icon Icon { get; }
        /// <summary>
        /// Optional physical path of the item or container.
        /// </summary>
        string PhysicalPath { get; }
        /// <summary>
        /// Kind of item or container.
        /// </summary>
        ItemKind Kind { get; }
        /// <summary>
        /// Whether the item or container is selected.
        /// </summary>
        bool IsSelected { get; set; }
        /// <summary>
        /// Child items of the item or container.
        /// </summary>
        IEnumerable<IItemContainer> Items { get; }
        /// <summary>
        /// The parent that contains this item or container.
        /// </summary>
        IItemContainer Parent { get; }

        /// <summary>
        /// Makes the element the current selection.
        /// </summary>
        void Select();

        /// <summary>
        /// Attemps to perform a smart cast to the given type 
        /// from the element. This may involve retriving 
        /// underlying implementation objects such as 
        /// a <c>DTE</c> object or an <c>IVsHierarchy</c>.
        /// </summary>
        T As<T>() where T : class;
    }
}