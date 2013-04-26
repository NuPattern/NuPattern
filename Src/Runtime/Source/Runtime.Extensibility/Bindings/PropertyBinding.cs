using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// A binding to a property.
    /// </summary>
    public abstract class PropertyBinding
    {
        private readonly ITracer tracer;

        /// <summary>
        /// Creates a new instance of a <see cref="PropertyBinding"/> class.
        /// </summary>
        /// <param name="propertyName"></param>
        protected PropertyBinding(string propertyName)
        {
            this.PropertyName = propertyName;
            this.tracer = Tracer.Get(this.GetType());
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Sets the value of the binding.
        /// </summary>
        /// <param name="target"></param>
        public abstract void SetValue(object target);

        /// <summary>
        /// Sets the value of the binding
        /// </summary>
        protected void SetValue(object target, object value)
        {
            var property = TypeDescriptor.GetProperties(target)
                .Cast<PropertyDescriptor>()
                .FirstOrDefault(d => d.Name == this.PropertyName);

            if (property != null)
            {
                if (value != null && !property.PropertyType.IsAssignableFrom(value.GetType()))
                {
                    if (property.Converter != null)
                    {
                        if (property.Converter.CanConvertFrom(value.GetType()))
                        {
                            property.SetValue(target, property.Converter.ConvertFrom(value));
                        }
                        else
                        {
                            var message = string.Format(CultureInfo.CurrentCulture,
                                Resources.PropertyBinding_PropertyNotCompatible,
                                ObjectDumper.ToString(value, 5), target, this.PropertyName, property.PropertyType, property.Converter);

                            tracer.Error(message);
                            throw new ArgumentException(message);
                        }
                    }
                    else
                    {
                        var message = string.Format(CultureInfo.CurrentCulture,
                            Resources.PropertyBinding_TracePropertyCustomNotCompatible,
                            ObjectDumper.ToString(value, 5), target, this.PropertyName, property.PropertyType);

                        tracer.Error(message);
                        throw new ArgumentException(message);
                    }
                }
                else
                {
                    property.SetValue(target, value);
                }
            }
            else
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.PropertyBinding_TracePropertyNotFound, target, this.PropertyName);
                tracer.Error(message);
                throw new ArgumentException(message);
            }
        }
    }
}