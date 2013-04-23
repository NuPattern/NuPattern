using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NuPattern.ComponentModel.Composition;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Extensions to types for guidance extensions.
    /// </summary>
    [CLSCompliant(false)]
    public static class ComponentMetadataExtensions
    {
        /// <summary>
        /// Returns the component metadata
        /// </summary>
        public static IComponentMetadata AsProjectComponent(this Type componentType)
        {
            var customAttribute = componentType.ComponentDynamic();
            if (customAttribute == null)
                return null;

            if (string.IsNullOrEmpty(customAttribute.Id))
                customAttribute.Id = componentType.FullName;

            if (string.IsNullOrEmpty(customAttribute.Category))
            {
                customAttribute.Category = componentType
                    .GetCustomAttributes(true)
                    .Where(x => x.GetType().IsAssignableTo(typeof(CategoryAttribute)))
                    .Select(x => ((dynamic)x).Category)
                    .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(customAttribute.Category))
                customAttribute.Category = Resources.DefaultCategory;

            if (string.IsNullOrEmpty(customAttribute.Description))
            {
                customAttribute.Description = componentType
                    .GetCustomAttributes(true)
                    .Where(x => x.GetType().IsAssignableTo(typeof(DescriptionAttribute)))
                    .Select(x => ((dynamic)x).Description)
                    .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(customAttribute.DisplayName))
            {
                customAttribute.DisplayName = componentType
                    .GetCustomAttributes(true)
                    .Where(x => x.GetType().IsAssignableTo(typeof(DisplayNameAttribute)))
                    .Select(x => ((dynamic)x).DisplayName)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(customAttribute.DisplayName))
                    customAttribute.DisplayName = componentType.Name;
            }

            return customAttribute;
        }

        private static ComponentAttribute ComponentDynamic(this Type component)
        {
            // The dynamic version is necessary when the component type was obtained using the type service
            foreach (var attribute in component.GetCustomAttributes(true))
            {
                if (IsComponentMetadata(attribute.GetType()))
                {
                    var componentAttribute = new ComponentAttribute();

                    foreach (var property in attribute.GetType().GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.GetProperty |
                        BindingFlags.SetProperty))
                    {
                        var componentAttributeProperty = componentAttribute.GetType().GetProperty(property.Name);
                        if (componentAttributeProperty != null && componentAttributeProperty.CanWrite)
                            componentAttributeProperty.SetValue(componentAttribute, property.GetValue(attribute, null), null);
                    }

                    return componentAttribute;
                }
            }

            return null;
        }

        private static bool IsComponentMetadata(Type attributeType)
        {
            if (attributeType == null) return false;

            if (attributeType.AssemblyQualifiedName == typeof(ComponentAttribute).AssemblyQualifiedName)
                return true;
            else
                return IsComponentMetadata(attributeType.BaseType);
        }
    }
}
