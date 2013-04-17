using Microsoft.VisualStudio.Modeling;
using NuPattern.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Extensions ot the <see cref="CustomizableElementSchema"/> class.
    /// </summary>
    internal static class CustomizableElementSchemaExtensions
    {
        /// <summary>
        /// Ensures the customization policy and default settings are created.
        /// </summary>
        public static void EnsurePolicyAndDefaultSettings(this CustomizableElementSchema element)
        {
            // Add the customization policy
            if (element.Policy == null)
            {
                element.WithTransaction(elem => elem.Create<CustomizationPolicySchema>());
            }

            // Add the customizable settings derived from attributes on this model.
            CustomizableSettingSchema.EnsurePolicyPopulated(element);
        }

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
