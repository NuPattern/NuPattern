using System;
using System.ComponentModel;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Design
{
    /// <summary>
    /// Defines a custom property descriptor for property that 
    /// contains a reference by id to another settings element
    /// within the same container.
    /// </summary>
    [CLSCompliant(false)]
    public class SettingsReferencePropertyDescriptor<TSettings, TReferenceSetting> : DelegatingPropertyDescriptor
        where TSettings : IAutomationSettings
        where TReferenceSetting : IAutomationSettings
    {
        private PropertyDescriptor innerDescriptor;
        private Func<TSettings, Guid> resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsReferencePropertyDescriptor{TSettings, TReferenceSetting}"/> class.
        /// </summary>
        /// <param name="innerDescriptor">The inner descriptor.</param>
        /// <param name="resolver">The id resolver.</param>
        /// <param name="attributes">The attributes.</param>
        public SettingsReferencePropertyDescriptor(PropertyDescriptor innerDescriptor, Func<TSettings, Guid> resolver, params Attribute[] attributes)
            : base(innerDescriptor, attributes)
        {
            Guard.NotNull(() => innerDescriptor, innerDescriptor);

            this.innerDescriptor = innerDescriptor;
            this.resolver = resolver;
        }

        /// <summary>
        /// Gets the type converter for this property.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.ComponentModel.TypeConverter"/> that is used to convert the <see cref="T:System.Type"/> of this property.</returns>
        public override TypeConverter Converter
        {
            get { return new SettingsReferenceTypeConverter<TReferenceSetting>(); }
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
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            var settings = (IAutomationSettings)component;

            return (from cs in settings.Owner.AutomationSettings
                    let setting = cs.As<TReferenceSetting>()
                    where setting != null && setting.Id == this.resolver((TSettings)settings)
                    select cs.Name)
                    .SingleOrDefault();
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            var settings = (IAutomationSettings)component;

            var id = (from cs in settings.Owner.AutomationSettings
                      where cs.Name.Equals((string)value)
                      let setting = cs.As<TReferenceSetting>()
                      where setting != null
                      select setting.Id)
                      .SingleOrDefault();

            this.innerDescriptor.SetValue(component, id);
        }
    }
}