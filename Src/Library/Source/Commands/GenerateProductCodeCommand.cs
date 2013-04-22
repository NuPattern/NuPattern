using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Automation;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;
using Dsl = Microsoft.VisualStudio.Modeling.Design;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// A command that generates code from text templates.
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("GenerateProductCodeCommand_DisplayName", typeof(Resources))]
    [DescriptionResource("GenerateProductCodeCommand_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    public class GenerateProductCodeCommand : GenerateModelingCodeCommand
    {
        private const bool DefaultSyncName = false;
        private const string DefaultTag = "";
        private const string DefaultTargetFileName = "{InstanceName}";
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<GenerateProductCodeCommand>();

        /// <summary>
        /// Creates a new instance fo the <see cref="GenerateProductCodeCommand"/> class.
        /// </summary>
        public GenerateProductCodeCommand()
        {
            this.SyncName = DefaultSyncName;
            this.Tag = DefaultTag;
        }

        /// <summary>
        /// Hides the base model file which is calculated now.
        /// </summary>
        [Browsable(false)]
        public override string ModelFile
        {
            get { return base.ModelFile; }
            set { base.ModelFile = value; }
        }

        /// <summary>
        /// Hides the base model element which is now imported.
        /// </summary>
        [Browsable(false)]
        public override ModelElement ModelElement
        {
            get { return base.ModelElement; }
            set { base.ModelElement = value; }
        }

        /// <summary>
        /// Gets or sets the name of the target.
        /// </summary>
        [DefaultValue(DefaultTargetFileName)]
        public override string TargetFileName
        {
            get { return base.TargetFileName; }
            set { base.TargetFileName = value; }
        }

        /// <summary>
        /// Gets or sets Synchronization between solution item names and artifact names
        /// </summary>
        [DefaultValue(DefaultSyncName)]
        [DesignOnly(true)]
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/SyncName.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/SyncName.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        public bool SyncName { get; set; }

        /// <summary>
        /// An optional value to atg the generated reference for the generated file.
        /// </summary>
        [Dsl.DisplayNameResource("NuPattern.Library.Automation.TemplateSettings/Tag.DisplayName", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [Dsl.DescriptionResource("NuPattern.Library.Automation.TemplateSettings/Tag.Description", typeof(LibraryDomainModel), "NuPattern.Library.GeneratedCode.DomainModelResx.gen")]
        [DefaultValue(DefaultTag)]
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the current pattern manager.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IPatternManager PatternManager { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the settings that were used to instantiate this command.
        /// </summary>
        [Import(AllowDefault = true)]
        public ICommandSettings Settings { get; set; }

        /// <summary>
        /// Runs the configured template data against the element.
        /// </summary>
        public override void Execute()
        {
            tracer.TraceData(TraceEventType.Verbose, Resources.GenerateProductCodeCommand_StartedEvent, this.CurrentElement);

            // Note: we are NOT doing this.ValidateObject() thing here as we are setting the base class 
            // properties ourselves. This will be a problem if this is automatically validated by the runtime.
            if (this.PatternManager != null)
            {
                if (!this.PatternManager.IsOpen)
                    throw new InvalidOperationException(Resources.GenerateProductCodeCommand_PatternManagerNotOpen);

                this.ModelFile = this.PatternManager.StoreFile;

                tracer.TraceInformation(
                    Resources.GenerateProductCodeCommand_TraceModelFile, this.ModelFile);
            }

            this.ModelElement = this.CurrentElement as ModelElement;

            Guard.NotNull(() => this.Settings, this.Settings);

            IItemContainer existingArtifact = null;
            var originalTargetFilename = this.TargetFileName;

            // If there is an existing artifact that we can resolve, override the configured settings and 
            // reuse that one instead.
            var existingArtifactLink = SolutionArtifactLinkReference
                .GetReferences(this.CurrentElement, r => GetIdFromReferenceTag(r) == this.Settings.Id)
                .FirstOrDefault();
            if (existingArtifactLink != null)
            {
                // Try to locate the existing solution item
                existingArtifact = this.UriService.TryResolveUri<IItemContainer>(existingArtifactLink);
                if (existingArtifact != null)
                {
                    // If the item exists, then we'll override the configured paths to point 
                    // to the located element.
                    this.TargetPath = existingArtifact.Parent.GetLogicalPath();
                    this.TargetFileName = existingArtifact.Name;

                    tracer.TraceVerbose(
                        Resources.GenerateProductCodeCommand_Trace_ExistingArtifactUsed, existingArtifact.GetLogicalPath());
                }
                else
                {
                    // Otherwise, we'll use the configured path and filename.
                    tracer.TraceInformation(Resources.GenerateProductCodeCommand_Trace_ExistingArtifactUriNotFound, existingArtifactLink);
                }
            }

            //Re-evaluate all properties of current element
            RefreshProvidedValues();

            // Generate the file according to current settings and porperty values
            base.Execute();

            //Restore targetfilename
            if (existingArtifactLink != null)
            {
                if (existingArtifact != null)
                {
                    this.TargetFileName = originalTargetFilename;
                }
            }

            // If an item was generated
            if (this.GeneratedItem != null)
            {
                tracer.TraceVerbose(
                    Resources.GenerateProductCodeCommand_Trace_GeneratedArtifact, this.GeneratedItem.GetLogicalPath());

                // Add new artifact link
                if (existingArtifactLink == null)
                {
                    // Add the new link and set the tag to our settings, so that we know 
                    // it's the link generated by this command instance.
                    var newLink = this.UriService.CreateUri(this.GeneratedItem);

                    SolutionArtifactLinkReference
                        .AddReference(this.CurrentElement, newLink)
                        .Tag = BindingSerializer.Serialize(new ReferenceTag
                        {
                            Tag = this.Tag ?? string.Empty,
                            SyncNames = this.SyncName,
                            TargetFileName = this.TargetFileName,
                            Id = this.Settings.Id
                        });

                    tracer.TraceVerbose(
                        Resources.GenerateProductCodeCommand_Trace_NewArtifactLinkAdded, newLink);
                }
                else
                {
                    // Update existing artifact link

                    // If existing artifact was not found (perhaps its now deleted), a new 
                    // link will be generated for the newly added item, so we must update the existing reference.
                    if (existingArtifact == null)
                    {
                        var newLink = this.UriService.CreateUri(this.GeneratedItem);

                        var reference = this.CurrentElement.References.First(r => GetIdFromReferenceTag(r) == Settings.Id);
                        SolutionArtifactLinkReference.SetReference(reference, newLink);

                        tracer.TraceVerbose(
                            Resources.GenerateProductCodeCommand_Trace_UpdatedExistingArtifactLink, newLink);
                    }
                    else
                    {
                        // Existing artifact found
                        if (this.SyncName)
                        {
                            // Must rename the file to the new filename, (paths remains the same)

                            // Recalculate the filename
                            var resolver = new PathResolver(this.ModelElement, this.UriService, this.TargetPath,
                                (!string.IsNullOrEmpty(this.TargetFileName)) ? this.TargetFileName : ((IProductElement)this.ModelElement).InstanceName);
                            resolver.Resolve();

                            var proposedItemName = resolver.FileName;
                            if (this.SanitizeName)
                            {
                                proposedItemName = SanitizeItemName(proposedItemName);
                            }

                            // Rename file if different name now (taking into account whether extension is specified in TargetFileName or not)
                            if (!proposedItemName.Equals(
                                (string.IsNullOrEmpty(Path.GetExtension(proposedItemName)))
                                    ? Path.GetFileNameWithoutExtension(this.GeneratedItem.Name)
                                    : this.GeneratedItem.Name,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                tracer.TraceInformation(
                                    Resources.GenerateProductCodeCommand_TraceRenameSolutionItem, this.GeneratedItem.Name, proposedItemName);

                                var uiService = (IVsUIShell)this.ServiceProvider.GetService(typeof(IVsUIShell));
                                var newItemName = this.GeneratedItem.Rename(proposedItemName, true, uiService);

                                tracer.TraceInformation(
                                    Resources.GenerateProductCodeCommand_TraceSolutionItemRenamed, this.GeneratedItem.Name, newItemName);
                            }
                        }
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static Guid GetIdFromReferenceTag(IReference r)
        {
            try
            {
                return BindingSerializer.Deserialize<ReferenceTag>(r.Tag).Id;
            }
            catch
            {
                Guid result;
                if (Guid.TryParse(r.Tag, out result))
                {
                    // Try to get the legacy settings guid
                    return result;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        /// <summary>
        /// Finds all properties that have value providers, and re-evaluates them so they are 
        /// current when the T4 code runs from the serialized value, if necessary. Derived 
        /// classes can override this behavior and provider smarter or different evaluation 
        /// strategy.
        /// </summary>
        protected virtual void RefreshProvidedValues()
        {
            using (var tx = this.CurrentElement.Root.ProductState.BeginTransaction())
            {
                // Can't save only on changed as we may not have a changed value within 
                // the transaction (in-memory) but the underlying stored file may be outdated anyways :(
                this.CurrentElement.Root.ProductState
                     .FindAll<IProperty>()
                     .Where(prop =>
                         prop.Root == this.CurrentElement.Root &&
                         prop.Info != null &&
                         prop.Info.HasValueProvider())
                     .ForEach(x => x.SaveProvidedValue());

                tx.Commit();
                this.PatternManager.Save();
            }
        }
    }
}
