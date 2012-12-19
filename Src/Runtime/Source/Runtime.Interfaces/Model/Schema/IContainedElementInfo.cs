using System;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Interface implemented by elements that are contained in a parent element.
    /// </summary>
    public interface IContainedElementInfo : INamedElementInfo
    {
        /// <summary>
        /// Gets the cardinality of the contained element.
        /// </summary>
        Cardinality Cardinality { get; }

        /// <summary>
        /// Gets the auto create option of the contained element.
        /// </summary>
        bool AutoCreate { get; }

        /// <summary>
        /// Gets the allow add new of the contained element.
        /// </summary>
        bool AllowAddNew { get; }

        /// <summary>
        /// Gets the ordering group for the instances of the contained element.
        /// </summary>
        Int32 OrderGroup { get; }

        /// <summary>
        /// Gets the ordering group comparer for custom ordering.
        /// </summary>
        string OrderGroupComparerTypeName { get; }
    }
}