using System;

namespace NuPattern.ComponentModel.Design
{
    /// <summary>
    /// Attribute to apply to converters that must be used in the 
    /// design solution itself, rather than only through library/MEF.
    /// </summary>
    public class DesignTypeConverterAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DesignTypeConverterAttribute"/> class.
        /// </summary>
        public DesignTypeConverterAttribute(Type converterType)
        {
            this.ConverterType = converterType;
        }

        /// <summary>
        /// Gets the type of the converter.
        /// </summary>
        public Type ConverterType { get; private set; }
    }
}
