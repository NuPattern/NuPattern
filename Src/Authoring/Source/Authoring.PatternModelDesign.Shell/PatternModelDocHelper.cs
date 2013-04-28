using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Modeling;

namespace NuPattern.Runtime.Schema
{
    internal static class PatternModelDocHelper
    {
        /// <summary>
        /// Runs the valiation on the pattern model file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ValidateDocument(string fileName)
        {
            var valid = false;
            NuPattern.Modeling.Windows.WindowExtensions.DoActionOnDesigner(fileName, docData =>
                {
                    var document = docData as PatternModelDocData;
                    if (document != null)
                    {
                        // Validate
                        valid = document.ValidationController.Validate(docData.GetAllElementsForValidation(), ValidationCategories.Save);
                        if (!valid)
                        {
                            // Open document for editing if fails validation
                            NuPattern.VisualStudio.Windows.WindowExtensions.ActivateDocument(fileName);
                        }
                    }
                }, true);

            return valid;
        }


        /// <summary>
        /// Creates a new view digram for the pattern model.
        /// </summary>
        /// <param name="patternModel">The pattern model</param>
        /// <param name="docData">The document window data</param>
        /// <returns></returns>
        public static Guid CreateNewViewDiagram(PatternModelSchema patternModel, ModelingDocData docData)
        {
            Guard.NotNull(() => patternModel, patternModel);
            Guard.NotNull(() => docData, docData);

            // Create a new diagram file
            var docView = docData.DocViews.FirstOrDefault() as SingleDiagramDocView;
            PatternModelSchemaDiagram diagram = null;
            patternModel.Store.TransactionManager.DoWithinTransaction(() =>
            {
                diagram = PatternModelSerializationHelper.CreatePatternModelSchemaDiagram(
                    new SerializationResult(),
                    patternModel.Store.DefaultPartition,
                    patternModel.Store.GetRootElement(),
                    string.Empty);
            });
            if (diagram != null)
            {
                SetCurrentDiagram(docView, diagram, patternModel.Pattern);

                FixUpDiagram(patternModel, patternModel.Pattern, diagram.Id.ToString(),
                    PresentationViewsSubject.GetPresentation(patternModel.Pattern).OfType<ShapeElement>());

                return diagram.Id;
            }

            return Guid.Empty;
        }

        private static void SetCurrentDiagram(SingleDiagramDocView docview, PatternModelSchemaDiagram diagram, PatternSchema pattern)
        {
            docview.Diagram = diagram;

            pattern.WithTransaction(prod => prod.CurrentDiagramId = diagram.Id.ToString());
        }

        private static void FixUpDiagram(ModelElement root, ModelElement child, string diagramId, IEnumerable<ShapeElement> shapes)
        {
            if (shapes.Count() == 0 ||
                !shapes.Any(shape => ((PatternModelSchemaDiagram)shape.Diagram).Id.ToString().Equals(diagramId, StringComparison.OrdinalIgnoreCase)))
            {
                root.Store.TransactionManager.DoWithinTransaction(() => Diagram.FixUpDiagram(root, child));
            }
        }
    }
}
