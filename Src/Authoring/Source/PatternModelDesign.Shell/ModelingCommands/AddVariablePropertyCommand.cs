using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// Add variable property command.
    /// </summary>
    [AuthoringCommandExtension]
    public class AddVariablePropertyCommand : ModelingCommand<IPatternElementSchema>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by MEF.")]
        [Import]
        private IUserMessageService MessageService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The command text.</value>
        public override string Text
        {
            get
            {
                return Properties.ShellResources.VariableProperty_DisplayName;
            }
        }

        /// <summary>
        /// Gets the current selection.
        /// </summary>
        /// <value>The current selection.</value>
        protected override IEnumerable<IPatternElementSchema> CurrentSelection
        {
            get
            {
                var currentSelectionContainer = this.MonitorSelection.CurrentSelectionContainer as ModelingWindowPane;

                if (currentSelectionContainer != null)
                {
                    var selectedShapes = currentSelectionContainer.GetSelectedComponents().OfType<ShapeElement>();

                    return selectedShapes
                        .Where(shape => IsVariablePropertyCompartment(shape))
                        .Select(shape => shape.ParentShape.ModelElement as IPatternElementSchema);
                }

                return null;
            }
        }

        /// <summary>
        /// Queries the status.
        /// </summary>
        /// <param name="command">The command.</param>
        public override void QueryStatus(IMenuCommand command)
        {
            Guard.NotNull(() => command, command);

            command.Visible = command.Enabled =
                this.CurrentSelection != null &&
                this.CurrentSelection.Any() &&
                !this.CurrentSelection.OfType<ExtensionPointSchema>().Any(ext => !string.IsNullOrEmpty(ext.RepresentedExtensionPointId));
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        public override void Execute(IMenuCommand command)
        {
            var propertyContainerSchemas = this.CurrentSelection.Cast<PatternElementSchema>();

            // Warn user if changing tailored extension point contract
            var extensionPoints = propertyContainerSchemas.OfType<ExtensionPointSchema>();
            if (extensionPoints.Any(ext => ext.IsInheritedFromBase) && extensionPoints.Any(ext => ext.Properties.All(p => p.IsInheritedFromBase)))
            {
                var resume = this.MessageService.PromptWarning(Properties.ShellResources.AddVariablePropertyCommand_BreakContractWarning);
                if (!resume)
                {
                    return;
                }
            }

            var diagramItems = new DiagramItemCollection();
            var clientView = this.View.CurrentDesigner.DiagramClientView;

            foreach (var propertyContainerSchema in propertyContainerSchemas)
            {
                var property = propertyContainerSchema.Create<PropertySchema>();

                var shape = PresentationViewsSubject.GetPresentation(propertyContainerSchema).OfType<CompartmentShape>().FirstOrDefault();

                if (shape != null)
                {
                    var diagramItem = shape.FindDiagramItem<PropertySchema>(p => p.Id == property.Id);

                    if (diagramItem != null)
                    {
                        diagramItems.Add(diagramItem);
                    }
                }
            }

            clientView.Selection.Set(diagramItems);
        }

        private static bool IsVariablePropertyCompartment(ShapeElement shape)
        {
            var compartment = shape as ElementListCompartment;

            return compartment != null &&
                compartment.DefaultCreationDomainClass.Id == PropertySchema.DomainClassId;
        }
    }
}