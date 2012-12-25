using System.Collections.Generic;
using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    ///  Defines a view in a model element.
    /// </summary>
    public partial class ViewSchema
    {
        /// <summary>
        /// Returns a value indicating whether the source element represented by the
        /// specified root ProtoElement can be added to this element.
        /// </summary>
        /// <param name="rootElement">The root ProtoElement representing a source element.  This can be null,
        /// in which case the ElementGroupPrototype does not contain an ProtoElements
        /// and the code should inspect the ElementGroupPrototype context information.</param>
        /// <param name="elementGroupPrototype">The ElementGroupPrototype that contains the root ProtoElement.</param>
        /// <returns>
        /// True if the source element represented by the ProtoElement can be added to this target element.
        /// </returns>
        protected override bool CanMerge(ProtoElementBase rootElement, ElementGroupPrototype elementGroupPrototype)
        {
            Guard.NotNull(() => rootElement, rootElement);
            Guard.NotNull(() => elementGroupPrototype, elementGroupPrototype);

            return false;
        }

        IEnumerable<IAbstractElementInfo> IElementInfoContainer.Elements
        {
            get { return this.Elements; }
        }

        IEnumerable<IAbstractElementSchema> IElementSchemaContainer.Elements
        {
            get { return this.Elements; }
        }

        IEnumerable<IExtensionPointInfo> IElementInfoContainer.ExtensionPoints
        {
            get { return this.ExtensionPoints; }
        }

        IEnumerable<IExtensionPointSchema> IElementSchemaContainer.ExtensionPoints
        {
            get { return this.ExtensionPoints; }
        }
    }
}