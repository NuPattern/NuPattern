using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Shell;

namespace NuPattern.Runtime.Schema
{
    internal static class ViewSchemaHelper
    {
        public static string PatternModelFileExtension = DesignerConstants.ModelExtension;
        public static string PatternModelDiagramFileExtension = DesignerConstants.DiagramFileExtension;

        /// <summary>
        /// Opens the pattern model from the given file in the solution, and executes the given action.
        /// </summary>
        /// <returns></returns>
        public static void WithPatternModel(string patternModelFilePath, Action<IPatternModelSchema> action, bool leaveOpen = true)
        {
            Guard.NotNullOrEmpty(() => patternModelFilePath, patternModelFilePath);
            Guard.NotNull(() => action, action);

            NuPattern.Modeling.Windows.WindowExtensions.DoActionOnDesigner(patternModelFilePath, docData =>
                {
                    if (docData != null)
                    {
                        var patternModelSchema = docData.Store.GetRootElement();
                        if (patternModelSchema != null)
                        {
                            action(patternModelSchema);
                        }
                    }
                }, leaveOpen);
        }


        /// <summary>
        /// Selects (activates) the current diagram.
        /// </summary>
        public static void SelectViewDiagram(string patternModelFilePath, IViewSchema view)
        {
            Guard.NotNullOrEmpty(() => patternModelFilePath, patternModelFilePath);
            Guard.NotNull(() => view, view);

            var docData = NuPattern.Modeling.Windows.WindowExtensions.GetDesignerData(patternModelFilePath);

            var docview = docData.DocViews.First() as SingleDiagramDocView;
            var diagram = docData.Store.GetDiagramForView(view as ViewSchema);
            if (docview != null && diagram != null)
            {
                docview.Diagram = diagram;
                view.Pattern.CurrentDiagramId = diagram.Id.ToString();
            }
        }

        /// <summary>
        /// Gets the given view in the pattern.
        /// </summary>
        /// <returns></returns>
        public static IViewSchema GetView(this IPatternSchema pattern, Guid defaultViewId)
        {
            Guard.NotNull(() => pattern, pattern);
            Guard.NotNull(() => defaultViewId, defaultViewId);

            return pattern.Views.FirstOrDefault(v => ((INamedElementSchema)v).Id == defaultViewId);
        }

        /// <summary>
        /// Sets the given view as the default view in the pattern.
        /// </summary>
        /// <returns></returns>
        public static void SetDefaultView(this IPatternSchema pattern, Guid defaultViewId)
        {
            Guard.NotNull(() => pattern, pattern);
            Guard.NotNull(() => defaultViewId, defaultViewId);

            var viewSchema = pattern.Views.FirstOrDefault(v => ((INamedElementSchema)v).Id == defaultViewId);
            if (viewSchema != null)
            {
                viewSchema.IsDefault = true;
            }
        }

        /// <summary>
        /// Deletes the view from teh current pattern.
        /// </summary>
        public static bool DeleteView(this IPatternSchema pattern, Guid defaultViewId)
        {
            Guard.NotNull(() => pattern, pattern);
            Guard.NotNull(() => defaultViewId, defaultViewId);

            var viewSchema = pattern.Views.FirstOrDefault(v => ((INamedElementSchema)v).Id == defaultViewId);
            if (viewSchema != null)
            {
                if (viewSchema.IsDefault)
                {
                    // Set another view as the default
                    SetOtherViewAsDefault(pattern, viewSchema);
                }

                // Delete from the DSL
                pattern.DeleteViewSchema(viewSchema);

                return true;
            }

            return false;
        }

        private static void SetOtherViewAsDefault(IPatternSchema pattern, IViewSchema viewSchema)
        {
            var otherViewSchema = pattern.Views.ToList<INamedElementSchema>()
                .OrderBy(v => v.Name)
                .FirstOrDefault(v => v.Id != ((INamedElementSchema)viewSchema).Id);

            if (otherViewSchema != null)
            {
                SetDefaultView(pattern, otherViewSchema.Id);
            }
        }
    }
}
