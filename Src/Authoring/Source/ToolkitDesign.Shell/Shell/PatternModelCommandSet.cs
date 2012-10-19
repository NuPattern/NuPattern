using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Shell;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// Pattern model CommandSet class.
    /// </summary>
    internal partial class PatternModelCommandSet
    {
        /// <summary>
        /// Type name for AddCompartmentShapeItemMenuCommand.
        /// </summary>
        private const string AddCompartmentShapeItemMenuCommandTypeName = "AddCompartmentShapeItemMenuCommand";

        /// <summary>
        /// Provide the menu commands that this command set handles.
        /// </summary>
        protected override IList<MenuCommand> GetMenuCommands()
        {
            var commands = base.GetMenuCommands();

            var addCommands = commands.Where(c => c.GetType().Name == AddCompartmentShapeItemMenuCommandTypeName).ToList();

            foreach (var addCommand in addCommands)
            {
                commands.Remove(addCommand);
            }

            return commands;
        }

        /// <summary>
        /// Virtual method for processing the Delete menu status handler.
        /// </summary>
        /// <param name="command">Menu command called from the Visual Studio</param>
        protected override void ProcessOnStatusDeleteCommand(MenuCommand command)
        {
            if (command != null)
            {
                command.Visible = command.Enabled = CanDelete();
            }
        }

        private bool CanDelete()
        {
            DiagramClientView diagramClientView = GetClientView();

            if (diagramClientView == null)
            {
                return false;
            }

            if (this.IsDiagramSelected() || this.IsAnyDocumentSelectionUndeletable())
            {
                return false;
            }

            var elements = BuildList(this.CurrentDocumentSelection.OfType<ModelElement>());

            return diagramClientView.Diagram.ElementOperations.CanDelete(elements, PatternElementHasProperties.PropertySchemaDomainRoleId);
        }

        internal DiagramClientView GetClientView()
        {
            DiagramClientView diagramClientView = null;

            var currentModelingDocView = this.CurrentModelingDocView as DiagramDocView;

            if ((currentModelingDocView != null) && (currentModelingDocView.CurrentDiagram != null))
            {
                var activeDiagramView = currentModelingDocView.CurrentDiagram.ActiveDiagramView;

                if (activeDiagramView != null)
                {
                    diagramClientView = activeDiagramView.DiagramClientView;
                }
            }

            return diagramClientView;
        }

        private static IEnumerable<ModelElement> BuildList(IEnumerable<ModelElement> selectedElements)
        {
            var list = new List<ModelElement>();

            foreach (ModelElement element in selectedElements)
            {
                var item = element as ShapeElement;

                if (item == null)
                {
                    list.Add(element);
                }
                else if (!(item is Diagram))
                {
                    if (item.ModelElement != null)
                    {
                        list.Add(item.ModelElement);
                        continue;
                    }

                    list.Add(item);
                }
            }

            return list;
        }
    }
}