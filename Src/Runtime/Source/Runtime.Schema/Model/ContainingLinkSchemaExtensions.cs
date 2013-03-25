using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    internal static class ContainingLinkSchemaExtensions
    {
        /// <summary>
        /// Returns the value of the property projected from the containing link to the element.
        /// </summary>
        /// <returns></returns>
        public static TResult GetProjectedLinkProperty<TResult>(this PatternElementSchema element, Func<IContainingLinkSchema, TResult> property)
        {
            if (element is AbstractElementSchema)
            {
                var viewLink = DomainRoleInfo.GetElementLinks<ViewHasElements>(element, ViewHasElements.AbstractElementSchemaDomainRoleId).FirstOrDefault();
                if (viewLink != null)
                {
                    return property(viewLink);
                }

                var elementLink = DomainRoleInfo.GetElementLinks<ElementHasElements>(element, ElementHasElements.ChildElementDomainRoleId).FirstOrDefault();
                if (elementLink == null)
                {
                    throw new InvalidOperationException(Resources.AutoCreate_NoParentLinkFound);
                }

                return property(elementLink);
            }

            if (element is ExtensionPointSchema)
            {
                var viewLink = DomainRoleInfo.GetElementLinks<ViewHasExtensionPoints>(element, ViewHasExtensionPoints.ExtensionPointSchemaDomainRoleId).FirstOrDefault();
                if (viewLink != null)
                {
                    return property(viewLink);
                }

                var elementLink = DomainRoleInfo.GetElementLinks<ElementHasExtensionPoints>(element, ElementHasExtensionPoints.ChildElementDomainRoleId).FirstOrDefault();
                if (elementLink == null)
                {
                    throw new InvalidOperationException(Resources.AutoCreate_NoParentLinkFound);
                }

                return property(elementLink);
            }


            throw new NotImplementedException();
        }
    }
}
