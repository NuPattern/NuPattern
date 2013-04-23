using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.VisualStudio.Properties;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    [Export(typeof(ISolution))]
    internal class VsSolution : ISolution
    {
        #region private fields
        internal IVsSolution solution;
        private IServiceProvider serviceProvider;
        #endregion

        [ImportingConstructor]
        public VsSolution(
            [Import(typeof(SVsServiceProvider), AllowDefault = true)]
            IServiceProvider serviceProvider)
        {
            Debug.Assert(serviceProvider != null);
            this.serviceProvider = serviceProvider;
            this.solution = this.serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
            this.Data = new VsGlobalsDynamicProperties(() => ((EnvDTE.Solution)this.ExtenderObject).Globals);
        }

        /// <summary>
        /// Arbitrary data associated with the solution, using dynamic property syntax 
        /// (i.e. solution.Data.MyValue = "Hello").
        /// </summary>
        public dynamic Data { get; private set; }

        public ItemKind Kind
        {
            [DebuggerStepThrough]
            get { return ItemKind.Solution; }
        }

        /// <summary>
        /// Whether the solution is opened. In Visual Studio, there is always a solution object, but it may 
        /// not be open, meaning there's no solution, really.
        /// </summary>
        public bool IsOpen
        {
            get { return ((EnvDTE.Solution)this.ExtenderObject).IsOpen; }
        }

        /// <summary>
        /// Returns the phyicsal path to the solution.
        /// </summary>
        public string PhysicalPath
        {
            get
            {
                object solutionFile;
                if (ErrorHandler.Failed(this.solution.GetProperty((int)__VSPROPID.VSPROPID_SolutionFileName, out solutionFile)))
                {
                    // Calculate file name we'll have eventually.
                    object solutionDirectory;
                    object solutionName;

                    try
                    {
                        ErrorHandler.ThrowOnFailure(this.solution.GetProperty((int)__VSPROPID.VSPROPID_SolutionDirectory, out solutionDirectory));
                        ErrorHandler.ThrowOnFailure(this.solution.GetProperty((int)__VSPROPID.VSPROPID_SolutionBaseName, out solutionName));
                    }
                    catch
                    {
                        return null;
                    }

                    if (solutionDirectory == null)
                        return null;

                    var solutionFileName = Path.Combine((string)solutionDirectory, ((string)solutionName) + ".sln");
                    return solutionFileName;
                }
                else
                {
                    return (string)solutionFile;
                }
            }
        }

        /// <summary>
        /// Creates a solution folder with the given name within the solution.
        /// </summary>
        /// <param name="name">Name of the solution folder to create.</param>
        /// <returns>The created solution folder.</returns>
        public ISolutionFolder CreateSolutionFolder(string name)
        {
            if (this.Items.Where(i => i.Name == name).FirstOrDefault() != null)
                throw new ArgumentException(String.Format(
                    CultureInfo.CurrentCulture, Resources.VsItem_DuplicateItemName, this.Name, name));

            var solution = (Solution2)this.ExtenderObject;

            solution.AddSolutionFolder(name);

            return this.Items
                .OfType<ISolutionFolder>()
                .Where(folder => folder.Name == name)
                .Single();
        }

        /// <summary>
        /// returns the list of all solution folders in this container
        /// </summary>
        public IEnumerable<ISolutionFolder> SolutionFolders
        {
            get { return this.Items.OfType<ISolutionFolder>(); }
        }

        /// <summary>
        /// Returns the Extended Object of the current object.
        /// </summary>
        internal object ExtenderObject
        {
            get
            {
                using (HierarchyNode node = new HierarchyNode(this.solution))
                {
                    return node.ExtObject;
                }
            }
        }

        /// <summary>
        /// Returns child items of the current solutions.
        /// </summary>
        public IEnumerable<IItemContainer> Items
        {
            get
            {
                using (HierarchyNode hierNode = new HierarchyNode(this.solution))
                {
                    return hierNode.Children.Select(node => ItemFactory.CreateItem(this.serviceProvider, node));
                }
            }
        }

        /// <summary>
        /// Unique identifier for the item or container.
        /// </summary>
        public string Id
        {
            [DebuggerStepThrough]
            get { return null; }
        }

        /// <summary>
        /// Name of the item or container.
        /// </summary>
        public string Name
        {
            get
            {
                using (HierarchyNode hierNode = new HierarchyNode(this.solution))
                {
                    return hierNode.Name;
                }
            }
        }

        /// <summary>
        /// The item icon, if available.
        /// </summary>
        public System.Drawing.Icon Icon
        {
            get
            {
                using (HierarchyNode hierNode = new HierarchyNode(this.solution))
                {
                    return hierNode.Icon;
                }
            }
        }

        /// <summary>
        /// Returns whether the item or container is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                var selected = this.GetSelection();

                return selected.Contains(this);
            }
            set
            {
                Select();
            }
        }

        /// <summary>
        /// Returns parent container.
        /// </summary>
        public IItemContainer Parent
        {
            get { return null; }
        }

        /// <summary>
        /// Makes the element the current selection.
        /// </summary>
        public void Select()
        {
            using (HierarchyNode node = new HierarchyNode(this.solution))
            {
                VsHelper.Select(this.serviceProvider, node);
            }
        }

        public void SelectItems(IEnumerable<IItemContainer> items)
        {
            VsHelper.Select(this.serviceProvider, items.Select(item => item.As<IHierarchyNode>()));
        }

        /// <summary>
        /// Attemps to perform a smart cast to the given type 
        /// from the element. This may involve retriving 
        /// underlying implementation objects such as 
        /// a <c>DTE</c> object or an <c>IVsHierarchy</c>.
        /// </summary>
        public T As<T>() where T : class
        {
            T result = null;

            using (HierarchyNode node = new HierarchyNode(this.solution))
            {
                result = node.ExtObject as T;
                if (result == null)
                {
                    result = node.GetObject<IVsHierarchy>() as T;
                }

                if (result == null)
                {
                    result = node as T;
                }
            }

            if (result == null)
            {
                result = this as T;
            }

            return result;
        }


        /// <summary>
        /// Compares two lifeline monikers for object equality.
        /// </summary>
        public static bool operator ==(VsSolution left, VsSolution right)
        {
            if ((object)left == null || (object)right == null)
                return (object)left == null && (object)right == null;
            else
                return left.solution == right.solution;
        }


        /// <summary>
        /// Compares two lifeline monikers for object equality.
        /// </summary>
        public static bool operator !=(VsSolution left, VsSolution right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return this.solution.GetHashCode();
        }

        /// <summary>
        /// Compares this object to other for object equality.
        /// </summary>
        /// <param name="other">Other object</param>
        /// <returns>true if both objects are equal</returns>
        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            VsSolution otherVsSolution = (VsSolution)other;
            return this.GetHashCode() == otherVsSolution.GetHashCode();
        }
    }
}
