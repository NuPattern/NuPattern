using System;
using System.Globalization;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Definitions for URI's for VSTemplates
    /// </summary>
    public static class VsTemplateUri
    {
        /// <summary>
        /// The scheme of the URI
        /// </summary>
        public const string UriScheme = "template";

        /// <summary>
        /// The format of the URI host 
        /// </summary>
        public const string HostFormat = VsTemplateUri.UriScheme + "://{TemplateType}";

        /// <summary>
        /// Gets the base URI for the given template type.
        /// </summary>
        [CLSCompliant(false)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "The current code base treats URIs as lower case.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Must treat this uri as a string.")]
        public static string GetUriBase(VsTemplateType templateType)
        {
            if (templateType == VsTemplateType.Item ||
                templateType == VsTemplateType.Project ||
                templateType == VsTemplateType.ProjectGroup)
            {
                return HostFormat.NamedFormat(new
                {
                    TemplateType = templateType.ToString().ToLower(CultureInfo.InvariantCulture),
                });
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
