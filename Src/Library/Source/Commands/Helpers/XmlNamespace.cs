
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// An XML namespace declaration.
    /// </summary>
    [DisplayNameResource("XmlNamespace_DisplayName", typeof(Resources))]
    [DescriptionResource("XmlNamespace_Description", typeof(Resources))]
    public class XmlNamespace
    {
        /// <summary>
        /// The prefix of the namespace
        /// </summary>
        [DisplayNameResource("XmlNamespace_Prefix_DisplayName", typeof(Resources))]
        [DescriptionResource("XmlNamespace_Prefix_Description", typeof(Resources))]
        public string Prefix { get; set; }

        /// <summary>
        /// The full namespace
        /// </summary>
        [DisplayNameResource("XmlNamespace_Namespace_DisplayName", typeof(Resources))]
        [DescriptionResource("XmlNamespace_Namespace_Description", typeof(Resources))]
        public string Namespace { get; set; }

        /// <summary>
        /// Returns the string representation of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Namespace))
            {
                return Resources.XmlNamespace_EmptyText;
            }

            return string.Format(CultureInfo.CurrentCulture, "xmlns{2}{0}={1}", this.Prefix, this.Namespace, (!string.IsNullOrEmpty(this.Prefix) ? ":" : ""));
        }
    }

    /// <summary>
    /// Extension for the <see cref="XmlNamespace"/> class.
    /// </summary>
    public static class XmlNamespaceExtensions
    {
        /// <summary>
        /// Returns the namespace collection as a dictionary.
        /// </summary>
        /// <param name="namespaces">The namesapces collection.</param>
        /// <returns></returns>
        public static IDictionary<string, string> ToDictionary(this IEnumerable<XmlNamespace> namespaces)
        {
            return namespaces
                .Distinct(new SelectorEqualityComparer<XmlNamespace, string>(prop => prop.Prefix))
                .ToDictionary(ns => ns.Prefix, ns => ns.Namespace);


        }
    }
}
