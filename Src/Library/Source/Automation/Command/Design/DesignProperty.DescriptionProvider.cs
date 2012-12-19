using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Defines a <see cref="TypeDescriptionProvider"/> over <see cref="DesignProperty"/>.
    /// </summary>
    internal class DesignPropertyTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static TypeDescriptionProvider parent = TypeDescriptor.GetProvider(typeof(DesignProperty));

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignPropertyTypeDescriptionProvider"/> class.
        /// </summary>
        public DesignPropertyTypeDescriptionProvider()
            : base(parent)
        {
        }

        /// <summary>
        /// Gets a custom type descriptor for the given type and object.
        /// </summary>
        /// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
        /// <param name="instance">An instance of the type. Can be null if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor"/>.</param>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.ICustomTypeDescriptor"/> that can provide metadata for the type.
        /// </returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new DesignPropertyDescriptor(parent.GetTypeDescriptor(objectType, instance), instance as DesignProperty);
        }

        /// <summary>
        /// Defines <see cref="PropertyDescriptor"/> over <see cref="DesignProperty"/>.
        /// </summary>
        private class DesignPropertyDescriptor : CustomTypeDescriptor
        {
            private DesignProperty designProperty;
            private ICustomTypeDescriptor parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="DesignPropertyDescriptor"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="designProperty">The design property.</param>
            public DesignPropertyDescriptor(ICustomTypeDescriptor parent, DesignProperty designProperty)
                : base(parent)
            {
                this.parent = parent;
                this.designProperty = designProperty;
            }

            /// <summary>
            /// Returns a filtered collection of property descriptors for the object represented by this type descriptor.
            /// </summary>
            /// <param name="attributes">An array of attributes to use as a filter. This can be null.</param>
            /// <returns>
            /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"/>.
            /// </returns>
            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Filter errors")]
            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = this.parent.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

                try
                {
                    TypeConverter converter = null;
                    var typeConverterAttribute = this.designProperty.Attributes.OfType<TypeConverterAttribute>().FirstOrDefault();

                    if (typeConverterAttribute != null)
                    {
                        var converterType = Type.GetType(typeConverterAttribute.ConverterTypeName);

                        if (converterType != null)
                        {
                            converter = (TypeConverter)Activator.CreateInstance(converterType);
                        }
                    }

					if (converter == null)
					{
						//Fallback to property type
						converter = TypeDescriptor.GetConverter(this.designProperty.Type);
					}

                    if (converter != null && converter.CanConvertFrom(typeof(string)))
                    {
                        properties.Add(new DesignPropertyValueDescriptor(
							this.designProperty.ModelProperty.Name, 
							this.designProperty.Type, 
							converter,
							this.designProperty.Attributes));
                    }
                }
                catch
                {
                    // The propety value can't be added.
                    // Value provider should be used instead
                }

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }
    }
}