using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
    /// <summary>
    /// Creates a new instance of a child element for each item.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class CreateElementFromItemCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CreateElementFromItemCommand>();

        /// <summary>
        /// Gets or sets the URI service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IFxrUriReferenceService UriService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the  child element to create for each item.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("CreateElementFromItemCommand_ChildElementName_DisplayName", typeof(Resources))]
        [DescriptionResource("CreateElementFromItemCommand_ChildElementName_Description", typeof(Resources))]
        public virtual string ChildElementName
        {
            get;
            set;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.CreateElementFromItemCommand_TraceInitial, this.CurrentElement.InstanceName, this.ChildElementName);

            var childElementInfo = this.CurrentElement.EnsureChildContainer(this.ChildElementName);

            // Get the items to create
            var items = GetItemIds();
            if (items != null && items.Any())
            {
                tracer.TraceWarning(
                    Resources.CreateElementFromItemCommand_TraceMatches, this.CurrentElement.InstanceName, items.Count());

                items
                    .ForEach(item =>
                    {
                        // Verify the item
                        if (ValidateItem(item))
                        {
                            // Get element instance name
                            var instanceName = GetElementNameFromItem(item);

                            tracer.TraceInformation(
                                Resources.CreateElementFromItemCommand_TraceInstanceCreation, this.CurrentElement.InstanceName, this.ChildElementName, instanceName);

                            // Create a new instance of the named element
                            var childElement = this.CurrentElement.CreateChildElement(childElementInfo, instanceName, false);
                            if (childElement != null)
                            {
                                // Initialize the element
                                InitializeCreatedElement(item, childElement);
                            }

                            tracer.TraceInformation(
                                Resources.CreateElementFromItemCommand_TraceAddedChildElement, this.CurrentElement.InstanceName, this.ChildElementName, instanceName);
                        }
                    });
            }
            else
            {
                tracer.TraceWarning(
                    Resources.CreateElementFromItemCommand_TraceNoDataMatch, this.CurrentElement.InstanceName);
            }
        }

        /// <summary>
        /// Gets the items to process.
        /// </summary>
        /// <returns>Identifiers of items</returns>
        protected abstract IEnumerable<string> GetItemIds();

        /// <summary>
        /// Determines that item is valid prior to creating an element for it.
        /// </summary>
        /// <returns>True if the item is to be processed, false otherwise.</returns>
        protected virtual bool ValidateItem(string itemId)
        {
            return true;
        }

        /// <summary>
        /// Returns the instance name of the element to be created from the given identifer
        /// </summary>
        /// <returns>A name for the new element being created to represent the item.</returns>
        protected abstract string GetElementNameFromItem(string itemId);

        /// <summary>
        /// Initializes the child element after it is created and added to the parent for the given item.
        /// </summary>
        protected virtual void InitializeCreatedElement(string itemId, IAbstractElement childElement)
        {
        }
    }
}
