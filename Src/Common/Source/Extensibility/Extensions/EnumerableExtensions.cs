using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// General-purpose extensions over IEnumerable.
	/// </summary>
	[DebuggerStepThrough]
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Runs the given action for each element in the source.
		/// </summary>
		public static IEnumerable<T> Add<T>(this IEnumerable<T> source, T item)
		{
			Guard.NotNull(() => source, source);
			Guard.NotNull(() => item, item);

			return source.Concat(new[] { item });
		}

		/// <summary>
		/// Runs the given action for each element in the source.
		/// </summary>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			Guard.NotNull(() => source, source);
			Guard.NotNull(() => action, action);

			foreach (var item in source)
			{
				action(item);
			}
		}
	}
}