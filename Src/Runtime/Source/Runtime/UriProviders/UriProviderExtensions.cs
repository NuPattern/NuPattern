using System;
using System.Globalization;
using Microsoft.VisualStudio.Patterning.Runtime.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Runtime.UriProviders
{
	/// <summary>
	/// Provides usability methods for uri provider consumers and authors.
	/// </summary>
	public static class UriProviderExtensions
	{
		/// <summary>
		/// Throws <see cref="NotSupportedException"/> if the <see cref="Uri.Scheme"/> does 
		/// not match the <see cref="IFxrUriReferenceProvider.UriScheme"/>.
		/// </summary>
		[CLSCompliant(false)]
		public static void ThrowIfNotSupportedUriScheme(this IFxrUriReferenceProvider provider, Uri uri)
		{
			Guard.NotNull(() => provider, provider);
			Guard.NotNull(() => uri, uri);

			if (uri.Scheme != provider.UriScheme)
			{
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.UriProviderExtensions_WrongScheme,
					provider.UriScheme));
			}
		}
	}
}
