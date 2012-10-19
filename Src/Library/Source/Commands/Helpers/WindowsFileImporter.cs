using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
    /// <summary>
    /// Performs the importing of the given files from windows explorer.
    /// </summary>
    [CLSCompliant(false)]
    public class WindowsFileImporter : IWindowsFileImporter
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<WindowsFileImporter>();
        private ISolution solution;
        private IFxrUriReferenceService uriService;
        private IProductElement currentElement;
        private string targetPath;

        /// <summary>
        /// Creates a new instance of the <see cref="WindowsFileImporter"/> class.
        /// </summary>
        public WindowsFileImporter(ISolution solution, IFxrUriReferenceService uriService, IProductElement currentElement, string targetPath)
        {
            this.solution = solution;
            this.uriService = uriService;
            this.currentElement = currentElement;
            this.targetPath = targetPath;
        }

        /// <summary>
        /// Gets the added files by the importer.
        /// </summary>
        protected Dictionary<string, string> AddedItems
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the target container created by the importer.
        /// </summary>
        public IItemContainer TargetContainer
        {
            get;
            private set;
        }

        /// <summary>
        /// Starts the import.
        /// </summary>
        public virtual void Initialize()
        {
            // Ensure target folder exists
            EnsureTargetContainer();

            this.AddedItems = new Dictionary<string, string>();
        }

        /// <summary>
        /// Cleansup the import
        /// </summary>
        public virtual void Cleanup()
        {
        }

        /// <summary>
        /// Process each dragged file.
        /// </summary>
        /// <param name="filePath"></param>
        public virtual bool ImportFileToSolution(string filePath)
        {
            // Cache added item
            this.AddedItems.Add(filePath, filePath);

            //Ensure filepath is unique name in the container folder.
            var uniqueName = EnsureItemNameUniqueInTargetContainer(filePath);
            if (!uniqueName.Equals(filePath, StringComparison.OrdinalIgnoreCase))
            {
                tracer.TraceVerbose(
                    Resources.WindowsFileImporter_TraceRenamingAddedFile, filePath, uniqueName);

                // Update added filename
                this.AddedItems[filePath] = uniqueName;
            }

            // Add file to the solution
            var addedFile = this.TargetContainer.Add(filePath, this.AddedItems[filePath], true, false);

            tracer.TraceInformation(
                Resources.WindowsFileImporter_TraceImportComplete, filePath, this.currentElement.InstanceName, addedFile.GetLogicalPath());

            return true;
        }

        /// <summary>
        /// Returns an item in the solution for the given dropped file.
        /// </summary>
        public virtual IItemContainer GetItemInSolution(string filePath)
        {
            //Get previously added item
            var filename = this.AddedItems[filePath];
            if (!string.IsNullOrEmpty(filename))
            {
                // Get the added item in the solution
                var addedItem = this.TargetContainer.Find<IItemContainer>(filename).FirstOrDefault();
                if (addedItem == null)
                {
                    tracer.TraceError(
                        Resources.WindowsFileImporter_TraceAddedItemNotFound, filename, TargetContainer.GetLogicalPath());
                    return null;
                }

                return addedItem;
            }

            return null;
        }

        /// <summary>
        /// Returns a unique name for the given name within the target container.
        /// </summary>
        /// <param name="itemname"></param>
        /// <returns>A unique name for the item.</returns>
        protected string EnsureItemNameUniqueInTargetContainer(string itemname)
        {
            return UniqueNameGenerator.EnsureUnique(itemname,
                newName => !this.TargetContainer.Find<IItemContainer>(newName).Any());
        }

        private void EnsureTargetContainer()
        {
            var resolver = new PathResolver(this.currentElement, this.uriService, this.targetPath);
            resolver.Resolve();
            if (!string.IsNullOrEmpty(resolver.Path))
            {
                this.TargetContainer = this.solution.Find(resolver.Path).FirstOrDefault();
                if (this.TargetContainer == null)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                        Resources.WindowsFileImporter_ErrorTargetContainerNotExist, resolver.Path));
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    Resources.WindowsFileImporter_ErrorTargetPathResolvedFailed, this.targetPath));
            }

            tracer.TraceInformation(
                Resources.WindowsFileImporter_TraceTargetContainer, this.TargetContainer.GetLogicalPath());
        }
    }
}
