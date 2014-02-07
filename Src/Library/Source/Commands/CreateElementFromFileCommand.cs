using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Creates a new instance of a child element for each imported file.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class CreateElementFromFileCommand : CreateElementFromItemCommand
    {
        private static readonly ITracer tracer = Tracer.Get<CreateElementFromFileCommand>();
        private const bool DefaultSyncName = true;

        /// <summary>
        /// Creates a new instance fo the <see cref="CreateElementFromFileCommand"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected CreateElementFromFileCommand()
        {
            this.SyncName = DefaultSyncName;
        }

        /// <summary>
        /// Gets or sets whether to sync the name of the added file
        /// </summary>
        [DisplayNameResource(@"CreateElementFromFileCommand_SyncName_DisplayName", typeof(Resources))]
        [DescriptionResource(@"CreateElementFromFileCommand_SyncName_Description", typeof(Resources))]
        [DesignOnly(true)]
        [DefaultValue(DefaultSyncName)]
        public virtual bool SyncName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an optional tag on the generated reference for each file
        /// </summary>
        [DisplayNameResource(@"CreateElementFromFileCommand_Tag_DisplayName", typeof(Resources))]
        [DescriptionResource(@"CreateElementFromFileCommand_Tag_Description", typeof(Resources))]
        [DefaultValue("")]
        public string Tag { get; set; }

        /// <summary>
        /// Gets the identifiers of the items.
        /// </summary>
        /// <returns></returns>
        protected sealed override IEnumerable<string> GetItemIds()
        {
            return GetFilePaths();
        }

        /// <summary>
        /// Gets the paths of files.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected abstract IEnumerable<string> GetFilePaths();

        /// <summary>
        /// Determines that item is valid prior to creating an element for it.
        /// </summary>
        /// <returns>True if the item is to be processed, false otherwise.</returns>
        protected sealed override bool ValidateItem(string itemId)
        {
            return AddFileToSolution(itemId);
        }

        /// <summary>
        /// Determines that file is valid prior to creating an element for it.
        /// </summary>
        /// <returns>True if the file is to be processed, false otherwise.</returns>
        protected abstract bool AddFileToSolution(string filePath);

        /// <summary>
        /// Returns the instance name of the element to be created from the file path
        /// </summary>
        /// <returns>A name for the new element being created to represent the imported file.</returns>
        protected override string GetElementNameFromItem(string itemId)
        {
            return Path.GetFileNameWithoutExtension(itemId);
        }

        /// <summary>
        /// Initializes the child element after it is created and added to the parent for the given item.
        /// </summary>
        protected override void InitializeCreatedElement(string itemId, IAbstractElement childElement)
        {
            Guard.NotNull(() => childElement, childElement);

            // Get the solution item for this imported file.
            var solutionItem = GetItemInSolution(itemId);
            if (solutionItem != null)
            {
                tracer.Info(
                    Resources.CreateElementFromFileCommand_TraceAddingReference, this.CurrentElement.InstanceName, childElement.InstanceName, solutionItem.GetLogicalPath());

                // Create artifact link
                var reference = SolutionArtifactLinkReference.AddReference(childElement, this.UriService.CreateUri(solutionItem));
                reference.AddTag(this.Tag);
                reference.AddTag(BindingSerializer.Serialize(new ReferenceTag
                {
                    SyncNames = this.SyncName
                }));
            }
            else
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    Resources.CreateElementFromFileCommand_ErrorNoSolutionItem, this.CurrentElement.InstanceName, childElement.InstanceName));
            }
        }

        /// <summary>
        /// Returns the item in the solution that represents the dropped and added file to the solution.
        /// </summary>
        protected virtual IItemContainer GetItemInSolution(string filePath)
        {
            // Find the solution item that was originally dragged.
            return Solution.Traverse()
                .Where(item => !String.IsNullOrEmpty(item.PhysicalPath) && filePath.Equals(item.PhysicalPath, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }
    }
}
