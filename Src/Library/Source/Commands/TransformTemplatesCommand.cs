using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;
using VSLangProj;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Command used to Transform Templates
    /// </summary>
    [DisplayNameResource("TransformTemplatesCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("TransformTemplatesCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class TransformTemplatesCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<TransformTemplatesCommand>();

        private const string CustomToolName = "TextTemplatingFileGenerator";

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
        /// Gets or sets the status bar.
        /// </summary>
        /// <value>The status bar.</value>
        [Required]
        [Import(AllowDefault = true)]
        public IStatusBar StatusBar { get; set; }

        /// <summary>
        /// Gets or sets the service that resolves templates.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual IUriReferenceService UriService { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        /// <value>The current element.</value>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        [DisplayNameResource("TransformTemplatesCommand_TargetPath_DisplayName", typeof(Resources))]
        [DescriptionResource("TransformTemplatesCommand_TargetPath_Description", typeof(Resources))]
        public virtual string TargetPath { get; set; }

        /// <summary>
        /// Transforms the Templates
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            using (tracer.StartActivity(
                Resources.TransformTemplatesCommand_TraceInitial, this.CurrentElement.InstanceName, this.TargetPath))
            {
                foreach (var parentItem in SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement, this.UriService))
                {
                    if (parentItem != null)
                    {
                        var itemAsItem = parentItem as IItem;
                        if (itemAsItem != null)
                        {
                            ProcessItem(itemAsItem);
                        }

                        parentItem.Find<IItem>().ForEach(item => ProcessItem(item));
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ProcessItem(IItem item)
        {
            if (IsValidTarget(item))
            {
                var message = string.Empty;

                if (this.Dte.SourceControl.IsItemUnderSCC(item.PhysicalPath))
                {
                    this.Dte.SourceControl.CheckOutItem(item.PhysicalPath);
                }
                else
                {
                    var attributes = File.GetAttributes(item.PhysicalPath);

                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        tracer.TraceInformation(
                            Resources.TransformTemplatesCommand_TraceReadOnly, item.GetLogicalPath());

                        return;
                    }
                }

                try
                {
                    message = string.Format(CultureInfo.CurrentCulture,
                            Properties.Resources.TransformTemplatesCommand_TraceSucceded, item.GetLogicalPath(), CustomToolName);

                    var vsProjectItem = item.As<ProjectItem>().Object as VSProjectItem;

                    if (vsProjectItem != null)
                    {
                        vsProjectItem.RunCustomTool();
                    }
                }
                catch (Exception e)
                {
                    message = string.Format(CultureInfo.CurrentCulture,
                                Properties.Resources.TransformTemplatesCommand_TraceError, item.GetLogicalPath(), e.Message);
                }

                tracer.TraceInformation(message);
                this.StatusBar.DisplayMessage(message);
            }
        }

        private static bool IsValidTarget(IItem item)
        {
            return item != null &&
                   item.Kind == ItemKind.Item &&
                   !string.IsNullOrEmpty(item.PhysicalPath) &&
                   File.Exists(item.PhysicalPath) &&
                   item.Data.CustomTool.Equals(
                        CustomToolName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
