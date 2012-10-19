using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Binding
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

            IPropertyBindingSettings propertySettings = null;
            // This check for the type of component is here because 
            // we use the same descriptor for both properties on the 
            // condition model as well as on the value provider.
            var settings = component as IBindingSettings;
            if (settings != null)
            {
                propertySettings = settings.Properties.FirstOrDefault(prop => prop.Name == this.Name);
                if (propertySettings == null)
                {
                    propertySettings = new PropertyBindingSettings { Name = this.Name };
                    settings.Properties.Add(propertySettings);
                }
            }
            else
            {
                var design = component as DesignValueProvider;
                if (design != null)
                {
                    propertySettings = design.ValueProvider.Properties.FirstOrDefault(prop => prop.Name == this.Name);
                    if (propertySettings == null)
                    {
                        propertySettings = new PropertyBindingSettings { Name = this.Name };
                        design.ValueProvider.Properties.Add(propertySettings);
                    }
                }
            }

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
    }
}