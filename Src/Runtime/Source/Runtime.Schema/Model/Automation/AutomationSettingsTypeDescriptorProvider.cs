using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using Microsoft.VisualStudio.Modeling.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Automation Type Descriptor Provider for automation settings.
	/// </summary>
	public class AutomationSettingsTypeDescriptorProvider : ElementTypeDescriptionProvider
	{
		/// <summary>
		/// Overridables for the derived class to provide a custom type descriptor.
		/// </summary>
		/// <param name="parent">Parent custom type descriptor.</param>
		/// <param name="element">Element to be described.</param>
		protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
		{
			if (element is AutomationSettingsSchema)
			{
				return new AutomationSettingsTypeDescriptor(parent, element);
			}

			return base.CreateTypeDescriptor(parent, element);
		}
	}
	/// <summary>
	/// Automation Type Descriptor that injects automation settings.
	/// </summary>
	internal class AutomationSettingsTypeDescriptor : CustomizableElementTypeDescriptor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AutomationSettingsTypeDescriptor"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="modelElement">The model element.</param>
		public AutomationSettingsTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
			: base(parent, modelElement)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AutomationSettingsTypeDescriptor"/> class.
		/// </summary>
		/// <param name="modelElement">The model element.</param>
		public AutomationSettingsTypeDescriptor(ModelElement modelElement)
			: base(modelElement)
		{
		}

		/// <summary>
		/// Returns the properties for this instance of a component using the attribute array as a filter.
		/// </summary>
		/// <param name="attributes">An array of type Attribute that is used as a filter.</param>
		/// <returns>
		/// An array of type Attribute that represents the properties for this component instance that match the given set of attributes.
		/// </returns>
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var properties = base.GetProperties(attributes);

			if (this.ModelElement.GetAllExtensions().Any())
			{
				var extension = this.ModelElement.GetAllExtensions().First();

				// Get the existing descriptor for the Settings property
				var oldSettingProperty = properties[Properties.Resources.SettingsPropertyName];
				if (oldSettingProperty != null)
				{
					var oldSettingPropertyAttributes = oldSettingProperty.Attributes.OfType<Attribute>().ToArray<Attribute>();

					// Remove old settings property
					properties.Remove(oldSettingProperty);

					// Add new settings property descriptor
					properties.Add(new SimpleNamedPropertyDescriptor(
						oldSettingProperty.Name, extension, oldSettingPropertyAttributes));
				}
			}

			return properties;
		}


		/// <summary>
		/// Returns the property descriptors for any extension elements.
		/// </summary>
		/// <param name="baseElement">The current element being described.</param>
		/// <param name="attributes">An array of type Attribute that is used as a filter.</param>
		/// <returns>
		/// A collection containing the properties of all extension elements.
		/// </returns>
		protected override IEnumerable<PropertyDescriptor> GetExtensionProperties(ModelElement baseElement, Attribute[] attributes)
		{
			var propertyDescriptors = new List<PropertyDescriptor>();

			return propertyDescriptors;
		}
	}
}