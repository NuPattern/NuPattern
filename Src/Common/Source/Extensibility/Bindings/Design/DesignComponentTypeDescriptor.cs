using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.ComponentModel;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Bindings.Design
{
    /// <summary>
    /// A <see cref="TypeDescriptor"/> for displaying the configured properties for a component type.
    /// </summary>
    public abstract class DesignComponentTypeDescriptor<TComponents, TInstance> : CustomTypeDescriptor
        where TComponents : class
        where TInstance : class
    {
        private ICustomTypeDescriptor parent;

        /// <summary>
        /// Creates a new instance of the <see cref="DesignComponentTypeDescriptor{TComponents, TInstance}"/> class.
        /// </summary>
        /// <param name="parent">The parent descriptor</param>
        protected DesignComponentTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets the parent descriptor.
        /// </summary>
        protected ICustomTypeDescriptor Parent
        {
            get { return this.parent; }
        }


        [ImportMany]
        private IEnumerable<Lazy<TComponents, IFeatureComponentMetadata>> Components { get; set; }

        [Import]
        private INuPatternProjectTypeProvider ProjectTypeProvider { get; set; }

        /// <summary>
        /// Gets the properties for the descriptor.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (this.ProjectTypeProvider == null)
            {
                FeatureCompositionService.Instance.SatisfyImportsOnce(this);
            }

            var properties = this.Parent.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

            // Add base properties
            //properties.AddRange(attributes != null
            //                        ? base.GetProperties(attributes).Cast<PropertyDescriptor>()
            //                        : base.GetProperties().Cast<PropertyDescriptor>());

            // Add component properties
            var extensionName = GetExtensionName();
            properties.AddRange(GetComponentProperties(this.ProjectTypeProvider, this.Components, extensionName));

            return new PropertyDescriptorCollection(properties.ToArray());
        }

        /// <summary>
        /// Returns the descriptors for the nested properties of the component type.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<PropertyDescriptor> GetComponentProperties(INuPatternProjectTypeProvider provider, IEnumerable<Lazy<TComponents, IFeatureComponentMetadata>> components, string extensionName)
        {
            Guard.NotNull(() => provider, provider);
            Guard.NotNull(() => components, components);

            var properties = new List<PropertyDescriptor>();

            if (!String.IsNullOrEmpty(extensionName))
            {
                // Find the extension type.
                var extensionType = GetExtensionType(provider, components, extensionName);
                if (extensionType != null)
                {
                    // Project descriptors from component onto the current instance.
                    var descriptors = TypeDescriptor.GetProperties(extensionType)
                                                    .Cast<PropertyDescriptor>()
                                                    .Where(d => d.IsBindableDesignProperty());
                    foreach (var descriptor in descriptors)
                    {
                        // Determine if a collection design property
                        if (descriptor.IsAutoDesignCollectionProperty())
                        {
                            properties.Add(new DesignCollectionPropertyDescriptor<TInstance>(descriptor));
                        }
                        else
                        {
                            // Determine if a [DesignOnly(true)] property
                            if (descriptor.IsDesignOnlyProperty())
                            {
                                properties.Add(new DesignOnlyPropertyDescriptor(descriptor));
                            }
                            else
                            {
                                // Regular design property
                                properties.Add(new DesignPropertyDescriptor(
                                                   descriptor.Name,
                                                   descriptor.PropertyType,
                                                   typeof(TInstance),
                                                   descriptor.Attributes.Cast<Attribute>().ToArray()));
                            }
                        }
                    }
                }
            }

            return properties;
        }

        /// <summary>
        /// Get the name of the extension.
        /// </summary>
        protected abstract string GetExtensionName();

        private static Type GetExtensionType(INuPatternProjectTypeProvider provider, IEnumerable<Lazy<TComponents, IFeatureComponentMetadata>> components, string extensionName)
        {
            if (string.IsNullOrEmpty(extensionName))
            {
                return null;
            }

            // Locate type from exports first
            var extensionType = components.FromFeaturesCatalog()
                                    .Where(binding => binding.Metadata.Id == extensionName)
                                    .Select(binding => binding.Metadata.ExportingType)
                                    .FirstOrDefault();
            if (extensionType == null)
            {
                if (provider == null)
                {
                    return null;
                }

                // Locate type in solution assemblies
                extensionType = (from type in provider.GetTypes<TComponents>()
                                 let meta = type.AsProjectFeatureComponent()
                                 where meta != null && meta.Id == extensionName
                                 select type)
                                .FirstOrDefault();
            }

            return extensionType;
        }

    }
}
