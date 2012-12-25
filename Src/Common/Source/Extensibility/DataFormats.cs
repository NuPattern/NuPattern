using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// A class that defines the data formats and expressions for various elements.
    /// </summary>
    public static class DataFormats
    {
        /// <summary>
        /// The maximum number of characters in the name of an item in the solution.
        /// </summary>
        public const int MaxSolutionItemNameLength = 512;

        /// <summary>
        /// The invalid characters for the name of an item in the solution.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly IEnumerable<char> InvalidSolutionItemNameChars = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).Distinct();

        /// <summary>
        /// The invalid characters for the path of an item in the solution.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly IEnumerable<char> InvalidSolutionPathNameChars = Path.GetInvalidPathChars();

        /// <summary>
        /// Determines of the given value is a valid C# identifier.
        /// </summary>
        public static bool IsValidCSharpIdentifier(string value)
        {
            Guard.NotNull(() => value, value);

            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                return provider.IsValidIdentifier(value);
            }
        }

        /// <summary>
        /// Determines whether the given value is a valid name for a solution item.
        /// </summary>
        public static bool IsValidSolutionItemName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (value.Length > DataFormats.MaxSolutionItemNameLength)
            {
                return false;
            }

            return (value.Intersect(InvalidSolutionItemNameChars).Count() == 0);
        }

        /// <summary>
        /// Determines whether the given value is a valid name for a solution item.
        /// </summary>
        public static bool IsValidSolutionPathName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (value.Length > DataFormats.MaxSolutionItemNameLength)
            {
                return false;
            }

            return value.Intersect(InvalidSolutionPathNameChars).Any();
        }

        /// <summary>
        /// Returns a valid solution item name from given name, by removing invalid file characters.
        /// </summary>
        public static string MakeValidSolutionItemName(string value)
        {
            return new string(value.Where(ch => !DataFormats.InvalidSolutionItemNameChars.Contains(ch)).ToArray());
        }

        /// <summary>
        /// Returns a valid solution path name from given path, by removing invalid file characters.
        /// </summary>
        public static string MakeValidSolutionPathName(string value)
        {
            return new string(value.Where(ch => !DataFormats.InvalidSolutionPathNameChars.Contains(ch)).ToArray());
        }

        /// <summary>
        /// Returns a prefered valid solution item name from given name, by removing invalid characters and spaces.
        /// </summary>
        public static string MakePreferredSolutionItemName(string value)
        {
            return DataFormats.MakeValidSolutionItemName(value).Replace(" ", string.Empty);
        }

        /// <summary>
        /// Data formats and expressions for designtime elements.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class DesignTime
        {
            private const string DisplayNameCommonChars = @"~!@#\$%\^&\*\(\)-_\+=\{\}\|\[\];:""'\<\>,\.\?";

            /// <summary>
            /// Format of a DisplayName.
            /// </summary>
            public const string DisplayNameExpression = @"(^[\d\w]+[\d\w\x20" + DisplayNameCommonChars + @"]*[\d\w" + DisplayNameCommonChars + @"]$|^[\d\w]$)";

            /// <summary>
            /// Format of a Namespace.
            /// </summary>
            public const string NamespaceExpression = @"^([a-zA-Z_][\w]*\.)*[a-zA-Z_][a-zA-Z0-9]*$";

            /// <summary>
            /// Format of a Category.
            /// </summary>
            public const string CategoryNameExpression = @"^[\w \-]+$";

            /// <summary>
            /// Maximum length of a Property name.
            /// </summary>
            public const int MaxPropertyLength = 512;

            /// <summary>
            /// Determines of the given value is a valid .NET namespace identifier.
            /// </summary>
            public static bool IsValidNamespaceIdentifier(string value)
            {
                Guard.NotNull(() => value, value);

                Regex regEx = new Regex(NamespaceExpression, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
                return regEx.IsMatch(value);
            }
        }

        /// <summary>
        /// Data formats and expressions for runtime elements.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Runtime
        {
            /// <summary>
            /// Format of an Instance Name.
            /// </summary>
            public const string InstanceNameExpression = DesignTime.DisplayNameExpression;
        }
    }
}
