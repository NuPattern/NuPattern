using System;
using System.Collections.Generic;
using System.Linq;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Extension to <see cref="INode"/>.
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        /// Finds all the nodes that match the predicate.
        /// </summary>
        /// <typeparam name="T">The types of nodes to search.</typeparam>
        /// <param name="node">The root node where the search begins.</param>
        /// <param name="predicate">The predicate used to match the nodes.</param>
        /// <returns>The collection of nodes that match the predicate.</returns>
        public static IEnumerable<T> Find<T>(this INode node, Func<T, bool> predicate = null) where T : INode
        {
            if (predicate == null)
            {
                return node.Traverse().OfType<T>();
            }

            return node.Traverse().OfType<T>().Where(predicate);
        }

        /// <summary>
        /// Traverses all the descendents of the given node.
        /// </summary>
        public static IEnumerable<INode> Traverse(this INode node)
        {
            return node.Successors.Traverse(n => n.Successors);
        }
    }
}