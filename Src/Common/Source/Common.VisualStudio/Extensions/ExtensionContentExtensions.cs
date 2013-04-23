using System;
using System.Collections.Generic;
using System.Linq;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="IExtensionContent"/> interface.
    /// </summary>
    [CLSCompliant(false)]
    public static class ExtensionContentExtensions
    {
        /// <summary>
        /// Gets the value of the attribute
        /// </summary>
        /// <param name="content">The content element</param>
        /// <param name="attributeName">The name of the expected attribute.</param>
        /// <param name="defaultValue">The default value to use if the attribute is not found</param>
        /// <returns>The value of the attribute if found, else the value of the <paramref name="defaultValue"/> </returns>
        public static string GetCustomAttributeValue(this IExtensionContent content, string attributeName, string defaultValue = "")
        {
            var attributeValuePair = content.Attributes.Where(attr => attr.Key.Equals(attributeName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (attributeValuePair.Equals(default(KeyValuePair<string, string>)))
            {
                return defaultValue;
            }
            else
            {
                return attributeValuePair.Value;
            }
        }

        /// <summary>
        /// Gets the value of the <see cref="ExtensionVisibility"/> attribute.
        /// </summary>
        /// <param name="content">The content element</param>
        /// <param name="attributeName">The name of the expected attribute.</param>
        /// <param name="defaultValue">The default value to use if the attribute is not found</param>
        /// <returns>The value of the attribute if found, else the value of the <paramref name="defaultValue"/> </returns>
        public static ExtensionVisibility GetVisibilityAttributeValue(this IExtensionContent content, string attributeName, ExtensionVisibility defaultValue)
        {
            var attributeValuePair = content.Attributes.FirstOrDefault(attr => attr.Key.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
            if (attributeValuePair.Equals(default(KeyValuePair<string, string>)))
            {
                return defaultValue;
            }

            ExtensionVisibility visibilityResult;
            if (Enum.TryParse<ExtensionVisibility>(attributeValuePair.Value, true, out visibilityResult))
            {
                return visibilityResult;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
