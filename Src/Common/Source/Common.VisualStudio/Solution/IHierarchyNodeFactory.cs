using System;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal interface IHierarchyNodeFactory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        IHierarchyNode CreateFromProjectGuid(Guid projectGuid);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IHierarchyNode GetSelectedProject();
    }

}
