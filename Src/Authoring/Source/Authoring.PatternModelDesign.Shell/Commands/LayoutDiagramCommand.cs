using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using NuPattern.Modeling;
using NuPattern.Runtime.Authoring;

namespace NuPattern.Runtime.Schema.Commands
{
    /// <summary>
    /// Layout diagram command.
    /// </summary>
    [AuthoringCommandExtension]
    internal class LayoutDiagramCommand : ModelingCommand<INamedElementSchema>
    {
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The command text.</value>
        public override string Text
        {
            get
            {
                return Properties.ShellResources.LayoutDiagramCommand_CommandCaption;
            }
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Unavoidable")]
        public override void Execute(IMenuCommand command)
        {
            var clientView = this.View.CurrentDesigner.DiagramClientView;
            var diagram = clientView.Diagram;

            // Layout current view shape
            var patternModel = diagram.ModelElement as PatternModelSchema;
            var view = patternModel.Pattern.Store.GetCurrentView();
            if (view != null)
            {
                var viewShape = view.GetShape<ViewShape>();
                if (viewShape != null)
                {
                    viewShape.Layout();
                }
            }
        }

        /// <summary>
        /// Queries the status.
        /// </summary>
        /// <param name="command">The command.</param>
        public override void QueryStatus(IMenuCommand command)
        {
            Guard.NotNull(() => command, command);

            command.Visible = command.Enabled = true;
        }
    }
}