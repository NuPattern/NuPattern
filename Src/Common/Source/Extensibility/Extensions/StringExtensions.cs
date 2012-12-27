using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Provides formatting of strings using object properties.
	/// </summary>
	internal static class StringExtensions
	{
		/// <summary>
		/// Formats the given string using the given object for formatting.
		/// </summary>
		public static string NamedFormat(this string format, object source)
		{
			return FormatWith(format, null, source);
		}

		private static string FormatWith(this string format, IFormatProvider provider, object source)
		{
			Guard.NotNullOrEmpty(() => format, format);

			var values = new List<object>();
			var rewrittenFormat = Regex.Replace(format,
			  @"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
			  match =>
			  {
				  var startGroup = match.Groups["start"];
				  var propertyGroup = match.Groups["property"];
				  var formatGroup = match.Groups["format"];
				  var endGroup = match.Groups["end"];

				  values.Add((propertyGroup.Value == "0")
					? source
					: Eval(source, propertyGroup.Value));

				  var openings = startGroup.Captures.Count;
				  var closings = endGroup.Captures.Count;

				  return openings > closings || openings % 2 == 0
					 ? match.Value
					 : new string('{', openings) + (values.Count - 1)
					   + formatGroup.Value
					   + new string('}', closings);
			  },
			  RegexOptions.Compiled
			  | RegexOptions.CultureInvariant
			  | RegexOptions.IgnoreCase);

			return string.Format(provider, rewrittenFormat, values.ToArray());
		}

		private static object Eval(object source, string expression)
		{
			try
			{
				return DataBinder.Eval(source, expression);
			}
			catch (HttpException e)
			{
				throw new FormatException(expression, e);
			}
		}
	}
}