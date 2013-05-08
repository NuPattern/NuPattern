using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
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
    [DisplayNameResource(@"AssociateDroppedArtifacts_DisplayName", typeof(Resources))]
    [DescriptionResource(@"AssociateDroppedArtifacts_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class AssociateDroppedArtifacts : Command
    {
        private static readonly ITracer tracer = Tracer.Get<AssociateDroppedArtifacts>();

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
        /// Based on the <see cref="DragArgs"/>, finds the dropped solution 
        /// items in the solution, creates Uris for them using the <see cref="UriService"/> 
        /// and adds those references (if they don't exist already) to the current element.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            var paths = new HashSet<string>(
                ((IEnumerable<string>)new DroppedItemContainerValueProvider { DragArgs = this.DragArgs }.Evaluate())
                .Select(s => s.ToLowerInvariant()));

            if (paths.Count == 0)
                return;

            var existingUris = new HashSet<string>(SolutionArtifactLinkReference
                .GetReferences(this.CurrentElement)
                .Select(u => u.ToString().ToLowerInvariant()));

            var items = this.Solution.Find(i =>
                (i.Kind == ItemKind.Item || i.Kind == ItemKind.Project) &&
                paths.Contains(i.PhysicalPath.ToLowerInvariant()));

            foreach (var item in items)
            {
                var uri = UriService.CreateUri(item);
                if (!existingUris.Contains(uri.ToString().ToLowerInvariant()))
                    ReferenceKindProvider<SolutionArtifactLinkReference, Uri>.AddReference(CurrentElement, uri);
            }
        }
    }
}
