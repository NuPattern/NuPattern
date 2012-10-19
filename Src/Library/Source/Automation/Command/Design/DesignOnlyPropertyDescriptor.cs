using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	class DesignOnlyPropertyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor descriptor;

		public DesignOnlyPropertyDescriptor(PropertyDescriptor descriptor) : base(descriptor.Name, descriptor.Attributes.OfType<Attribute>().ToArray())
		{
			this.descriptor = descriptor;
		}

		public override bool CanResetValue(object component)
		{
			return IsNotDefaultValue(component, false);
		}

		public override Type ComponentType
		{
			get { return descriptor.ComponentType; }
		}

		public override object GetValue(object component)
		{
			var property = GetInternalProperty(component);
			return property.Value;
		}

		public override bool IsReadOnly
		{
			get { return descriptor.IsReadOnly; }
		}

		public override Type PropertyType
		{
			get { return descriptor.PropertyType; }
		}

		public override void ResetValue(object component)
		{
			var defaultValue = Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
			if (defaultValue != null)
			{
				SetValue(component, defaultValue.Value);
			}
		}

		public override void SetValue(object component, object value)
		{
			var property = GetInternalProperty(component);
			property.Value = value;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return IsNotDefaultValue(component, true);
		}

		private bool IsNotDefaultValue(object component, bool defaultReturn)
		{
			var defaultValue = Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
			if (defaultValue == null)
			{
				return defaultReturn;
			}
			else
			{
				return !string.Equals(defaultValue.Value.ToString(), GetValue(component).ToString());
			}
		}

		private DesignProperty GetInternalProperty(object component)
		{
			var designDescriptor = new DesignPropertyDescriptor(
				descriptor.Name,
				string.Empty,
				string.Empty,
				string.Empty,
				descriptor.PropertyType,
				descriptor.ComponentType,
				descriptor.Attributes.OfType<Attribute>().ToArray());

			var property = (DesignProperty)designDescriptor.GetValue(component);
			return property;
		}
	}
}
