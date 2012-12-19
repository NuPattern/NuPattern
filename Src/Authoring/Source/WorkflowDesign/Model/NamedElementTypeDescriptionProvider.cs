using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Provides a custom type descriptor for the <see cref="NamedElementSchema" /> class. 
	/// </summary>
	public class NamedElementTypeDescriptionProvider : ElementTypeDescriptionProvider
	{
		/// <summary>
		/// Returns an instance of a type descriptor for the given instance of the class.
		/// </summary>
		protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
		{
			if (element is NamedElementSchema)
			{
				return new NamedElementTypeDescriptor(parent, element);
			}

			return base.CreateTypeDescriptor(parent, element);
		}
	}
	/// <summary>
	/// Provides the custom type descriptor for <see cref="NamedElementSchema"/> class.
	/// </summary>
	internal class NamedElementTypeDescriptor : ElementTypeDescriptor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NamedElementTypeDescriptor"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="modelElement">The model element.</param>
		public NamedElementTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
			: base(parent, modelElement)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedElementTypeDescriptor"/> class.
		/// </summary>
		/// <param name="modelElement">The model element.</param>
		public NamedElementTypeDescriptor(ModelElement modelElement)
			: base(modelElement)
		{
		}

		/// <summary>
		/// Returns the properties for customization that reflect the current state of the class.
		/// </summary>
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var properties = base.GetProperties(attributes);
			var element = (NamedElementSchema)this.ModelElement;

			// Add tracking property descriptors
			element.ReplaceTrackingPropertyDescriptors(properties);

			return properties;
		}
	}
}