using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <summary>
	/// Type converter to display validation settings.
	/// </summary>
	internal class ValidationExecutionTypeConverter : ExpandableObjectConverter
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

			var element = ExtensionElement.GetExtension<ValidationExtension>(mel);
			if (element != null)
			{
				// Copy descriptors from owner (make Browsable)
				var descriptor1 = new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.ValidationOnBuild,
					new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<ValidationExtension, bool>(e => e.ValidationOnBuild))));
				var descriptor2 = new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.ValidationOnCustomEvent,
					new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<ValidationExtension, string>(e => e.ValidationOnCustomEvent))));
				var descriptor3 = new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.ValidationOnMenu,
					new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<ValidationExtension, bool>(e => e.ValidationOnMenu))));
				var descriptor4 = new DelegatingPropertyDescriptor(element, TypedDescriptor.CreateProperty(element, extension => extension.ValidationOnSave,
					new BrowsableAttribute(true), new DefaultValueAttribute(element.GetPropertyDefaultValue<ValidationExtension, bool>(e => e.ValidationOnSave))));
				descriptors = descriptors.Concat(new[] { descriptor1, descriptor2, descriptor3, descriptor4 });
			}

			return new PropertyDescriptorCollection(descriptors.ToArray());
		}
	}
}
