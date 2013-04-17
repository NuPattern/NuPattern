using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern;
using NuPattern.Diagnostics;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// A binding to a property.
    /// </summary>
    public abstract class PropertyBinding
    {
        private readonly ITraceSource tracer;

        /// <summary>
        /// Creates a new instance of a <see cref="PropertyBinding"/> class.
        /// </summary>
        /// <param name="propertyName"></param>
        protected PropertyBinding(string propertyName)
        {
            this.PropertyName = propertyName;
            this.tracer = Tracer.GetSourceFor(this.GetType());
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
                                "Provided value {0} is not compatible with property {1}.{2} of type {3}, and specific type conversion is not supported by provided converter {4}.",
                                ObjectDumper.ToString(value, 5), target, this.PropertyName, property.PropertyType, property.Converter);

                            tracer.TraceError(message);
                            throw new ArgumentException(message);
                        }
                    }
                    else
                    {
                        var message = string.Format(CultureInfo.CurrentCulture,
                            "Provided value {0} is not compatible with property {1}.{2} of type {3}, and a custom type conversion is not provided.",
                            ObjectDumper.ToString(value, 5), target, this.PropertyName, property.PropertyType);

                        tracer.TraceError(message);
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
                var message = string.Format(CultureInfo.CurrentCulture, "Property {0}.{1} not found", target, this.PropertyName);
                tracer.TraceError(message);
                throw new ArgumentException(message);
            }
        }
    }
}