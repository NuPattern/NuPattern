using System;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsProjectReferences : HierarchyItem
    {
        public VsProjectReferences(IServiceProvider serviceProvider, IHierarchyNode node)
            : base(serviceProvider, node)
        { }

        public override ItemKind Kind
        {
            get { return ItemKind.ReferencesFolder; }
        }
    }
}
