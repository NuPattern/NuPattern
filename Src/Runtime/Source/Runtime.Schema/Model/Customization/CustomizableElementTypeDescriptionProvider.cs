using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Provides a custom type descriptor for the <see cref="CustomizableElementSchema" /> class. 
	/// </summary>
	public class CustomizableElementTypeDescriptionProvider : ElementTypeDescriptionProvider
	{
		/// <summary>
		/// Returns an instance of a type descriptor for the given instance of the class.
		/// </summary>
		protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
		{
			if (element is CustomizableElementSchema)
			{
				return new CustomizableElementTypeDescriptor(parent, element);
			}

			return base.CreateTypeDescriptor(parent, element);
		}
	}

	/// <summary>
	/// Provides the custom type descriptor for <see cref="CustomizableElementSchema"/> class, 
	/// that displays customization properties depending on the state of the customizable element.
	/// </summary>
	internal class CustomizableElementTypeDescriptor : NamedElementTypeDescriptor
	{
		internal const string PolicyDisplayedValueFormatter = "({0})";

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomizableElementTypeDescriptor"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="modelElement">The model element.</param>
		public CustomizableElementTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
			: base(parent, modelElement)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomizableElementTypeDescriptor"/> class.
		/// </summary>
		/// <param name="modelElement">The model element.</param>
		public CustomizableElementTypeDescriptor(ModelElement modelElement)
			: base(modelElement)
		{
		}

		/// <summary>
		/// Returns the properties for customization that reflect the current state of the class.
		/// </summary>
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>();
			var element = (CustomizableElementSchema)this.ModelElement;

			// Add descriptor for the policy
			properties = properties.Concat(new[] { GetDescriptorForPolicy(element) });

			// Apply rules for customization
			if (element.IsInheritedFromBase)
			{
				properties = ApplyCustomizationRules(element, properties).Cast<PropertyDescriptor>();
			}

			return new PropertyDescriptorCollection(properties.ToArray());
		}

		/// <summary>
		/// Applies the rules for customization of this element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="properties"></param>
		private static PropertyDescriptorCollection ApplyCustomizationRules(CustomizableElementSchema element, IEnumerable<PropertyDescriptor> properties)
		{
			var descriptors = properties.ToDictionary(property => property.Name);

			// Add read-only attribute to IsCustomizable property, when the element customization is disabled.
			if (!element.IsCustomizationEnabled)
			{
				if (element.Policy != null)
				{
					// Hide the policy
					var policyProperty = descriptors[Reflector<CustomizableElementSchema>.GetProperty(elem => elem.Policy).Name];
					descriptors[policyProperty.Name] = new DelegatingPropertyDescriptor(
						policyProperty, new BrowsableAttribute(false));
				}

				// Element.IsCustomizable property is displayed as read-only to author/tailor with its current value.
				var isCustomizableProperty = descriptors[Reflector<CustomizableElementSchema>.GetProperty(elem => elem.IsCustomizable).Name];
				descriptors[isCustomizableProperty.Name] = new DelegatingPropertyDescriptor(
					isCustomizableProperty,
					new ReadOnlyAttribute(true));
			}

			if (element.Policy != null)
			{
				// Add read-only attribute to the property of the element that the policy.setting refers to, when the setting is not customizable.
				foreach (var disabledSetting in element.Policy.Settings.Where(setting => !setting.IsEnabled))
				{
					// Disable the referred to property of this element
					descriptors[disabledSetting.PropertyId] = new DelegatingPropertyDescriptor(
						descriptors[disabledSetting.PropertyId],
						new ReadOnlyAttribute(true));
				}
			}

			return new PropertyDescriptorCollection(descriptors.Values.ToArray());
		}

		/// <summary>
		/// Returns a descriptor for the customization policy.
		/// </summary>
		/// <returns>
		/// An ExpandableObjectConverter descriptor, that has attributes copied from the CustomizationPolicy relationship and 
		/// the CustomizationPolicy domain class.
		/// </returns>
		private static PropertyDescriptor GetDescriptorForPolicy(CustomizableElementSchema element)
		{
			var attributes = new List<Attribute>();

			//// Get the attributes of the domain role between element and policy.
			var sourceRole = element.GetDomainClass().AllDomainRolesPlayed.Single(
				role => role.DomainRelationship.Name == typeof(CustomizableElementHasPolicy).Name);

			//// Copy meta-attributes from CustomizationPolicy MEL
			var category = typeof(CustomizationPolicySchema).Category();
			if (!string.IsNullOrEmpty(category))
			{
				attributes.Add(new CategoryAttribute(category));
			}

			//// Add presentation attributes
			attributes.Add(new TypeConverterAttribute(typeof(CustomizationPolicyConverter)));

			return new CustomizationPolicyRolePlayerDescriptor(element, sourceRole.OppositeDomainRole, attributes.ToArray());
		}

		/// <summary>
		/// Custom expandable converter to support custom rendering of the policy type.
		/// </summary>
		private class CustomizationPolicyConverter : ExpandableObjectConverter
		{
			/// <summary>
			/// Adds support for converting to string.
			/// </summary>
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof(string))
				{
					return true;
				}

				return base.CanConvertTo(context, destinationType);
			}

			/// <summary>
			/// Converts to string.
			/// </summary>
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (value != null && destinationType == typeof(string))
				{
					return string.Format(CultureInfo.CurrentCulture, CustomizableElementTypeDescriptor.PolicyDisplayedValueFormatter, ((CustomizationPolicySchema)value).CustomizationLevel);
				}

				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		/// <summary>
		/// Custom descriptor to allow resetting all settings to their default values.
		/// </summary>
		private class CustomizationPolicyRolePlayerDescriptor : RolePlayerPropertyDescriptor
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="CustomizationPolicyRolePlayerDescriptor"/> class.
			/// </summary>
			/// <param name="element">The element.</param>
			/// <param name="domainRoleInfo">The domain role info.</param>
			/// <param name="attributes">The attributes.</param>
			public CustomizationPolicyRolePlayerDescriptor(CustomizableElementSchema element, DomainRoleInfo domainRoleInfo, Attribute[] attributes)
				: base(element, domainRoleInfo, attributes)
			{
			}

			/// <summary>
			/// Gets the displayed name for this descriptor.
			/// </summary>
			public override string DisplayName
			{
				get
				{
					return Properties.Resources.CustomizationPolicyRolePlayerDescriptor_DisplayName;
				}
			}

			/// <summary>
			/// Gets the description used for this descriptor.
			/// </summary>
			public override string Description
			{
				get
				{
					return Properties.Resources.CustomizationPolicyRolePlayerDescriptor_Description;
				}
			}

			/// <summary>
			/// Roles can be reset to null if nulling is permitted, the property descriptor is not read-only
			/// and if any existing link is eligible for deletion.
			/// </summary>
			/// <param name="component">The propery object.</param>
			/// <returns>
			/// Overrideable. Returns true unless the property is read-only or the something in the existing links closure is delete locked.
			/// </returns>
			public override bool CanResetValue(object component)
			{
				return true;
			}

			/// <summary>
			/// Reset domain propertyvalue to the default based on the default of the domain.
			/// </summary>
			public override void ResetValue(object component)
			{
				var element = (CustomizableElementSchema)this.GetDescribedElement(component);

				element.Store.TransactionManager.DoWithinTransaction(() =>
				{
					foreach (var setting in element.Policy.Settings)
					{
						setting.Value = setting.DefaultValue;
					}
				});
			}
		}
	}
}