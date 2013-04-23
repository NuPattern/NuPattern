using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Presentation;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.Runtime.UI;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Deletes the linked artifacts associated to current element from the solution.
    /// </summary>
    [DisplayNameResource(@"DeleteArtifactsCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"DeleteArtifactsCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class DeleteArtifactsCommand : Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<DeleteArtifactsCommand>();
        private ISolutionSelector selector;

        /// <summary>
        /// Creates a new instance of the <see cref="DeleteArtifactsCommand"/> class.
        /// </summary>
        public DeleteArtifactsCommand()
        {
            this.Action = DeleteAction.DeleteAll;
        }

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
        /// Gets the VS service proider
        /// </summary>
        [Import(typeof(SVsServiceProvider))]
        internal IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        [Required]
        [DesignOnly(true)]
        [DefaultValue(DeleteAction.DeleteAll)]
        [DisplayNameResource(@"DeleteArtifactsCommand_Action_DisplayName", typeof(Resources))]
        [DescriptionResource(@"DeleteArtifactsCommand_Action_Description", typeof(Resources))]
        public DeleteAction Action { get; set; }

        /// <summary>
        /// Gets or sets the solution selector.
        /// </summary>
        [Required]
        [Browsable(false)]
        public ISolutionSelector SolutionSelector
        {
            get
            {
                if (this.selector == null)
                {
                    var componentModel = this.ServiceProvider.GetService<SComponentModel, IComponentModel>();
                    this.selector = componentModel.GetService<Func<ISolutionSelector>>()();
                }

                return this.selector;
            }
            set { this.selector = value; }
        }

        /// <summary>
        /// Executes the activation behavior.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.DeleteArtifactsCommand_TraceInitial, this.CurrentElement.InstanceName, this.Action);

            // Verify whether there are any (valid) artifact links
            var artifactLinks = SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement, this.UriReferenceService);
            if (artifactLinks == null || !artifactLinks.Any())
            {
                tracer.TraceInformation(
                    Resources.DeleteArtifactsCommand_TraceNoLinks, this.CurrentElement.InstanceName);
                return;
            }

            switch (this.Action)
            {
                case DeleteAction.DeleteAll:
                    DeleteSolutionItems(artifactLinks);
                    break;

                case DeleteAction.PromptUser:
                    var selectedLinks = PromptForSolutionItems(artifactLinks);
                    if (!selectedLinks.Any())
                    {
                        tracer.TraceInformation(
                            Resources.DeleteArtifactsCommand_TraceNoLinks, this.CurrentElement.InstanceName);
                        return;
                    }

                    DeleteSolutionItems(selectedLinks);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private static void DeleteSolutionItems(IEnumerable<IItemContainer> artifactLinks)
        {
            // Automatically delete all solution items
            artifactLinks.ForEach(al =>
            {
                var itemPath = al.GetLogicalPath();
                try
                {
                    tracer.TraceInformation(
                        Resources.DeleteArtifactsCommand_TraceDeleteSolutionItem, itemPath);

                    al.Delete();
                }
                catch (Exception ex)
                {
                    tracer.TraceError(
                        Resources.DeleteArtifactsCommand_ErrorDeletingSolutionItem, itemPath, ex.Message);
                }
            });
        }

        private IEnumerable<IItemContainer> PromptForSolutionItems(IEnumerable<IItemContainer> artifactLinks)
        {
            // Show items in dialog
            this.SolutionSelector.Owner = (Application.Current != null) ? Application.Current.MainWindow : null;
            this.SolutionSelector.RootItem = this.Solution;
            this.SolutionSelector.Title = Resources.DeleteArtifactsCommand_SelectorTitle;
            this.SolutionSelector.UserMessage = string.Format(CultureInfo.CurrentCulture, Resources.DeleteArtifactsCommand_SelectorMessage, this.CurrentElement.InstanceName);
            this.SolutionSelector.EmptyItemsMessage = Resources.DeleteArtifactsCommand_SelectorEmptyItems;
            this.SolutionSelector.Filter.MatchItems = artifactLinks;
            this.SolutionSelector.SelectedItems = artifactLinks;
            this.SolutionSelector.ShowAllExpanded = true;

            var result = Enumerable.Empty<IItemContainer>();
            tracer.ShieldUI(
                () =>
                {
                    using (new MouseCursor(Cursors.Arrow))
                    {
                        if (this.SolutionSelector.ShowDialog())
                        {
                            result = this.SolutionSelector.SelectedItems;
                        }
                    }
                },
                Resources.DeleteArtifactsCommand_SelectorFailed);
            return result;
        }
    }
}