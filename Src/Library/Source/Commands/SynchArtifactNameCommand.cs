using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
    /// Auto renames the owner's referenced artifact with the owner name.
    /// </summary>
    [DisplayNameResource(@"SynchArtifactNameCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"SynchArtifactNameCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class SynchArtifactNameCommand : Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SynchArtifactNameCommand>();
        internal const string FilteredReferenceTagValue = "FilteredByTag";

        /// <summary>
        /// Gets or sets the tag for the command.
        /// </summary>
        [DisplayNameResource(@"SynchArtifactNameCommand_ReferenceTag_DisplayName", typeof(Resources))]
        [DescriptionResource(@"SynchArtifactNameCommand_ReferenceTag_Description", typeof(Resources))]
        public string ReferenceTag { get; set; }

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
        /// Gets or sets the service provider.
        /// </summary>
        [Required]
        [Import(typeof(SVsServiceProvider))]
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Executes the rename behavior.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override void Execute()
        {
            try
            {
                this.ValidateObject();

                tracer.TraceInformation(
                    Resources.SynchArtifactNameCommand_TraceInitial, this.CurrentElement.InstanceName, this.ReferenceTag);

                if (!string.IsNullOrEmpty(this.ReferenceTag) && this.ReferenceTag != FilteredReferenceTagValue)
                {
                    UpdateReferenceTagTo1113();
                }

                foreach (var reference in this.CurrentElement.References)
                {
                    if (reference.Kind == typeof(SolutionArtifactLinkReference).ToString())
                    {
                        var referenceTag = GetReference(reference.Tag);
                        if (referenceTag == null)
                            continue;

                        if (referenceTag.SyncNames || this.ReferenceTag == null)
                        {
                            var referenceUri = ReferenceKindProvider<SolutionArtifactLinkReference, Uri>.FromReference(reference);

                            var solutionItem = this.UriReferenceService.TryResolveUri<IItemContainer>(referenceUri);
                            if (solutionItem == null)
                            {
                                tracer.TraceWarning(
                                    Resources.SynchArtifactNameCommand_TraceFailedToResolveReference, referenceUri, this.CurrentElement.InstanceName);
                                continue;
                            }

                            var solutionItemName = Path.GetFileNameWithoutExtension(solutionItem.Name);

                            if (!solutionItemName.Equals(this.CurrentElement.InstanceName, StringComparison.OrdinalIgnoreCase))
                            {
                                var proposedItemName = string.Empty;
                                if (string.IsNullOrEmpty(referenceTag.TargetFileName))
                                {
                                    // TODO: Determine if the 'InstanceName' property was the one changed, if not abort sync

                                    proposedItemName = this.CurrentElement.InstanceName;
                                }
                                else
                                {
                                    // TODO: Determine whether any of the properties referenced in TargetFileName syntax 
                                    // (i.e. {InstanceName} or {PropertyName}) was the property actually changed, and if not, dont sync!

                                    var resolver = new PathResolver(this.CurrentElement, this.UriReferenceService, @"\",
                                        string.IsNullOrEmpty(referenceTag.TargetFileName) ? this.CurrentElement.InstanceName : referenceTag.TargetFileName);
                                    resolver.Resolve();

                                    proposedItemName = Path.GetFileNameWithoutExtension(resolver.FileName);
                                }

                                tracer.TraceInformation(
                                    Resources.SynchArtifactNameCommand_TraceRenameSolutionItem, solutionItem.Name, proposedItemName);

                                var uiService = (IVsUIShell)this.ServiceProvider.GetService(typeof(IVsUIShell));
                                var newItemName = solutionItem.Rename(proposedItemName, true, uiService);

                                tracer.TraceInformation(
                                    Resources.SynchArtifactNameCommand_TraceSolutionItemRenamed, solutionItem.Name, newItemName);
                            }
                            else
                            {
                                tracer.TraceInformation(
                                    Resources.SynchArtifactNameCommand_TraceItemIgnoredSameName, solutionItem.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                tracer.TraceError(
                    Resources.SynchArtifactNameCommand_TraceFailedSync, this.CurrentElement.InstanceName);
                throw;
            }
        }

        private void UpdateReferenceTagTo1113()
        {
            var references = this.CurrentElement.References.Where(r => r.Tag == this.ReferenceTag);
            foreach (var reference in references)
            {
                reference.Tag = BindingSerializer.Serialize(new ReferenceTag { SyncNames = true, TargetFileName = string.Empty });
            }
            this.ReferenceTag = string.Empty;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal static ReferenceTag GetReference(string referenceTag)
        {
            try
            {
                return BindingSerializer.Deserialize<ReferenceTag>(referenceTag);
            }
            catch
            {
                return new ReferenceTag { SyncNames = false, TargetFileName = string.Empty };
            }
        }
    }
}
