using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Extensions for customizable model elements.
    /// </summary>
    internal static class CustomizationExtensions
    {
        /// <summary>
        /// Returns the customizable element container of the given element (if any).
        /// </summary>
        public static CustomizableElementSchema GetParentCustomizationElement(this ModelElement element)
        {
            var parentElement = DomainRelationshipInfo.FindEmbeddingElement(element);
            var parent = parentElement as CustomizableElementSchema;
            return parent;
        }

        /// <summary>
        /// Returns the first ancestor customizable element that has a customization value.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static CustomizableElementSchema FindYoungestNonInheritingAncestor(this CustomizableElementSchema element)
        {
            return element.Traverse(
                c => DomainClassInfo.FindEmbeddingElement(c) as CustomizableElementSchema,
                c => c == null || c.IsCustomizable != CustomizationState.Inherited);
        }

        /// <summary>
        /// Returns the first ancestor customizable element that has a customization value.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static CustomizationState GetAncestorCustomizationState(this CustomizableElementSchema element)
        {
            var ancestor = element.FindYoungestNonInheritingAncestor();
            return ancestor.IsCustomizable;
        }
    }
}
