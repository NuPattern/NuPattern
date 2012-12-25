using System;
using System.Collections.Generic;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Extension methods for an Error List.
	/// </summary>
    [CLSCompliant(false)]
	public static class IErrorListExtensions
	{
		/// <summary>
		/// Adds the given results to the error list
		/// </summary>
		public static void AddRange(this IErrorList errorList, IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> results)
		{
			AddRange(errorList, null, results);
		}

		/// <summary>
		/// Adds the given results to the error list
		/// </summary>
		public static void AddRange(this IErrorList errorList, string document, IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> results)
		{
			Guard.NotNull(() => errorList, errorList);

			if (results != null)
			{
				results.ForEach(r => errorList.AddMessage(r.ErrorMessage, document, Microsoft.VisualStudio.Shell.TaskErrorCategory.Error));
			}
		}
	}
}
