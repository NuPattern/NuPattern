using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using NuPattern.ComponentModel.Design;
using NuPattern.Reflection;
using NuPattern.Runtime.Design;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Provides a custom type descriptor for the <see cref="IContainingLinkSchema" /> class. 
    /// </summary>
    internal class ContainingLinkSchemaTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Returns an instance of a type descriptor for the given instance of the class.
        /// </summary>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            if (element is ElementLink)
            {
                return new ContainingLinkSchemaTypeDescriptor(element as ElementLink);
            }

            return base.CreateTypeDescriptor(parent, element);
        }

        /// <summary>
        /// Provides the custom type descriptor for <see cref="IContainingLinkSchema"/> relationship classes, 
        /// that displays properties based on current authoring mode.
        /// </summary>
        private class ContainingLinkSchemaTypeDescriptor : PropertyDescriptorTypeDescriptor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ContainingLinkSchemaTypeDescriptor"/> class.
            /// </summary>
            /// <param name="elementLink">The model element.</param>
            public ContainingLinkSchemaTypeDescriptor(ElementLink elementLink)
                : base(elementLink)
            {
            }

            /// <summary>
            /// Returns the properties that reflect the current state of the relationship.
            /// </summary>
            public override System.ComponentModel.PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>();
                var link = (ElementLink)this.ModelElement;


                return ApplyDesignPolicy(link, properties);
            }

            /// <summary>
            /// Applies the design-time policy to the given element properties.
            /// </summary>
            private static PropertyDescriptorCollection ApplyDesignPolicy(ElementLink link, IEnumerable<PropertyDescriptor> properties)
            {
                var descriptors = properties.ToDictionary(property => property.Name);

                // Get the target role as a NamedElement
                NamedElementSchema targetElement = DomainRoleInfo.GetTargetRolePlayer(link) as NamedElementSchema;
                if (targetElement != null)
                {
                    if (targetElement.IsInheritedFromBase)
                    {
                        // Add read-only attribute to the Cardinality property of the element (and its relationship to the owning parent element)
                        string propertyName = Reflector<ViewHasElements>.GetProperty(rellie => rellie.Cardinality).Name;
                        var cardinalityProperty = descriptors[propertyName];
                        descriptors[cardinalityProperty.Name] = new DelegatingPropertyDescriptor(cardinalityProperty, new ReadOnlyAttribute(true));
                    }
                }

                return new PropertyDescriptorCollection(descriptors.Values.ToArray());
            }
        }
    }
}
