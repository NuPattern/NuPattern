using System;
using System.ComponentModel;
using System.Linq;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Binding
{
    internal class DesignPropertyDescriptor : PropertyDescriptor
    {
        private Type componentType;
        private Type propertyType;

        public DesignPropertyDescriptor(string name, Type valueType, Type componentType, Attribute[] attrs)
            : base(name, attrs)
        {
            this.componentType = componentType;
            this.propertyType = Type.GetType(valueType.AssemblyQualifiedName) ?? TypeDescriptor.GetProvider(valueType).GetRuntimeType(valueType);
        }

        public override bool CanResetValue(object component)
        {
            //TODO: Get access to nested design property and calculate whether it is configured.
            return false;
        }

        public override Type ComponentType
        {
            get { return this.componentType; }
        }

        public override bool SupportsChangeEvents
        {
            get { return true; }
        }

        public override object GetValue(object component)
        {
            //TODO: Display instructional text to user when unconfigured.
            //i.e. !propertySettings.IsConfigured => "(Expand to modify)"

            var propertySettings = EnsurePropertySettings(component, this.Name, this.propertyType);
            return new DesignProperty(propertySettings)
            {
                Type = this.propertyType,
                Attributes = this.AttributeArray
            };
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(DesignProperty); }
        }

        public override void SetValue(object component, object value)
        {
            //Ignore value, cant be set at this level.
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void ResetValue(object component)
        {
            //TODO: get access to nested design property and reset it
        }

        internal static IPropertyBindingSettings EnsurePropertySettings(object component, string propertyName, Type propertyType)
        {
            // This check for the type of component is here because 
            // we use the same descriptor for both properties on the 
            // condition model as well as on the value provider.
            IPropertyBindingSettings propertySettings = null;
            var settings = component as IBindingSettings;
            if (settings != null)
            {
                propertySettings = settings.Properties.FirstOrDefault(prop => prop.Name == propertyName);
                if (propertySettings == null)
                {
                    propertySettings = CreatePropertySettings(settings, propertyName, propertyType);
                    settings.Properties.Add(propertySettings);
                }
            }
            else
            {
                var design = component as DesignValueProvider;
                if (design != null)
                {
                    propertySettings = design.ValueProvider.Properties.FirstOrDefault(prop => prop.Name == propertyName);
                    if (propertySettings == null)
                    {
                        propertySettings = CreatePropertySettings(design, propertyName, propertyType);
                        design.ValueProvider.Properties.Add(propertySettings);
                    }
                }
            }

            return propertySettings;
        }

        private static PropertyBindingSettings CreatePropertySettings(object component, string name, Type propertyType)
        {
            // Get the default value of the instance
            var converter = TypeDescriptor.GetConverter(propertyType);
            var defaultValue = (string)null;
            var descriptor = TypeDescriptor.GetProperties(component).Cast<PropertyDescriptor>().FirstOrDefault(p => p.Name == name);
            if (descriptor != null)
            {
                var defaultValueAttribute = descriptor.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
                if (defaultValueAttribute != null)
                {
                    defaultValue = converter.ConvertToString(defaultValueAttribute.Value);
                }
            }

            // Create new property
            return new PropertyBindingSettings
            {
                Name = name,
                Value = (!String.IsNullOrEmpty(defaultValue)) ? defaultValue : null,
            };
        }
    }
}