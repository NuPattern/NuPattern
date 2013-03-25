using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Extensibility;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.Common.Presentation;
using NuPattern.Extensibility;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Shell;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Add automation extension command.
    /// </summary>
    [AuthoringCommandExtension]
    internal class AddAutomationExtensionCommand : ModelingCommand<IPatternElementSchema>
    {
        [Import(typeof(SVsServiceProvider))]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        internal IServiceProvider ServiceProvider { get; set; }

        [ImportMany]
        internal IEnumerable<Lazy<IAutomationSettings, IExportedAutomationMetadata>> AutomationSettings { get; set; }

        internal Func<object, IDialogWindow> DialogFactory
        {
            get
            {
                var shell = this.ServiceProvider.GetService<SVsUIShell, IVsUIShell>();
                return context => shell.CreateDialog<AddAutomationExtensionView>(context);
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The command text.</value>
        public override string Text
        {
            get
            {
                return Properties.ShellResources.AddAutomationExtensionCommand_CommandCaption;
            }
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Unavoidable")]
        public override void Execute(IMenuCommand command)
        {
            var viewModel = new AddAutomationExtensionViewModel(this.AutomationSettings.Select(l => l.Metadata));

            var view = this.DialogFactory(viewModel);

            if (view.ShowDialog().GetValueOrDefault())
            {
                var diagramItems = new DiagramItemCollection();
                var clientView = this.View.CurrentDesigner.DiagramClientView;

                foreach (var target in this.CurrentSelection)
                {
                    var element = target as IPatternElementSchema;

                    var automationSchema = element.CreateAutomationSettingsSchema(aes =>
                    {
                        var aesMel = (ModelElement)aes;
                        var extension = aesMel.AddExtension(viewModel.CurrentExportedAutomation.ExportingType);

                        string displayName = extension.GetDomainClass().DisplayName;

                        aes.Name = aesMel.GetUniqueName(SanitizeName(displayName));
                        aes.AutomationType = displayName;
                        aes.Classification = ((IAutomationSettings)extension).Classification;
                    });

                    var shape = PresentationViewsSubject.GetPresentation((PatternElementSchema)element).OfType<CompartmentShape>().FirstOrDefault();

                    if (shape != null)
                    {
                        var diagramItem = shape.FindDiagramItem<AutomationSettingsSchema>(a => a.Id == automationSchema.Id);

                        if (diagramItem != null)
                        {
                            diagramItems.Add(diagramItem);
                        }
                    }
                }

                clientView.Selection.Set(diagramItems);
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
                        .Where(shape => IsAutomationSettingsCompartment(shape))
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
                this.CurrentSelection.OfType<PatternElementSchema>().Any();
        }

        private static bool IsAutomationSettingsCompartment(ShapeElement shape)
        {
            var compartment = shape as ElementListCompartment;

            return compartment != null &&
                compartment.DefaultCreationDomainClass.Id == AutomationSettingsSchema.DomainClassId;
        }

        private static string SanitizeName(string value)
        {
            Guard.NotNull(() => value, value);

            return value.Replace(" ", string.Empty);
        }
    }
}