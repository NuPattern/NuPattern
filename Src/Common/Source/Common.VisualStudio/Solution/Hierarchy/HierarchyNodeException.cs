using System;
using System.Runtime.Serialization;

namespace NuPattern.VisualStudio.Solution.Hierarchy
{
    [Serializable]
    internal class HierarchyNodeException : Exception
    {
        public HierarchyNodeException()
        {
        }

        public HierarchyNodeException(string message)
            : base(message)
        {
        }

        public HierarchyNodeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected HierarchyNodeException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}