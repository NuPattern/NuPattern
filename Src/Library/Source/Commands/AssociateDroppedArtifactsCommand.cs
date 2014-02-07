using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Automation;
using NuPattern.Library.Properties;
using NuPattern.Library.ValueProviders;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Based on the <see cref="DragArgs"/>, finds the dropped solution 
    /// items in the solution, creates Uris for them using the <see cref="UriService"/> 
    /// and adds those references (if they don't exist already) to the current element.
    /// </summary>
    [DisplayNameResource(@"AssociateDroppedArtifactsCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"AssociateDroppedArtifactsCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class AssociateDroppedArtifactsCommand : Command
    {
        private static readonly ITracer tracer = Tracer.Get<AssociateDroppedArtifactsCommand>();

        /// <summary>
        /// Gets or sets the drag event argument.
        /// </summary>
        [Required, Import(AllowDefault = true)]
        public DragEventArgs DragArgs { get; set; }

        /// <summary>
        /// Gets or sets the current solution.
        /// </summary>
        [Required, Import(AllowDefault = true)]
        public ISolution Solution { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required, Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the Uri reference service.
        /// </summary>
        [Required, Import(AllowDefault = true)]
        public IUriReferenceService UriService { get; set; }

        /// <summary>
        /// Gets or sets an optional tag on the generated reference for each dropped file
        /// </summary>
        [DisplayNameResource(@"AssociateDroppedArtifactsCommand_Tag_DisplayName", typeof(Resources))]
        [DescriptionResource(@"AssociateDroppedArtifactsCommand_Tag_Description", typeof(Resources))]
        [DefaultValue("")]
        public string Tag { get; set; }

        /// <summary>
        /// Based on the <see cref="DragArgs"/>, finds the dropped solution 
        /// items in the solution, creates Uris for them using the <see cref="UriService"/> 
        /// and adds those references (if they don't exist already) to the current element.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.Info(Resources.AssociateDroppedArtifactsCommand_TraceInitial, this.CurrentElement.InstanceName);

            var paths = new HashSet<string>(
                ((IEnumerable<string>)new DroppedItemContainerValueProvider { DragArgs = this.DragArgs }.Evaluate())
                .Select(s => s.ToLowerInvariant()));

            AssociateDroppedSolutionItems(paths);
        }

        internal void AssociateDroppedSolutionItems(HashSet<string> paths)
        {
            if (paths.Count == 0)
                return;

            var existingUris = new HashSet<string>(SolutionArtifactLinkReference
                .GetReferenceValues(this.CurrentElement)
                .Select(u => u.ToString().ToLowerInvariant()));

            var items = this.Solution.Find(i =>
                (i.Kind == ItemKind.Item || i.Kind == ItemKind.Project) &&
                paths.Contains(i.PhysicalPath.ToLowerInvariant()));

            foreach (var item in items)
            {
                var uri = UriService.CreateUri(item);
                if (!existingUris.Contains(uri.ToString().ToLowerInvariant()))
                {
                    // add new reference with tag
                    ReferenceKindProvider<SolutionArtifactLinkReference, Uri>.AddReference(CurrentElement, uri).Tag =
                        this.Tag ?? string.Empty;
                }
                else
                {
                    // Update tag on reference (add if not already exist)
                    var reference = ReferenceKindProvider<SolutionArtifactLinkReference, Uri>.GetReference(CurrentElement, uri);
                    if (reference != null)
                    {
                        reference.AddTag(this.Tag);
                    }
                }
            }
        }
    }
}
