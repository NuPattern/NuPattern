using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.Extensibility;
using NuPattern.Runtime;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Command used to validate the pattern model 
    /// </summary>
    [DisplayNameResource("ValidatePatternModelCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("ValidatePatternModelCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ValidatePatternModelCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ValidatePatternModelCommand>();

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
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        [DisplayNameResource("ValidatePatternModelCommand_TargetPath_DisplayName", typeof(Resources))]
        [DescriptionResource("ValidatePatternModelCommand_TargetPath_Description", typeof(Resources))]
        public virtual string TargetPath { get; set; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ValidatePatternModelCommand_TraceInitial, this.CurrentElement.InstanceName, this.TargetPath);

            var resolver = new PathResolver(this.CurrentElement, this.UriService,
                this.TargetPath);

            resolver.Resolve();

            if (!string.IsNullOrEmpty(resolver.Path))
            {
                var itemContainer = this.Solution.Find(resolver.Path).FirstOrDefault();

                if (itemContainer != null)
                {
                    var rdt = new RunningDocumentTable(ServiceProvider.GlobalProvider);
                    var documentInfo = rdt.FirstOrDefault(info =>
                        info.Moniker.Equals(itemContainer.PhysicalPath, StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrEmpty(documentInfo.Moniker))
                    {
                        var docdata = documentInfo.DocData as PatternModelDocData;

                        if (docdata == null)
                        {
                            //File is opened but not in designer view
                            var projectItem = dte.Solution.FindProjectItem(documentInfo.Moniker);

                            projectItem.Document.Close(vsSaveChanges.vsSaveChangesYes);

                            var document = DesignerCommandHelper.OpenDesigner(this.Dte, itemContainer.PhysicalPath, false);

                            docdata = DesignerCommandHelper.GetModelingDocData(rdt, document) as PatternModelDocData;
                        }

                        tracer.TraceInformation(
                            Resources.ValidatePatternModelCommand_TraceValidating, this.CurrentElement.InstanceName, documentInfo.Moniker);

                        if (!docdata.ValidationController.Validate(docdata.GetAllElementsForValidation(), ValidationCategories.Save))
                        {
                            DesignerCommandHelper.ActivateDocument(dte, documentInfo.Moniker);
                        }
                    }
                    else
                    {
                        var window = OpenDesigner(this.Dte, itemContainer.PhysicalPath);
                        var document = window.Document;
                        var docdata = DesignerCommandHelper.GetModelingDocData(rdt, document) as PatternModelDocData;

                        tracer.TraceInformation(
                            Resources.ValidatePatternModelCommand_TraceValidating, this.CurrentElement.InstanceName, itemContainer.Name);

                        if (!docdata.ValidationController.Validate(docdata.GetAllElementsForValidation(), ValidationCategories.Save))
                        {
                            tracer.TraceInformation(
                                Resources.ValidatePatternModelCommand_TraceOpeningForResolution, this.CurrentElement.InstanceName, itemContainer.Name);

                            window.Visible = true;
                            window.Activate();
                        }
                        else
                        {
                            document.Close(vsSaveChanges.vsSaveChangesNo);
                        }
                    }
                }
            }
            else
            {
                tracer.TraceWarning(
                    Resources.ValidatePatternModelCommand_TraceDesignerNotFound, this.CurrentElement.InstanceName);
            }
        }

        private static Window OpenDesigner(DTE dte, string fileName)
        {
            var projectItem = dte.Solution.FindProjectItem(fileName);

            return projectItem.Open(EnvDTE.Constants.vsViewKindDesigner);
        }
    }
}