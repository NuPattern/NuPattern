using System;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsProjectReference : HierarchyItem, IItemContainer
    {
        public VsProjectReference(IServiceProvider serviceProvider, IHierarchyNode node)
            : base(serviceProvider, node)
        { }

        public override ItemKind Kind
        {
            get { return ItemKind.Reference; }
        }
    }
}
