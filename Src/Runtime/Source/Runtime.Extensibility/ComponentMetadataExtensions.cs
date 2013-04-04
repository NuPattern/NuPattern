using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Extensions to types for feature extensions.
    /// </summary>
    [CLSCompliant(false)]
    public static class ComponentMetadataExtensions
    {
        /// <summary>
        /// Returns the component metadata
        /// </summary>
        public static IFeatureComponentMetadata AsProjectFeatureComponent(this Type componentType)
        {
            var customAttribute = componentType.FeatureComponentDynamic();
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

        private static FeatureComponentAttribute FeatureComponentDynamic(this Type component)
        {
            // The dynamic version is necessary when the component type was obtained using the type service
            foreach (var attribute in component.GetCustomAttributes(true))
            {
                if (IsFeatureComponentMetadata(attribute.GetType()))
                {
                    var featureComponentAttribute = new FeatureComponentAttribute();

                    foreach (var property in attribute.GetType().GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.GetProperty |
                        BindingFlags.SetProperty))
                    {
                        var featureComponentAttributeProperty = featureComponentAttribute.GetType().GetProperty(property.Name);
                        if (featureComponentAttributeProperty != null && featureComponentAttributeProperty.CanWrite)
                            featureComponentAttributeProperty.SetValue(featureComponentAttribute, property.GetValue(attribute, null), null);
                    }

                    return featureComponentAttribute;
                }
            }

            return null;
        }

        private static bool IsFeatureComponentMetadata(Type attributeType)
        {
            if (attributeType == null) return false;

            if (attributeType.AssemblyQualifiedName == typeof(FeatureComponentAttribute).AssemblyQualifiedName)
                return true;
            else
                return IsFeatureComponentMetadata(attributeType.BaseType);
        }
    }
}
