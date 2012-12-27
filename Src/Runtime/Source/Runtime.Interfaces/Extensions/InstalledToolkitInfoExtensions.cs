using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Provides extension methods for working with an <see cref="IInstalledToolkitInfo"/>.
    /// </summary>
    [CLSCompliant(false)]
    public static class InstalledToolkitInfoExtensions
    {
        /// <summary>
        /// Determines whether the specified toolkit contains the schema.
        /// </summary>
        /// <param name="toolkitInfo">The toolkit info.</param>
        /// <param name="product">The product to verify the schema.</param>
        /// <returns>
        /// <c>true</c> if the specified toolkit info contains the schema; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsSchema(this IInstalledToolkitInfo toolkitInfo, IProduct product)
        {
            Guard.NotNull(() => toolkitInfo, toolkitInfo);
            Guard.NotNull(() => product, product);

            return product.ExtensionId.Equals(toolkitInfo.Id, StringComparison.OrdinalIgnoreCase) &&
                toolkitInfo.Schema != null &&
                toolkitInfo.Schema.Pattern != null &&
                product.DefinitionId == toolkitInfo.Schema.Pattern.Id;
        }

        /// <summary>
        /// Retrieves the custom extensions that match the given content type within the extension.
        /// </summary>
        public static IEnumerable<string> GetCustomExtensions(this IInstalledToolkitInfo registration, string contentType)
        {
            Guard.NotNull(() => registration, registration);
            Guard.NotNullOrEmpty(() => contentType, contentType);

            return registration.Extension.Content.Where(
                content => content.ContentTypeName.Equals(contentType, StringComparison.OrdinalIgnoreCase))
                .Select(content => content.RelativePath);
        }

        /// <summary>
        /// Gets the customizable extensions.
        /// </summary>
        /// <param name="registration">The toolkit registration.</param>
        /// <returns>The list of customizable extensions.</returns>
        public static IEnumerable<IExtensionContent> GetCustomizableExtensions(this IInstalledToolkitInfo registration)
        {
            Guard.NotNull(() => registration, registration);

            return registration.Extension.Content.Where(content => IsCustomizable(content));
        }

        /// <summary>
        /// Gets the extension points.
        /// </summary>
        public static IEnumerable<IExtensionPointInfo> GetExtensionPoints(this IInstalledToolkitInfo registration)
        {
            Guard.NotNull(() => registration, registration);

            return registration.Schema.FindAll<IExtensionPointInfo>();
        }

        private static bool IsCustomizable(IExtensionContent content)
        {
            const string DocumentsFolder = "Documentation\\";
            const string IsCustomizableAttributeName = "IsCustomizable";

            var customizable = content.RelativePath.StartsWith(DocumentsFolder, StringComparison.OrdinalIgnoreCase);
            string customizableString;

            if (content.Attributes != null && content.Attributes.TryGetValue(IsCustomizableAttributeName, out customizableString))
            {
                if (bool.TryParse(customizableString, out customizable))
                {
                    return customizable;
                }
            }

            return customizable;
        }
    }
}