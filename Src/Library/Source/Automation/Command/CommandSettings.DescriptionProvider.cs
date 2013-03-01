using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Binding;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Defines a <see cref="TypeDescriptionProvider"/> over <see cref="CommandSettings"/>.
    /// </summary>
    public class CommandSettingsTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Overridables for the derived class to provide a custom type descriptor.
        /// </summary>
        /// <param name="parent">Parent custom type descriptor.</param>
        /// <param name="element">Element to be described.</param>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            return new CommandSettingsTypeDescriptor(parent, element);
        }

        /// <summary>
        /// Defines a type descriptor over <see cref="CommandSettings"/>.
        /// </summary>
        internal class CommandSettingsTypeDescriptor : ElementTypeDescriptor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandSettingsTypeDescriptor"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="selectedElement">The selected element.</param>
            public CommandSettingsTypeDescriptor(ICustomTypeDescriptor parent, ModelElement selectedElement)
                : base(parent, selectedElement)
            {
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
            static internal IEnumerable<Lazy<IFeatureCommand, IFeatureComponentMetadata>> Components { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
            static internal IPlatuProjectTypeProvider ProjectTypeProvider { get; set; }

            /// <summary>
            /// Returns the properties for this instance of a component.
            /// </summary>
            /// <returns>
            /// A PropertyDescriptorCollection that represents the properties for this component instance.
            /// </returns>
            public override PropertyDescriptorCollection GetProperties()
            {
                return this.GetProperties(new Attribute[0]);
            }

            /// <summary>
            /// Returns the properties for this instance of a component using the attribute array as a filter.
            /// </summary>
            /// <param name="attributes">An array of type Attribute that is used as a filter.</param>
            /// <returns>
            /// An array of type Attribute that represents the properties for this component instance that match the given set of attributes.
            /// </returns>
            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

                var commandSettings = (CommandSettings)this.ModelElement;

                if (ProjectTypeProvider == null || Components == null)
                {
                    ProjectTypeProvider = commandSettings.Store.GetService<IPlatuProjectTypeProvider>();
                    Components = commandSettings.Store.GetService<IFeatureCompositionService>().GetExports<IFeatureCommand, IFeatureComponentMetadata>();
                }

                var extensionType = Components
                    .FromFeaturesCatalog()
                    .Where(binding => binding.Metadata.Id == commandSettings.TypeId)
                    .Select(binding => binding.Metadata.ExportingType)
                    .FirstOrDefault();

                if (extensionType == null && !string.IsNullOrEmpty(commandSettings.TypeId))
                {
                    extensionType = (from type in ProjectTypeProvider.GetTypes<IFeatureCommand>()
                                     let meta = type.AsProjectFeatureComponent()
                                     where meta != null && meta.Id == commandSettings.TypeId
                                     select type)
                                    .FirstOrDefault();
                }

                if (extensionType != null)
                {
                    foreach (var descriptor in TypeDescriptor.GetProperties(extensionType).Cast<PropertyDescriptor>().Where(d => d.IsBindableDesignProperty()))
                    {
                        // Determine if a collection property
                        if (descriptor.IsPropertyTypeGeneric(typeof(Collection<>)))
                        {
                            properties.Add(new DesignCollectionPropertyDescriptor<CommandSettings>(descriptor));
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
                                // Browsable attribute will already be honored.
                                properties.Add(new DesignPropertyDescriptor(
                                    descriptor.Name,
                                    descriptor.PropertyType,
                                    this.ModelElement.GetType(),
                                    descriptor.Attributes.Cast<Attribute>().ToArray()));
                            }
                        }
                    }
                }

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }
    }
}