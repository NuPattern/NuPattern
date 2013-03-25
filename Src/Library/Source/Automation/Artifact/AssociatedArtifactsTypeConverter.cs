using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using NuPattern.ComponentModel;
using NuPattern.ComponentModel.Design;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Type converter to display artifact automation settings.
    /// </summary>
    internal class AssociatedArtifactsTypeConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Returns the property descriptors for this instance.
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            Guard.NotNull(() => context, context);

            var descriptors = base.GetProperties(context, value, attributes).Cast<PropertyDescriptor>();

            // Remove descriptors for the data type of this property (string)
            descriptors = descriptors.Where(descriptor => descriptor.ComponentType != typeof(string));

            // Get the model element being described
            var selection = context.Instance;
            ModelElement mel = selection as ModelElement;
            var pel = selection as PresentationElement;
            if (pel != null)
            {
                mel = pel.Subject;
            }

            var element = ExtensionElement.GetExtension<ArtifactExtension>(mel);
            if (element != null)
            {
                // Copy descriptors from owner (make Browsable)
                var descriptor1 = new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.OnArtifactActivation),
                    new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<ArtifactExtension, ArtifactActivatedAction>(e => e.OnArtifactActivation)));
                var descriptor2 = new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.OnArtifactDeletion),
                    new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<ArtifactExtension, ArtifactDeletedAction>(e => e.OnArtifactDeletion)));
                descriptors = descriptors.Concat(new[] { descriptor1, descriptor2 });
            }

            return new PropertyDescriptorCollection(descriptors.ToArray());
        }
    }
}
