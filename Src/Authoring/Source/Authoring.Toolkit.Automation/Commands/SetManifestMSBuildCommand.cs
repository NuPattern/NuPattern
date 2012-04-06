﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Patterning.Authoring.Authoring;
using Microsoft.VisualStudio.Patterning.Authoring.Automation.Properties;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility.References;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Authoring.Automation.Commands
{
    /// <summary>
    /// Command used to validate the pattern model 
    /// </summary>
    [DisplayNameResource("SetManifestMSBuildCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("SetManifestMSBuildCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class SetManifestMSBuildCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SetManifestMSBuildCommand>();

        private DTE dte;

        [Browsable(false)]
        internal DTE Dte
        {
            get
            {
                return this.dte ?? (this.dte = ServiceProvider.GlobalProvider.GetService<SDTE, DTE>());
            }
        }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        /// <value>The solution.</value>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution { get; set; }

        /// <summary>
        /// Gets or sets the service that resolves templates.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual IFxrUriReferenceService UriService { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        /// <value>The current element.</value>
        [Required]
        [Import(AllowDefault = true)]
        public IPatternToolkitInfo CurrentElement { get; set; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.SetManifestMSBuildCommand_TraceInitial, this.CurrentElement.InstanceName);

            // Find  link to vsixmanifest
            var template = SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement.AsElement(), this.UriService)
                .Where(r => r.Name.Equals("source.extension.tt", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (template != null)
            {
                var manifest = template.Items
                    .Where(i => Path.GetExtension(i.Name).EndsWith("vsixmanifest", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault() as IItem;
                if (manifest != null)
                {
                    manifest.Data.IsToolkitManifest = true;
                }
            }
        }
    }
}