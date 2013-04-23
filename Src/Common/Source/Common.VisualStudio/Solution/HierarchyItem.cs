using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    internal abstract class HierarchyItem : IItemContainer
    {
        // For testing
        protected HierarchyItem() { }

        internal HierarchyItem(IServiceProvider serviceProvider, IHierarchyNode node)
        {
            this.ServiceProvider = serviceProvider;
            this.HierarchyNode = node;
            this.Solution = (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));
        }

        public IHierarchyNode HierarchyNode { get; protected set; }
        public IVsSolution Solution { get; private set; }

        public string Name
        {
            get { return this.HierarchyNode.Name; }
        }

        public System.Drawing.Icon Icon
        {
            get { return this.HierarchyNode.Icon; }
        }

        public virtual string PhysicalPath
        {
            get
            {
                string path = null;
                try
                {
                    // When the item is a reference, references folder or something like that
                    // HierarchyNode.Path fails with a COMException, in those cases we're returning just the
                    // canonical name.
                    if (this.HierarchyNode is HierarchyNode)
                    {
                        path = ((HierarchyNode)HierarchyNode).Path;
                    }
                }
                catch (COMException)
                {
                    VsHierarchy.GetCanonicalName(this.HierarchyNode.ItemId, out path);
                }
                return path;
            }
        }

        public virtual string Id
        {
            get { return null; }
        }

        public abstract ItemKind Kind { get; }

        public bool IsSelected
        {
            get
            {
                var selected = this.GetSelection();

                return selected.Contains(this);
            }
            set
            {
                if (value)
                {
                    VsHelper.Select(this.ServiceProvider, this.HierarchyNode);
                }
            }
        }

        public void Select()
        {
            VsHelper.Select(this.ServiceProvider, this.HierarchyNode);
        }

        public virtual IEnumerable<IItemContainer> Items
        {
            get { return this.HierarchyNode.Children.Select(node => ItemFactory.CreateItem(this.ServiceProvider, node)); }
        }

        public virtual object ExtenderObject
        {
            get { return this.HierarchyNode.ExtObject; }
        }

        public IVsHierarchy VsHierarchy
        {
            get { return this.HierarchyNode.GetObject<IVsHierarchy>(); }
        }

        protected IServiceProvider ServiceProvider { get; private set; }

        public override bool Equals(object obj)
        {
            HierarchyItem item = obj as HierarchyItem;

            return item != null && item.VsHierarchy == this.VsHierarchy && this.HierarchyNode.ItemId == item.HierarchyNode.ItemId;
        }

        public override int GetHashCode()
        {
            var h1 = this.VsHierarchy.GetHashCode();
            var h2 = this.HierarchyNode.ItemId.GetHashCode();

            return (((h1 << 5) + h1) ^ h2);
        }

        public IItemContainer Parent
        {
            get
            {
                if (this.Kind == ItemKind.Solution)
                    return null;
                else if (this.HierarchyNode is HierarchyNode)
                    return ItemFactory.CreateItem(this.ServiceProvider, ((HierarchyNode)HierarchyNode).ParentNode);
                else
                    return null;
            }
        }

        public T As<T>() where T : class
        {
            T result = this.ExtenderObject as T;

            if (result == null)
                result = this.VsHierarchy as T;

            if (result == null)
                result = this.HierarchyNode as T;

            if (result == null)
                result = this as T;

            return result;
        }
    }
}
