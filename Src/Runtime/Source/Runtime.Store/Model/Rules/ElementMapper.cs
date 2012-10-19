using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
    /// <summary>
    /// Copy between an element and its info.
    /// </summary>
    internal static class ElementMapper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Not Applicable")]
        private static readonly ITraceSource tracer = Tracer.GetSourceFor(typeof(ElementMapper));

        /// <summary>
        /// Sync the elements/collections of the given element.
        /// </summary>
        internal static void SyncElementsFrom(this AbstractElement element, IEnumerable<IAbstractElementInfo> elements)
        {
            element.Elements
                .Where(e => !elements.Any(i => i.Id == e.DefinitionId))
                .ToArray()
                .ForEach(e => e.Delete());

            element.Elements
                .GroupBy(x => x.DefinitionId)
                .Where(x => elements.Any(
                    i => x.Key == i.Id &&
                    (i.Cardinality == Cardinality.OneToOne || i.Cardinality == Cardinality.ZeroToOne) &&
                    x.Count() > 1))
                .SelectMany(x => x.Skip(1))
                .ForEach(x => x.Delete());

            var singletonElements = elements
                .Where(i => i.AutoCreate &&
                    !element.Elements.Any(e => e.DefinitionId == i.Id))
                .ToArray();

            singletonElements.OfType<IElementInfo>().ForEach(x => element.CreateElement(e => e.DefinitionId = x.Id));
            singletonElements.OfType<ICollectionInfo>().ForEach(x => element.CreateCollection(e => e.DefinitionId = x.Id));
        }

        /// <summary>
        /// Sync the elements/collections of the given view.
        /// </summary>
        internal static void SyncElementsFrom(this View view, IEnumerable<IAbstractElementInfo> elements)
        {
            view.Elements
                .Where(e => !elements.Any(i => i.Id == e.DefinitionId))
                .ToArray()
                .ForEach(e => e.Delete());

            view.Elements
                .GroupBy(x => x.DefinitionId)
                .Where(x => elements.Any(
                    i => x.Key == i.Id &&
                    (i.Cardinality == Cardinality.OneToOne || i.Cardinality == Cardinality.ZeroToOne) &&
                    x.Count() > 1))
                .SelectMany(x => x.Skip(1))
                .ForEach(x => x.Delete());

            var singletonElements = elements
                .Where(i => i.AutoCreate &&
                    !view.Elements.Any(e => e.DefinitionId == i.Id))
                .ToArray();

            singletonElements.OfType<IElementInfo>().ForEach(x => view.CreateElement(e => e.DefinitionId = x.Id));
            singletonElements.OfType<ICollectionInfo>().ForEach(x => view.CreateCollection(e => e.DefinitionId = x.Id));
        }

        /// <summary>
        /// Sync the properties of the given property container.
        /// </summary>
        internal static void SyncPropertiesFrom(this ProductElement container, IEnumerable<IPropertyInfo> properties)
        {
            // Delete existing properties that don't have a corresponding definition.
            container.Properties
                .Where(p => !properties.Any(info => info.Id == p.DefinitionId))
                .ToArray()
                .ForEach(p => p.Delete());

            // Initialize all the new properties. Existing ones are not modified.
            foreach (var info in properties.Where(info => !container.Properties.Any(p => p.DefinitionId == info.Id)))
            {
                // Assigning the DefinitionId on create automatically loads the Info property.
                var property = container.CreateProperty(p => { p.DefinitionId = info.Id; });
                // Reset evaluates VP and default value.
                property.Reset();
            }
        }

        /// <summary>
        /// Sync the views of the given pattern.
        /// </summary>
        internal static void SyncViewsFrom(this Product product, IEnumerable<IViewInfo> views)
        {
            product.Views
                .Where(v => !views.Any(i => i.Id == v.DefinitionId))
                .ToArray()
                .ForEach(v => v.Delete());

            views.Where(i => !product.Views.Any(v => v.DefinitionId == i.Id))
                .ForEach(i => product.CreateView(v => v.DefinitionId = i.Id));
        }

        /// <summary>
        /// Sync the extension points of the given element/collection.
        /// </summary>
        internal static void SyncExtensionPointsFrom(
            this AbstractElement element,
            IEnumerable<IExtensionPointInfo> extensionPoints,
            IPatternManager patternManager)
        {
            element.ExtensionProducts
                .Where(p => !IsValidExtensionPoint(p, extensionPoints, patternManager))
                .ToArray()
                .ForEach(p => p.Delete());

            element.ExtensionProducts
                .GroupBy(x => x.DefinitionId)
                .Where(x => x.Count() > 1 && HasSingletonExtensionPointsInElement(x, extensionPoints, patternManager))
                .SelectMany(x => x.Skip(1))
                .ForEach(x => x.Delete());
        }

        /// <summary>
        /// Sync the extension points of the given view.
        /// </summary>
        internal static void SyncExtensionPointsFrom(
            this View view,
            IEnumerable<IExtensionPointInfo> extensionPoints,
            IPatternManager patternManager)
        {
            view.ExtensionProducts
                .Where(p => !IsValidExtensionPoint(p, extensionPoints, patternManager))
                .ToArray()
                .ForEach(p => p.Delete());

            view.ExtensionProducts
                .GroupBy(x => x.DefinitionId)
                .Where(x => x.Count() > 1 && HasSingletonExtensionPointsInView(x, extensionPoints, patternManager))
                .SelectMany(x => x.Skip(1))
                .ForEach(x => x.Delete());
        }

        private static bool IsValidExtensionPoint(
            Product product,
            IEnumerable<IExtensionPointInfo> extensionPoints,
            IPatternManager patternManager)
        {
            return extensionPoints.Any(x => patternManager.GetCandidateExtensionPoints(x.RequiredExtensionPointId)
                    .Any(f => f.Id == product.ExtensionId && f.Schema.Pattern.Id == product.DefinitionId));
        }

        private static bool HasSingletonExtensionPointsInElement(
            IGrouping<Guid, Product> group,
            IEnumerable<IExtensionPointInfo> extensionPoints,
            IPatternManager patternManager)
        {
            return extensionPoints.Any(x =>
                (x.Cardinality == Cardinality.OneToOne || x.Cardinality == Cardinality.ZeroToOne) &&
                patternManager.GetCandidateExtensionPoints(x.RequiredExtensionPointId)
                    .Any(f => f.Schema.Pattern.Id == group.Key && f.Id == group.First().ExtensionId));
        }

        private static bool HasSingletonExtensionPointsInView(
            IGrouping<Guid, Product> group,
            IEnumerable<IExtensionPointInfo> extensionPoints,
            IPatternManager patternManager)
        {
            return extensionPoints.Any(x =>
                (x.Cardinality == Cardinality.OneToOne || x.Cardinality == Cardinality.ZeroToOne) &&
                patternManager.GetCandidateExtensionPoints(x.RequiredExtensionPointId)
                    .Any(f => f.Schema.Pattern.Id == group.Key && f.Id == group.First().ExtensionId));
        }
    }
}