using System;
using EnvDTE;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal class VsItem : HierarchyItem, IItem
    {
        public VsItem(IServiceProvider serviceProvider, IHierarchyNode node)
            : base(serviceProvider, node)
        {
            this.Data = new VsItemDynamicProperties(node);
        }

        public dynamic Data { get; private set; }

        public override ItemKind Kind
        {
            get { return ItemKind.Item; }
        }

        public override string PhysicalPath
        {
            get
            {
                ProjectItem projectItem = (ProjectItem)ExtenderObject;
                return projectItem.get_FileNames(1);
            }
        }
    }
}
