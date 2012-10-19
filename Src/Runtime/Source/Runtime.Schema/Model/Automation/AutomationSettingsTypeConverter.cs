using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Type converter thats renders an automation extension plus it settings.
	/// </summary>
	public class AutomationSettingsTypeConverter : ExpandableObjectConverter
	{
		/// <summary>
		/// Gets a collection of properties for the type of object specified by the value parameter.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="value">An <see cref="T:System.Object"/> that specifies the type of object to get the properties for.</param>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that will be used as a filter.</param>
		/// <returns>
		/// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for the component, or null if there are no properties.
		/// </returns>
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			var element = (ModelElement)value;
			var propertyDescriptors = base.GetProperties(context, value, attributes).Cast<PropertyDescriptor>().ToList();

			AddOppositeRolePlayerProperties(
				element,
				element.GetDomainClass().AllDomainRolesPlayed.Where(r => r.DomainRelationship.IsEmbedding),
				propertyDescriptors,
				new TypeConverterAttribute(typeof(ExpandableObjectConverter)));

			return new PropertyDescriptorCollection(propertyDescriptors.ToArray());
		}

		private static void AddOppositeRolePlayerProperties(ModelElement element, IEnumerable<DomainRoleInfo> domainRoles, List<PropertyDescriptor> propertyDescriptors, params Attribute[] additionalAttributes)
		{
			foreach (var sourceRole in domainRoles)
			{
				if (sourceRole.IsOne)
				{
					var descriptor = CreateRolePlayerPropertyDescriptor(
						element, sourceRole.OppositeDomainRole, GetRolePlayerPropertyAttributes(sourceRole).Concat(additionalAttributes).ToArray());

					if (descriptor != null)
					{
						propertyDescriptors.Add(descriptor);
					}
				}
			}
		}

		private static RolePlayerPropertyDescriptor CreateRolePlayerPropertyDescriptor(ModelElement element, DomainRoleInfo targetRoleInfo, Attribute[] sourceDomainRoleInfoAttributes)
		{
			return new RolePlayerPropertyDescriptor(element, targetRoleInfo, sourceDomainRoleInfoAttributes);
		}

		private static Attribute[] GetRolePlayerPropertyAttributes(DomainRoleInfo domainRole)
		{
			return domainRole.LinkPropertyInfo.GetCustomAttributes(false).OfType<Attribute>().ToArray();
		}
	}
}