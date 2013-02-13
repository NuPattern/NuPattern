using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.Runtime
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
        /// Gets the value of the <see cref="ToolkitVisibility"/> attribute.
        /// </summary>
        /// <param name="content">The content element</param>
        /// <param name="attributeName">The name of the expected attribute.</param>
        /// <param name="defaultValue">The default value to use if the attribute is not found</param>
        /// <returns>The value of the attribute if found, else the value of the <paramref name="defaultValue"/> </returns>
        public static ToolkitVisibility GetVisibilityAttributeValue(this IExtensionContent content, string attributeName, ToolkitVisibility defaultValue)
        {
            var attributeValuePair = content.Attributes.Where(attr => attr.Key.Equals(attributeName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (attributeValuePair.Equals(default(KeyValuePair<string, string>)))
            {
                return defaultValue;
            }

            ToolkitVisibility visibilityResult;
            if (Enum.TryParse<ToolkitVisibility>(attributeValuePair.Value, true, out visibilityResult))
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
