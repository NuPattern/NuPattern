using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Reflection;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Store.Properties;

namespace NuPattern.Runtime.Store
{
    internal class ProductElementTypeDescriptor : ElementTypeDescriptor
    {
        private ICustomTypeDescriptor parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductElementTypeDescriptor"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="modelElement">The model element.</param>
        public ProductElementTypeDescriptor(ICustomTypeDescriptor parent, ModelElement modelElement)
            : base(parent, modelElement)
        {
            this.parent = parent;
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var element = (ProductElement)this.ModelElement;

            var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>()
                .Concat(new[] { this.GetDescriptorForReferences() })
                .Concat(
                    from prop in element.Properties
                    where prop.Info != null
                    select prop.Descriptor);

            return new PropertyDescriptorCollection(properties.ToArray());
        }

        private PropertyDescriptor GetDescriptorForReferences()
        {
            var referencesName = Reflector<ProductElement>.GetPropertyName(c => c.References);
            return this.parent.GetProperties().Find(referencesName, false);
        }

        /// <summary>
        /// The TypeConverter for the References collection
        /// </summary>
        internal class ReferencesTypeConverter : ExpandableObjectConverter
        {
            /// <summary>
            /// Adds support for converting to string.
            /// </summary>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string) ? true : base.CanConvertTo(context, destinationType);
            }

            /// <summary>
            /// Displays the text of the converter
            /// </summary>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return destinationType == typeof(string) ?
                    Resources.ProductElementTypeDescriptor_ToString :
                    base.ConvertTo(context, culture, value, destinationType);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                if (context != null)
                {
                    var container = context.Instance as ProductElement;
                    if (container != null)
                    {
                        var service = context.GetService<IBindingCompositionService>();
                        return new PropertyDescriptorCollection(
                            container.References.Select(refer => new ReferencePropertyDescriptor(service, refer)).ToArray());
                    }
                }

                return null;
            }
        }
    }
}