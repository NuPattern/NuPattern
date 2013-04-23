using System;

namespace NuPattern.Library
{
    /// <summary>
    /// Helpers for GUID values
    /// </summary>
    internal static class GuidHelper
    {
        /// <summary>
        /// Returns the GUID formatter for the specified guid format.
        /// </summary>
        public static string GetFormat(GuidFormat format)
        {
            switch (format)
            {
                case GuidFormat.JustDigits:
                    return @"N";
                case GuidFormat.JustDigitsWithHyphens:
                    return @"D";
                case GuidFormat.DigitsHyphensCurlyBraces:
                    return @"B";
                case GuidFormat.DigitsHyphensRoundBraces:
                    return @"P";
                case GuidFormat.Hexadecimal:
                    return @"X";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
