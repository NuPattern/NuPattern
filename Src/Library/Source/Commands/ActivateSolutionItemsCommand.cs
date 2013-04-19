using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Base command class for activating solution items.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class ActivateSolutionItemsCommand : Command
    {
        private const bool DefaultOpen = false;
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ActivateSolutionItemsCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivateSolutionItemsCommand"/> class.
        /// </summary>
        protected ActivateSolutionItemsCommand()
        {
            this.Open = DefaultOpen;
        }

        /// <summary>
        /// Gets or sets whether the artifact is to be opened, or just selected.
        /// </summary>
        [Required]
        [DefaultValue(DefaultOpen)]
        [DisplayNameResource("ActivateSolutionItemsCommand_Open_DisplayName", typeof(Resources))]
        [DescriptionResource("ActivateSolutionItemsCommand_Open_Description", typeof(Resources))]
        public bool Open { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the URI reference service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IUriReferenceService UriReferenceService { get; set; }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution { get; set; }

        /// <summary>
        /// Executes the activation behavior.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ActivateSolutionItemsCommand_TraceInitial, this.CurrentElement.InstanceName, this.Open);

            var items = GetSolutionItems();
            if (items != null && items.Any())
            {
                ActivateItems(items);
            }
            else
            {
                tracer.TraceWarning(
                    Resources.ActivateSolutionItemsCommand_TraceNoItems, this.CurrentElement.InstanceName);
            }
        }

        /// <summary>
        /// Gets the solution items to activate.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IItemContainer> GetSolutionItems();

        /// <summary>
        /// Activates each of the items in solution explorer
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ActivateItems(IEnumerable<IItemContainer> items)
        {
            Guard.NotNull(() => items, items);

            if (items.Any())
            {
                tracer.TraceVerbose(
                    Resources.ActivateSolutionItemsCommand_TraceOpeningSolutionExplorer);

                // Ensure Solution Explorer is visible (Solution Explorer ensures item is visible)
                this.Solution.ShowSolutionExplorer();

                foreach (var item in items)
                {
                    try
                    {
                        // Open the item
                        if (this.Open)
                        {
                            tracer.TraceInformation(
                                Resources.ActivateSolutionItemsCommand_TraceOpeningItem, this.CurrentElement.InstanceName, item.GetLogicalPath());

                            this.UriReferenceService.Open(item);
                        }
                    }
                    catch (Exception e)
                    {
                        tracer.TraceWarning(
                            Resources.ActivateSolutionItemsCommand_TraceOpenFailed, item.GetLogicalPath(), this.CurrentElement.InstanceName, e.Message);
                    }
                }

                tracer.TraceInformation(
                    Resources.ActivateSolutionItemsCommand_TraceSelectingAllItems, this.CurrentElement.InstanceName);

                // Select items
                this.Solution.SelectItems(items);
            }
        }
    }
}
