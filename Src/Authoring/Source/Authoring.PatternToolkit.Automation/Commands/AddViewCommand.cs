using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.Authoring.PatternToolkit.Automation.UriProviders;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime;
using NuPattern.Runtime.References;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Command to add a new view
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("AddViewCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("AddViewCommand_Description", typeof(Resources))]
    public class AddViewCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<AddViewCommand>();

        /// <summary>
        /// Gets or sets the service that resolves templates.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public virtual IFxrUriReferenceService UriService { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IViewModel CurrentElement { get; set; }

        /// <summary>
        /// Adds a new view
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            var patternModel = this.CurrentElement.Parent.Parent.AsElement();

            tracer.TraceInformation(
                Resources.AddViewCommand_TraceInitial, patternModel.InstanceName);

            var reference = SolutionArtifactLinkReference.GetResolvedReferences(patternModel, this.UriService).FirstOrDefault();

            if (reference != null)
            {
                using (tracer.StartActivity(Resources.AddViewCommand_TraceAddingView, patternModel.InstanceName, this.CurrentElement.InstanceName))
                {



                    //DesignerCommandHelper.DoActionOnDesigner(
                    //    reference.PhysicalPath,
                    //    docdata =>
                    //    {
                    //        var viewSchema = docdata.Store.GetViews().FirstOrDefault(v => v.Name == this.CurrentElement.InstanceName);
                    //        if (viewSchema != null)
                    //        {
                    //            var view = viewSchema as IViewSchema;
                    //            if (ViewArtifactLinkReference.GetReference(this.CurrentElement.AsElement()) == null)
                    //            {
                    //                tracer.TraceInformation(
                    //                    Resources.AddViewCommand_TraceAddingReference, patternModel.InstanceName);

                    //                ViewArtifactLinkReference.AddReference(this.CurrentElement.AsElement(), this.UriService.CreateUri(view));
                    //            }
                    //        }
                    //        else
                    //        {
                    //            var patternSchema = docdata.Store.GetRootElement().Pattern;
                    //            var docview = docdata.DocViews.First() as SingleDiagramDocView;

                    //            PatternModelSchemaDiagram diagram = null;

                    //            docdata.Store.TransactionManager.DoWithinTransaction(() =>
                    //                {
                    //                    diagram = PatternModelSerializationHelper.CreatePatternModelSchemaDiagram(
                    //                        new SerializationResult(),
                    //                        docdata.Store.DefaultPartition,
                    //                        docdata.Store.GetRootElement(),
                    //                        string.Empty);
                    //                });

                    //            if (diagram != null)
                    //            {
                    //                SetCurrentDiagram(docview, diagram, patternSchema);

                    //                FixUpDiagram(
                    //                    patternSchema.PatternModel,
                    //                    patternSchema,
                    //                    diagram.Id.ToString(),
                    //                    PresentationViewsSubject.GetPresentation(patternSchema).OfType<ShapeElement>());

                    //                docdata.Store.TransactionManager.DoWithinTransaction(() =>
                    //                    {
                    //                        var view = patternSchema.CreateViewSchema(
                    //                            vw =>
                    //                            {
                    //                                ((INamedElementSchema)vw).Name = this.CurrentElement.InstanceName;
                    //                                vw.DiagramId = diagram.Id.ToString();
                    //                            });

                    //                        tracer.TraceInformation(
                    //                            Resources.AddViewCommand_TraceAddingReference, patternModel.InstanceName);

                    //                        ViewArtifactLinkReference.AddReference(this.CurrentElement.AsElement(), this.UriService.CreateUri(view));
                    //                    });
                    //            }
                    //        }
                    //    });
                }
            }
            else
            {
                tracer.TraceWarning(
                    Resources.AddViewCommand_TraceReferenceNotFound, patternModel.InstanceName);
            }
        }

        //private static void SetCurrentDiagram(SingleDiagramDocView docview, PatternModelSchemaDiagram diagram, PatternSchema pattern)
        //{
        //    docview.Diagram = diagram;

        //    pattern.WithTransaction(prod => prod.CurrentDiagramId = diagram.Id.ToString());
        //}

        //private static void FixUpDiagram(ModelElement root, ModelElement child, string diagramId, IEnumerable<ShapeElement> shapes)
        //{
        //    if (shapes.Count() == 0 ||
        //        !shapes.Any(shape => ((PatternModelSchemaDiagram)shape.Diagram).Id.ToString().Equals(diagramId, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        root.Store.TransactionManager.DoWithinTransaction(() => Diagram.FixUpDiagram(root, child));
        //    }
        //}
    }
}