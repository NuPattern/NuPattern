using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Binding
{
    /// <summary>
    /// A custom descriptor that automatically serializes a binding to Json, and provides a 
    /// dropdown picker for the binding type.
    /// </summary>
    [CLSCompliant(false)]
    public class BindingPropertyDescriptor<TValue> : DelegatingPropertyDescriptor
        where TValue : class
    {
        private BindingSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingPropertyDescriptor{T}"/> class with optional 
        /// custom attributes to modify the property behavior.
        /// </summary>
        /// <param name="innerDescriptor">The inner descriptor.</param>
        /// <param name="overriddenAttributes">The optional custom attributes.</param>
        public BindingPropertyDescriptor(PropertyDescriptor innerDescriptor, params Attribute[] overriddenAttributes)
            : base(innerDescriptor, AddStandardValuesEditor(overriddenAttributes))
        {
        }

        /// <summary>
        /// Adds the standard values editor to the property.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        private static Attribute[] AddStandardValuesEditor(Attribute[] attributes)
        {
            return attributes
                .Where(a => !(a is EditorAttribute))
                .Concat(new[] 
				{ 
					new EditorAttribute(typeof(StandardValuesEditor), typeof(UITypeEditor)) 
				}).ToArray();
        }

        /// <summary>
        /// Overrides the converter and provides the fixed one that sets the bound type value.
        /// </summary>
        public override TypeConverter Converter
        {
            get { return new TypeIdConverter(); }
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        public override object GetValue(object component)
        {
            // We cache the settings to avoid paying the serialization cost too often.
            // Also, this allows us to track changes more consistently.
            if (this.settings == null)
            {
                var json = base.GetValue(component) as string;

                // We always initialize the value to some non-null object, which 
                // turns on the dropdown for type selection. If we don't, the 
                // dropdown never shows up.
                if (string.IsNullOrWhiteSpace(json))
                {
                    this.settings = new BindingSettings();
                    // Save the value right after instantiation, so that 
                    // subsequent GetValue gets the same instance and 
                    // does not re-create from scratch.
                    SetValue(component, settings);
                }
                else
                {
                    // Deserialize always to the concrete type, as we the serializer needs to 
                    // know the type of thing to create.
                    this.settings = BindingSerializer.Deserialize<BindingSettings>(json);
                }

                // Hookup property changed event, which supports nested changes as well. This 
                // allows us to automatically save the serialized json on every property change.
                this.settings.PropertyChanged += OnSettingsChanged;
            }

            return this.settings;
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        public override void SetValue(object component, object value)
        {
            var stringValue = value as string;
            var settingsValue = value as BindingSettings;

            // The setter will be called with a plain string value whenever the standard values editor is 
            // used to pick the TypeId. So we need to actually set that value on the binding instead.
            if (stringValue != null)
            {
                // Note that the setting of the property TypeId automatically triggers a property changed 
                // that will cause the value to be serialized and saved :). Indeed, 
                // it will be calling the SetValue again, but this time with the entire binding settings
                // so it will call the next code branch.
                ((BindingSettings)this.GetValue(component)).TypeId = stringValue;
            }
            else if (settingsValue != null)
            {
                // Someone is explicitly setting the entire binding value, so we have to serialize it straight.
                SaveSettings(component, settings);

                // If the previous value was non-null, then we'd be monitoring its property changes 
                // already, and we'd need to unsubscribe.
                if (this.settings != null)
                {
                    this.settings.PropertyChanged -= OnSettingsChanged;
                }

                // Store for reuse on GetValue, and attach to property changes for auto-save.
                this.settings = settingsValue;
                this.settings.PropertyChanged += OnSettingsChanged;
            }
        }

        /// <summary>
        /// When overridden in a class, determines if the vaue of this property can be reset.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            return (this.settings != null && this.settings.IsConfigured());
        }

        /// <summary>
        /// When overridden in a class, resets teh property value.
        /// </summary>
        public override void ResetValue(object component)
        {
            this.settings.Reset();
        }

        private void OnSettingsChanged(object sender, EventArgs args)
        {
            // Automatically save the settings on change.
            SetValue(sender, this.settings);
        }

        private void SaveSettings(object component, BindingSettings settings)
        {
            if (!this.settings.IsConfigured())
            {
                base.SetValue(component, BindingSettings.Empty);
            }
            else
            {
                base.SetValue(component, BindingSerializer.Serialize(settings));
            }
        }

        /// <summary>
        /// Custom converter that customizes the way the binding is shown as a string.
        /// It inherits from the FeatureComponentTypeConverter which already handles 
        /// the initialization of standard values.
        /// </summary>
        private class TypeIdConverter : FeatureComponentTypeConverter<TValue>
        {
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                var settings = value as IBindingSettings;
                if (destinationType == typeof(string) && settings != null)
                {
                    // TODO: Display simplified type name (consistent across all bindings).
                    //if (settings.TypeId.Contains('.'))
                    //    return ((IBindingSettings)value).TypeId.Split('.').Last();
                    return settings.IsConfigured() ? settings.TypeId : BindingSettings.Empty;
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                var settings = (IBindingSettings)value;
                var properties = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

                if (settings.IsConfigured())
                {
                    var extensionType = this.Components.FromFeaturesCatalog()
                        .Where(binding => binding.Metadata.Id == settings.TypeId)
                        .Select(binding => binding.Metadata.ExportingType)
                        .FirstOrDefault();

                    if (extensionType == null && !string.IsNullOrEmpty(settings.TypeId))
                    {
                        extensionType = (from type in this.ProjectTypeProvider.GetTypes<TValue>()
                                         let meta = type.AsProjectFeatureComponent()
                                         where meta != null && meta.Id == settings.TypeId
                                         select type)
                                        .FirstOrDefault();
                    }

                    if (extensionType != null)
                    {
                        foreach (var descriptor in TypeDescriptor.GetProperties(extensionType).Cast<PropertyDescriptor>().Where(d => d.IsPlatuBindableProperty()))
                        {
                            properties.Add(new DesignPropertyDescriptor(
                                descriptor.Name,
                                descriptor.PropertyType,
                                settings.GetType(),
                                descriptor.Attributes.Cast<Attribute>().ToArray()));
                        }
                    }
                }

                return properties;
            }
        }
    }
}
