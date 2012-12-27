using System;
using System.Collections.Generic;
using System.Linq;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provides usability members over an <see cref="IPatternManager"/>.
	/// </summary>
	[CLSCompliant(false)]
	public static class PatternManagerExtensions
	{
		/// <summary>
		/// Finds a pattern instance by name.
		/// </summary>
		/// <param name="manager">The pattern manager to find products in.</param>
		/// <param name="name">The pattern name.</param>
		/// <returns>The found pattern or <see langword="null"/> if the pattern with the given name has not been instantiated.</returns>
		public static IProduct Find(this IPatternManager manager, string name)
		{
			Guard.NotNull(() => manager, manager);
			Guard.NotNullOrEmpty(() => name, name);

			return manager.IsOpen ?
				manager.Products.FirstOrDefault(p => p.InstanceName.Equals(name, StringComparison.OrdinalIgnoreCase)) :
				null;
		}

		/// <summary>
		/// Deletes the pattern by name.
		/// </summary>
		/// <param name="manager">The pattern manager.</param>
		/// <param name="name">The pattern name.</param>
		public static bool Delete(this IPatternManager manager, string name)
		{
			Guard.NotNull(() => manager, manager);
			Guard.NotNullOrEmpty(() => name, name);

			var product = manager.Find(name);
			if (product == null)
			{
				return false;
			}

			return manager.DeleteProduct(product);
		}

		/// <summary>
		/// Gets the candidate extension points.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="extensionPointId">The extension point id.</param>
		public static IEnumerable<IInstalledToolkitInfo> GetCandidateExtensionPoints(
			this IPatternManager manager,
			string extensionPointId)
		{
			Guard.NotNull(() => manager, manager);

			return manager.InstalledToolkits
				.Where(f => f.Schema != null && f.Schema.Pattern != null &&
					f.Schema.Pattern.ProvidedExtensionPoints.Any(e =>
						e.ExtensionPointId.Equals(extensionPointId, StringComparison.OrdinalIgnoreCase)));
		}
	}
}