using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Authoring.WorkflowDesign
{
    /// <summary>
    /// Customizations for the NamedElementSchema class.
    /// </summary>
    [TypeDescriptionProvider(typeof(NamedElementTypeDescriptionProvider))]
    partial class NamedElementSchema
    {
        internal const string SchemaPathDelimiter = ".";

        /// <summary>
        /// Returns a spaced string representation for the given pascal cased string.
        /// </summary>
        internal static string MakePascalIntoWords(string potentialPascal)
        {
            if (String.IsNullOrEmpty(potentialPascal))
            {
                return potentialPascal;
            }

            if (char.IsLower(potentialPascal[0]))
            {
                potentialPascal = char.ToUpper(potentialPascal[0], CultureInfo.CurrentCulture) + potentialPascal.Substring(1);
            }

            return Regex.Replace(potentialPascal, @"(?<low>[^\p{Lu}\p{Lt}\p{Nl}])(?<hi>[\p{Lu}\p{Lt}\p{Nl}])", "${low} ${hi}");
        }

        /// <summary>
        /// Provide default name of element, based upon the DisplayName property.
        /// </summary>
        protected override void MergeConfigure(ElementGroup elementGroup)
        {
            if (DomainClassInfo.HasNameProperty(this))
            {
                DomainClassInfo.SetUniqueName(this, SanitizeName(this.GetDomainClass().DisplayName));
            }

            base.MergeConfigure(elementGroup);
        }

        /// <summary>
        /// Makes a name from the given value.
        /// </summary>
        private static string SanitizeName(string value)
        {
            Guard.NotNull(() => value, value);

            return value.Replace(" ", string.Empty);
        }

        /// <summary>
        /// Returns the calculated value of the DisplayName property while being tracked.
        /// </summary>
        internal string CalculateDisplayNameTrackingValue()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return string.Empty;
            }

            // Separate pascal cased words in the string
            return MakePascalIntoWords(this.Name);
        }

        /// <summary>
        /// Gets the IsInheritedFromBase property value.
        /// </summary>
        private bool GetIsInheritedFromBaseValue()
        {
            return !string.IsNullOrEmpty(this.BaseId);
        }
    }
}