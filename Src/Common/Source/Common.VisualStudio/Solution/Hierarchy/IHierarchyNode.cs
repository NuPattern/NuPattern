using System;
using System.Collections.Generic;
using System.Drawing;

namespace NuPattern.VisualStudio.Solution.Hierarchy
{
    /// <summary>
    /// A visual studio hierarchy node.
    /// </summary>
    [CLSCompliant(false)]
    public interface IHierarchyNode
    {
        /// <summary>
        /// Gets the icon of the node.
        /// </summary>
        Icon Icon { get; }

        /// <summary>
        /// Name of this node
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the Key to index icons in an image collection
        /// </summary>
        string IconKey { get; }

        /// <summary>
        /// Returns true is there is al least one child under this node
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// Gets the children nodes.
        /// </summary>
        IEnumerable<IHierarchyNode> Children { get; }

        /// <summary>
        /// Returns the TypeGUID
        /// </summary>
        Guid TypeGuid { get; }

        /// <summary>
        /// Returns the Project GUID
        /// </summary>
        Guid ProjectGuid { get; }

        /// <summary>
        /// Returns the extensibility object
        /// </summary>
        object ExtObject { get; }

        /// <summary>
        /// Returns true if the current node is the solution root
        /// </summary>
        bool IsSolution { get; }

        /// <summary>
        /// Gets the unique identifier of the node.
        /// 
        /// </summary>
        uint ItemId { get; }

        /// <summary>
        /// Gets the relative path in the solution.
        /// </summary>
        string SolutionRelativeName { get; }

        /// <summary>
        /// Queries the type T to the internal hierarchy object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        T GetObject<T>()
            where T : class;
    }
}