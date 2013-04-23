using System;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsUnknownItem : HierarchyItem
    {
        public VsUnknownItem(IServiceProvider serviceProvider, IHierarchyNode node)
            : base(serviceProvider, node)
        { }

        public override ItemKind Kind
        {
            get { return ItemKind.Unknown; }
        }
    }
}
