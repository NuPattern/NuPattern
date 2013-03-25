using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.ComponentModel.Design;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;
using NuPattern.Library.TypeEditors;
using NuPattern.Runtime;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// A command that generates code from text templates.
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("GenerateModelingCodeCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_DslAutomation", typeof(Resources))]
    [DescriptionResource("GenerateModelingCodeCommand_Description", typeof(Resources))]
    public class GenerateModelingCodeCommand : FeatureCommand
    {
        private const bool DefaultSantizeName = true;
        private const string DefaultTargetBuildAction = "";
        private const CopyToOutput DefaultTargetCopyToOutput = CopyToOutput.DoNotCopy;
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<GenerateModelingCodeCommand>();

        // TODO: must be kept in sync with the PowerTools ModelingTextTransformation context keys.
        // Compatibility with PowerTools templates.
        private const string ElementReferenceCallContextKey = "ElementReference";
        private const string ElementTargetNameContextKey = "ElementTargetName";
        private const string ElementNamespaceContextKey = "ElementNamespace";
        private const string ElementAssemblyPathCallContextKey = "ElementAssemblyPath";

        /// <summary>
        /// Creates a new instance fo the <see cref="GenerateModelingCodeCommand"/> class.
        /// </summary>
        public GenerateModelingCodeCommand()
        {
            this.SanitizeName = DefaultSantizeName;
            this.TargetBuildAction = DefaultTargetBuildAction;
            this.TargetCopyToOutput = DefaultTargetCopyToOutput;
            this.Template = new Lazy<ITemplate>(() =>
            {
                var template = this.UriService.TryResolveUri<ITemplate>(this.TemplateUri);
                if (template == null)
                    throw new FileNotFoundException(Resources.GenerateModelingCodeCommand_TemplateNotFound, this.TemplateUri.ToString());

                return template;
            });
        }

        /// <summary>
        /// Optional Build Action to set on the generated output item. If empty, 
        /// the default for the owning project or file extension will be used automatically by Visual Studio.
        /// </summary>
        [DisplayNameResource("GenerateModelingCodeCommand_TargetBuildAction_DisplayName", typeof(Resources))]
        [DescriptionResource("GenerateModelingCodeCommand_TargetBuildAction_Description", typeof(Resources))]
        [DefaultValue(DefaultTargetBuildAction)]
        public string TargetBuildAction { get; set; }

        /// <summary>
        /// Optional Copy To Ouput to set on the generated output item. If empty, 
        /// </summary>
        [DisplayNameResource("GenerateModelingCodeCommand_TargetCopyToOutput_DisplayName", typeof(Resources))]
        [DescriptionResource("GenerateModelingCodeCommand_TargetCopyToOutput_Description", typeof(Resources))]
        [DefaultValue(DefaultTargetCopyToOutput)]
        public CopyToOutput TargetCopyToOutput { get; set; }

        /// <summary>
        /// Gets or sets the value of TemplateUri during authoring time.
        /// </summary>
        [Browsable(false)]
        public virtual Uri TemplateAuthoringUri { get; set; }

        /// <summary>
        /// Gets or sets the template filename.
        /// </summary>
        [DisplayNameResource("GenerateModelingCodeCommand_TemplateUri_DisplayName", typeof(Resources))]
        [DescriptionResource("GenerateModelingCodeCommand_TemplateUri_Description", typeof(Resources))]
        [Editor(typeof(TextTemplateUriEditor), typeof(UITypeEditor))]
        [DesignOnly(true)]
        [Required]
        public virtual Uri TemplateUri { get; set; }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        [DisplayNameResource("GenerateModelingCodeCommand_TargetPath_DisplayName", typeof(Resources))]
        [DescriptionResource("GenerateModelingCodeCommand_TargetPath_Description", typeof(Resources))]
        public virtual string TargetPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the target.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("GenerateModelingCodeCommand_TargetFileName_DisplayName", typeof(Resources))]
        [DescriptionResource("GenerateModelingCodeCommand_TargetFileName_Description", typeof(Resources))]
        public virtual string TargetFileName { get; set; }

        /// <summary>
        /// Gets or sets the automation extension that executes this command.
        /// </summary>
        [DisplayNameResource("GenerateModelingCodeCommand_ModelElement_DisplayName", typeof(Resources))]
        [DescriptionResource("GenerateModelingCodeCommand_ModelElement_Description", typeof(Resources))]
        public virtual ModelElement ModelElement { get; set; }

        /// <summary>
        /// Gets or sets the model file where the <see cref="ModelElement"/> lives.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("GenerateModelingCodeCommand_ModelFile_DisplayName", typeof(Resources))]
        [DescriptionResource("GenerateModelingCodeCommand_ModelFile_Description", typeof(Resources))]
        public virtual string ModelFile { get; set; }

        /// <summary>
        /// Gets or sets the whether to sanitize names or not.
        /// </summary>
        [DefaultValue(DefaultSantizeName)]
        [DesignOnly(true)]
        [DisplayNameResource("GenerateModelingCodeCommand_SanitizeName_DisplayName", typeof(Resources))]
        [DescriptionResource("GenerateModelingCodeCommand_SanitizeName_Description", typeof(Resources))]
        public virtual bool SanitizeName { get; set; }

        /// <summary>
        /// Gets the generated item in the solution
        /// </summary>
        protected internal virtual IItem GeneratedItem { get; private set; }

        /// <summary>
        /// Gets or sets the URI reference service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual IFxrUriReferenceService UriService { get; set; }

        /// <summary>
        /// Gets or sets the current solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual ISolution Solution { get; set; }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        [Required]
        [Import(typeof(SVsServiceProvider))]
        public virtual IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Lazily evaluates the <see cref="TemplateUri"/> using the <see cref="UriService"/> 
        /// to resolve it to its actual template instance to unfold.
        /// </summary>
        [Browsable(false)]
        public Lazy<ITemplate> Template { get; set; }

        /// <summary>
        /// Runs the configured template data against the element.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.GenerateModelingCodeCommand_TraceInitial, this.ModelFile, this.ModelElement.Id, this.TemplateUri.OriginalString, this.TargetPath, this.TargetFileName, this.SanitizeName);

            var template = this.Template.Value;

            var resolver = new PathResolver(this.ModelElement, this.UriService, this.TargetPath,
                        string.IsNullOrEmpty(this.TargetFileName) ? Path.ChangeExtension(((IProductElement)this.ModelElement).InstanceName, ".t4") : this.TargetFileName);
            if (!resolver.TryResolve())
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.GenerateModelingCodeCommand_ErrorTargetPathNameNotResolved, ((IProductElement)this.ModelElement).InstanceName, this.TargetPath));
            }

            var targetContainer = this.Solution.FindOrCreate(resolver.Path);
            var elementReference = SerializeReference();

            tracer.TraceInformation(
                Resources.GenerateModelingCodeCommand_TraceCreatedReference, elementReference);

            var elementType = this.ModelElement.GetType();

            CallContext.LogicalSetData(ModelElementDirectiveProcessor.KeyCallContextElementAssemblyPath, elementType.Assembly.ManifestModule.FullyQualifiedName);
            CallContext.LogicalSetData(ModelElementDirectiveProcessor.KeyCallContextElementNamespace, elementType.Namespace);

            // TODO: use template.Parameters
            CallContext.LogicalSetData(ModelElementTextTransformation.KeyModelBusReference, elementReference);
            CallContext.LogicalSetData(ModelElementTextTransformation.KeyTargetName, resolver.FileName);

            // Compatibility with PowerTools templates
            CallContext.LogicalSetData(ElementReferenceCallContextKey, elementReference);
            CallContext.LogicalSetData(ElementTargetNameContextKey, resolver.FileName);
            CallContext.LogicalSetData(ElementAssemblyPathCallContextKey, elementType.Assembly.ManifestModule.FullyQualifiedName);
            CallContext.LogicalSetData(ElementNamespaceContextKey, elementType.Namespace);

            var solutionItemName = resolver.FileName;

            if (SanitizeName)
            {
                solutionItemName = SanitizeItemName(solutionItemName);

                tracer.TraceVerbose(
                    Resources.GenerateModelingCodeCommand_TraceSanitizedName, solutionItemName);
            }

            tracer.TraceInformation(
                Resources.GenerateModelingCodeCommand_TraceGeneratingItem, solutionItemName, targetContainer.GetLogicalPath());

            // If the template supports assigning parameters.
            if (template.Parameters != null)
                template.Parameters.Element = this.ModelElement;

            var output = template.Unfold(solutionItemName, targetContainer);
            if (output != null && output.Kind == ItemKind.Item)
            {
                var item = (IItem)output;

                // Set the model bus reference
                if (item.GetContainingProject() != null)
                    item.Data.SourceModelReference = elementReference;

                if (this.TargetBuildAction != DefaultTargetBuildAction)
                    item.Data.ItemType = this.TargetBuildAction;

                if (this.TargetCopyToOutput != DefaultTargetCopyToOutput)
                    item.Data.CopyToOutputDirectory = (int)this.TargetCopyToOutput;

                this.GeneratedItem = item;
            }
        }

        /// <summary>
        /// Serializes the model element reference to marshal across the T4 app domain.
        /// </summary>
        protected virtual string SerializeReference()
        {
            var modelBus = this.ServiceProvider.GetService<SModelBus, IModelBus>();
            if (modelBus == null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ServiceUnavailable,
                    typeof(IModelBus)));
            }

            var instanceType = this.ModelElement.GetType();
            var adapter = (from manager in modelBus.FindAdapterManagers(new object[] { this.ModelFile })
                           from adapterId in manager.GetSupportedLogicalAdapterIds()
                           where manager.GetExposedElementTypes(adapterId).Any(type => type.Type.IsAssignableFrom(instanceType))
                           select new
                           {
                               Manager = manager,
                               AdapterId = adapterId
                           })
                           .FirstOrDefault();

            if (adapter == null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.GenerateModelingCodeCommand_ModelBusAdapterRequired,
                    this.ModelFile,
                    instanceType));
            }

            var busReference = new ModelBusReference(
                modelBus,
                adapter.AdapterId,
                Path.GetFileNameWithoutExtension(this.ModelFile),
                instanceType.Name,
                new ModelingAdapterReference(this.ModelElement.Id.ToString(), null, this.ModelFile));

            return modelBus.SerializeReference(busReference);
        }

        /// <summary>
        /// Sanitizes the filename for use in Solution Explorer.
        /// </summary>
        /// <returns></returns>
        protected string SanitizeItemName(string solutionItemName)
        {
            return DataFormats.MakeValidSolutionItemName(solutionItemName).Replace(" ", "");
        }
    }
}