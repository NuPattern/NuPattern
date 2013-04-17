using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NuPattern.Properties;

namespace NuPattern.ComponentModel
{
    /// <summary>
    /// Provides extensions for <see cref="Type"/> to access the  component metadata 
    /// and specify its defaults.
    /// </summary>
    public static class BananaComponentTypeExtensions
    {
        /// <summary>
        /// Returns the <see cref="IBananaComponentMetadata"/> of the type if available, 
        /// <see langword="null"/> otherwise (i.e. the type does not have a 
        /// <see cref="BananaComponentAttribute"/> attribute or a derived one applied).
        /// </summary>
        public static IBananaComponentMetadata AsComponent(this Type componentType)
        {
            var metadata = componentType.GetCustomAttribute<BananaComponentAttribute>();
            if (metadata == null)
                metadata = componentType.ComponentDynamic();

            if (metadata == null)
                return null;

            if (string.IsNullOrEmpty(metadata.Id))
                metadata.Id = componentType.FullName;

            if (metadata.ExportingType == null)
                metadata.ExportingType = componentType;

            if (string.IsNullOrEmpty(metadata.Category))
                metadata.Category = TypeDescriptor.GetAttributes(componentType)
                    .OfType<CategoryAttribute>()
                    .Select(attribute => attribute.Category)
                    .FirstOrDefault();

            if (string.IsNullOrEmpty(metadata.Category))
                metadata.Category = Resources.ComponentAttribute_DefaultCategory;

            if (string.IsNullOrEmpty(metadata.Description))
                metadata.Description = TypeDescriptor.GetAttributes(componentType)
                    .OfType<System.ComponentModel.DescriptionAttribute>()
                    .Select(attribute => attribute.Description)
                    .FirstOrDefault();

            if (string.IsNullOrEmpty(metadata.DisplayName))
            {
                var displayName = TypeDescriptor.GetAttributes(componentType)
                    .OfType<DisplayNameAttribute>()
                    .Select(attribute => attribute.DisplayName)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(displayName))
                    displayName = componentType.Name;

                metadata.DisplayName = displayName;
            }

            return metadata;
        }

        private static BananaComponentAttribute ComponentDynamic(this Type component)
        {
            // The dynamic version is necessary when the component type was obtained using the DynamicTypeService
            // So the component will be in another (temp) assembly and the cast to IComponentMetadata won't work.

            foreach (Attribute attribute in component.GetCustomAttributes(true))
            {
                if (IsComponentMetadata(attribute.GetType()))
                {
                    var ComponentAttribute = new BananaComponentAttribute();

                    foreach (var property in attribute.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty))
                    {
                        var ComponentAttributeProperty = ComponentAttribute.GetType().GetProperty(property.Name);
                        if (ComponentAttributeProperty != null && ComponentAttributeProperty.CanWrite)
                            ComponentAttributeProperty.SetValue(ComponentAttribute, property.GetValue(attribute, null), null);
                    }

                    return ComponentAttribute;
                }
            }

            return null;
        }

        private static bool IsComponentMetadata(Type attributeType)
        {
            if (attributeType == null) return false;

            if (attributeType.AssemblyQualifiedName == typeof(BananaComponentAttribute).AssemblyQualifiedName)
                return true;
            else
                return IsComponentMetadata(attributeType.BaseType);
        }
    }
}
