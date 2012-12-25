using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Store extension methods.
    /// </summary>
    internal static class StoreExtensions
    {
        /// <summary>
        /// Gets the root element.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <returns>The root element.</returns>
        internal static PatternModelSchema GetRootElement(this Store store)
        {
            return store.DefaultPartition.ElementDirectory.AllElements.OfType<PatternModelSchema>().SingleOrDefault();
        }

        /// <summary>
        /// Gets the default view.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <returns>The default view.</returns>
        internal static ViewSchema GetDefaultView(this Store store)
        {
            return store.DefaultPartition.ElementDirectory.AllElements.OfType<ViewSchema>()
                .Single(vw => vw.IsDefault);
        }

        /// <summary>
        /// Gets the views.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <returns>The view list.</returns>
        internal static IEnumerable<ViewSchema> GetViews(this Store store)
        {
            return store.DefaultPartition.ElementDirectory.AllElements.OfType<ViewSchema>();
        }

        /// <summary>
        /// Gets the diagram for default view.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <returns>The diagram for the default view.</returns>
        internal static PatternModelSchemaDiagram GetDiagramForDefaultView(this Store store)
        {
            var diagrams = store.GetDiagrams();

            return diagrams.Single(dg => dg.IsViewRepresented(store.GetDefaultView()));
        }

        /// <summary>
        /// Gets the diagram for view.
        /// </summary>
        /// <param name="store">The given store.</param>
        /// <param name="view">The given view.</param>
        /// <returns>The diagram for the view.</returns>
        internal static PatternModelSchemaDiagram GetDiagramForView(this Store store, ViewSchema view)
        {
            var diagrams = store.GetDiagrams();

            return diagrams.Single(dg => dg.IsViewRepresented(view));
        }

        /// <summary>
        /// Gets the current diagrams.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <returns>The current diagram.</returns>
        internal static PatternModelSchemaDiagram GetCurrentDiagram(this Store store)
        {
            var diagrams = store.GetDiagrams();

            if (diagrams.Count() == 1)
            {
                return diagrams.First();
            }

            return diagrams.Single(
                dg => dg.Id.ToString().Equals(
                    store.GetRootElement().Pattern.CurrentDiagramId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the diagrams.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <returns>The diagram list.</returns>
        internal static IEnumerable<PatternModelSchemaDiagram> GetDiagrams(this Store store)
        {
            return store.DefaultPartition.ElementDirectory.FindElements<PatternModelSchemaDiagram>();
        }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        /// <param name="store">The store.</param>
        internal static ViewSchema GetCurrentView(this Store store)
        {
            return store.GetViews().Single(
                vw => vw.DiagramId.Equals(
                    store.GetRootElement().Pattern.CurrentDiagramId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the diagram partition.
        /// </summary>
        /// <param name="store">The store.</param>
        internal static Partition GetDefaultDiagramPartition(this Store store)
        {
            return store.DefaultPartition;
        }
    }
}