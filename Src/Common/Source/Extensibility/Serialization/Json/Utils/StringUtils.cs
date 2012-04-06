using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization.Json
{
    internal static class StringUtils
    {
        //public const string CarriageReturnLineFeed = "\r\n";
        //public const string Empty = "";
        //public const char CarriageReturn = '\r';
        //public const char LineFeed = '\n';
        //public const char Tab = '\t';

        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            Guard.NotNull(() => format, format);

            return string.Format(provider, format, args);
        }

        ///// <summary>
        ///// Determines whether the string is all white space. Empty string will return false.
        ///// </summary>
        ///// <param name="s">The string to test whether it is all white space.</param>
        ///// <returns>
        ///// 	<c>true</c> if the string is all white space; otherwise, <c>false</c>.
        ///// </returns>
        //public static bool IsWhiteSpace(string s)
        //{
        //    if (s == null)
        //        throw new ArgumentNullException("s");

        //    if (s.Length == 0)
        //        return false;

        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        if (!char.IsWhiteSpace(s[i]))
        //            return false;
        //    }

        //    return true;
        //}

        /// <summary>
        /// Nulls an empty string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>Null if the string was null, otherwise the string unchanged.</returns>
        public static string NullEmptyString(string s)
        {
            return (string.IsNullOrEmpty(s)) ? null : s;
        }

        //public static StringWriter CreateStringWriter(int capacity)
        //{
        //    StringBuilder sb = new StringBuilder(capacity);
        //    StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);

        //    return sw;
        //}

        //public static int? GetLength(string value)
        //{
        //    if (value == null)
        //        return null;
        //    else
        //        return value.Length;
        //}

        //public static string ToCharAsUnicode(char c)
        //{
        //    char h1 = MathUtils.IntToHex((c >> 12) & '\x000f');
        //    char h2 = MathUtils.IntToHex((c >> 8) & '\x000f');
        //    char h3 = MathUtils.IntToHex((c >> 4) & '\x000f');
        //    char h4 = MathUtils.IntToHex(c & '\x000f');

        //    return new string(new[] { '\\', 'u', h1, h2, h3, h4 });
        //}

        public static TSource ForgivingCaseSensitiveFind<TSource>(this IEnumerable<TSource> source, Func<TSource, string> valueSelector, string testValue)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (valueSelector == null)
                throw new ArgumentNullException("valueSelector");

            var caseInsensitiveResults = source.Where(s => string.Equals(valueSelector(s), testValue, StringComparison.OrdinalIgnoreCase));
            if (caseInsensitiveResults.Count() <= 1)
            {
                return caseInsensitiveResults.SingleOrDefault();
            }
            else
            {
                // multiple results returned. now filter using case sensitivity
                var caseSensitiveResults = source.Where(s => string.Equals(valueSelector(s), testValue, StringComparison.Ordinal));
                return caseSensitiveResults.SingleOrDefault();
            }
        }

        //public static string ToCamelCase(string s)
        //{
        //    if (string.IsNullOrEmpty(s))
        //        return s;

        //    if (!char.IsUpper(s[0]))
        //        return s;

        //    string camelCase = char.ToLower(s[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        //    if (s.Length > 1)
        //        camelCase += s.Substring(1);

        //    return camelCase;
        //}
    }
}