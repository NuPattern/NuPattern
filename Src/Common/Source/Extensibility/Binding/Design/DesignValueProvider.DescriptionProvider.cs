using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Extensibility.Binding
{
    /// <summary>
    /// Defines a type description provider over <see cref="DesignValueProvider"/>.
    /// </summary>
    internal class DesignValueProviderTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static TypeDescriptionProvider parent = TypeDescriptor.GetProvider(typeof(DesignValueProvider));

        ///// <summary>
        ///// Creates a new instance of the <see cref="DesignValueProviderTypeDescriptionProvider"/> class.
        ///// </summary>
        //public DesignValueProviderTypeDescriptionProvider()
        //    : base(parent)
        //{
        //}

        /// <summary>
        /// Returns the type descriptor for the object type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new DesignValueProviderTypeDescriptor(parent.GetTypeDescriptor(objectType, instance), instance as DesignValueProvider);
        }

        private class DesignValueProviderTypeDescriptor : CustomTypeDescriptor
        {
            private DesignValueProvider designValueProvider;
            private ICustomTypeDescriptor parent;

            /// <summary>
            /// Creates a new instance of the <see cref="DesignValueProviderTypeDescriptor"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="valueProvider">The value provider.</param>
            public DesignValueProviderTypeDescriptor(ICustomTypeDescriptor parent, DesignValueProvider valueProvider)
                : base(parent)
            {
                this.parent = parent;
                this.designValueProvider = valueProvider;
            }

            [ImportMany]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
            private IEnumerable<Lazy<IValueProvider, IFeatureComponentMetadata>> ValueProviders { get; set; }

            [Import]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
            private IPlatuProjectTypeProvider ProjectTypeProvider { get; set; }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = parent.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();
                properties.AddRange(attributes != null
                                        ? base.GetProperties(attributes).Cast<PropertyDescriptor>()
                                        : base.GetProperties().Cast<PropertyDescriptor>());

                if (this.ProjectTypeProvider == null)
                {
                    FeatureCompositionService.Instance.SatisfyImportsOnce(this);
                }

                var valueProviderType = this.ValueProviders.FromFeaturesCatalog()
                    .Where(binding => binding.Metadata.Id == this.designValueProvider.Name)
                    .Select(binding => binding.Metadata.ExportingType)
                    .FirstOrDefault();

                if (valueProviderType == null && !string.IsNullOrEmpty(this.designValueProvider.Name))
                {
                    valueProviderType = (from type in this.ProjectTypeProvider.GetTypes<IValueProvider>()
                                         let meta = type.AsProjectFeatureComponent()
                                         where meta != null && meta.Id == this.designValueProvider.Name
                                         select type)
                                        .FirstOrDefault();
                }

                if (valueProviderType != null)
                {
                    foreach (var descriptor in TypeDescriptor.GetProperties(valueProviderType).Cast<PropertyDescriptor>().Where(d => d.IsBindableDesignProperty()))
                    {
                        // Determine if a collection property
                        if (descriptor.IsPropertyTypeGeneric(typeof(Collection<>)))
                        {
                            properties.Add(new DesignCollectionPropertyDescriptor<DesignValueProvider>(descriptor));
                        }
                        else
                        {
                            properties.Add(new DesignPropertyDescriptor(
                                               descriptor.Name,
                                               descriptor.PropertyType,
                                               typeof(DesignValueProvider),
                                               descriptor.Attributes.Cast<Attribute>().ToArray()));
                        }
                    }
                }

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }
    }
}