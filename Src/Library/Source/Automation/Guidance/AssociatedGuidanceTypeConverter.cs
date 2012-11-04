using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <summary>
	/// Type converter to display guidance automation settings.
	/// </summary>
	internal class AssociatedGuidanceTypeConverter : ExpandableObjectConverter
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

			var element = ExtensionElement.GetExtension<GuidanceExtension>(mel);
			if (element != null)
			{
				// Copy descriptors from owner (make Browsable)
				descriptors = descriptors.Concat(new[] 
				{ 
					new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.GuidanceInstanceName),
						new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<GuidanceExtension, string>(e => e.GuidanceInstanceName))), 
					new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.GuidanceActivateOnCreation),
						new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<GuidanceExtension, bool>(e => e.GuidanceActivateOnCreation))), 
					new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.GuidanceFeatureId),
						new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<GuidanceExtension, string>(e => e.GuidanceFeatureId))), 
					new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.GuidanceSharedInstance),
						new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<GuidanceExtension, bool>(e => e.GuidanceSharedInstance))) 
				});
			}

			return new PropertyDescriptorCollection(descriptors.ToArray());
		}
	}
}
