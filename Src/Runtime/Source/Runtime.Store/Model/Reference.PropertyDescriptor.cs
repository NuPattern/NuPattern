using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Extensibility;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Store
{
	internal class ReferencePropertyDescriptor : PropertyDescriptor
	{
		private Reference reference;

		// TODO: we must implement all the type conversion here, as well as the type loading and stuff.

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferencePropertyDescriptor"/> class.
		/// </summary>
		/// <param name="reference">The property.</param>
		/// <param name="service">The <see cref="Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.IFeatureCompositionService"/>.</param>
		public ReferencePropertyDescriptor(IFeatureCompositionService service, Reference reference)
			: base(reference.Kind, BuildAttributes(service, reference))
		{
			this.reference = reference;
		}

		/// <summary>
		/// When overridden in a derived class, gets the type of the component this property is bound to.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.</returns>
		public override Type ComponentType
		{
			get { return this.reference.GetType(); }
		}

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether this property is read-only.
		/// </summary>
		/// <value></value>
		/// <returns>true if the property is read-only; otherwise, false.</returns>
		public override bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// When overridden in a derived class, gets the type of the property.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Type"/> that represents the type of the property.</returns>
		public override Type PropertyType
		{
			get { return typeof(string); }
		}

		/// <summary>
		/// When overridden in a derived class, returns whether resetting an object changes its value.
		/// </summary>
		/// <param name="component">The component to test for reset capability.</param>
		/// <returns>
		/// True if resetting the component changes its value; otherwise, false.
		/// </returns>
		public override bool CanResetValue(object component)
		{
			return true;
		}

		/// <summary>
		/// When overridden in a derived class, gets the current value of the property on a component.
		/// </summary>
		/// <param name="component">The component with the property for which to retrieve the value.</param>
		/// <returns>
		/// The value of a property for a given component.
		/// </returns>
		public override object GetValue(object component)
		{
			return TypeDescriptor.GetConverter(this.PropertyType).ConvertFromString(this.reference.Value);
		}

		/// <summary>
		/// When overridden in a derived class, resets the value for this property of the component to the default value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be reset to the default value.</param>
		public override void ResetValue(object component)
		{
			this.reference.Value = string.Empty;
		}

		/// <summary>
		/// When overridden in a derived class, sets the value of the component to a different value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be set.</param>
		/// <param name="value">The new value.</param>
		public override void SetValue(object component, object value)
		{
			this.reference.Value = TypeDescriptor.GetConverter(this.PropertyType).ConvertToString(value);
		}

		/// <summary>
		/// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
		/// </summary>
		/// <param name="component">The component with the property to be examined for persistence.</param>
		/// <returns>
		/// True if the property should be persisted; otherwise, false.
		/// </returns>
		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		private static Attribute[] BuildAttributes(IFeatureCompositionService service, Reference reference)
		{
			if (service != null)
			{
				// Find KindProvider for this kind
				var exportedProviderRef = service.GetExports<IReferenceKindProvider, IFeatureComponentMetadata>()
					.Where(provider => provider.Metadata.Id == reference.Kind)
					.Select(provider => provider.Value)
					.FirstOrDefault();

				if (exportedProviderRef != null)
				{
					// Get attributes from Type of Provider
					return exportedProviderRef.GetType().GetCustomAttributes<Attribute>(true).ToArray();
				}
			}

			// Returns default attributes
			return new Attribute[]
				{
					new BrowsableAttribute(true),
					new ReadOnlyAttribute(false),
					new CategoryAttribute(string.Empty),
					new DescriptionAttribute(reference.Kind),
					new DisplayNameAttribute(reference.Kind),
				};
		}
	}
}
