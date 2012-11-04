using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Runtime.UriProviders
{
	/// <summary>
	/// Provides support for uris that reference resources in a solution.
	/// </summary>
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IFxrUriReferenceProvider))]
	[CLSCompliant(false)]
	public class PackUriProvider : IFxrUriReferenceProvider<ResourcePack>
	{
		[Import]
		internal ISolution Solution { get; set; }

		static PackUriProvider()
		{
			if (!UriParser.IsKnownScheme("pack"))
				UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);
		}

		/// <summary>
		/// Creates the URI.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public Uri CreateUri(ResourcePack instance)
		{
			Guard.NotNull(() => instance, instance);

			var project = instance.Item.GetContainingProject();
			var uriFormat = "pack://application:,,,/{0};component/{1}";
			return new Uri(string.Format(uriFormat, project.Data.AssemblyName, instance.Item.PhysicalPath.Substring(Path.GetDirectoryName(project.PhysicalPath).Length).Replace('\\', '/').TrimStart('/')), UriKind.RelativeOrAbsolute);
		}

		/// <summary>
		/// Opens the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public void Open(ResourcePack instance)
		{
			Guard.NotNull(() => instance, instance);

			instance.Item.Select();
		}

		/// <summary>
		/// Resolves the URI.
		/// </summary>
		public ResourcePack ResolveUri(Uri uri)
		{
			Guard.NotNull(() => uri, uri);

			var packRegex = new Regex("pack://application:,,,/([^;]+);component/(.+)");
			var match = packRegex.Match(uri.AbsoluteUri);
			if (match.Groups.Count != 3)
			{
				return null;
			}

			var project = GetIdFromAssemblyName(match.Groups[1].Value);

			if (project == null)
			{
				return null;
			}

			var items = project.Find(match.Groups[2].Value.Replace('/', '\\'));
			if (items != null && items.Count() > 0)
			{
				return new ResourcePack(items.FirstOrDefault().As<IItem>());
			}
			return null;
		}

		/// <summary>
		/// Gets the URI scheme.
		/// </summary>
		/// <value>The URI scheme.</value>
		public string UriScheme
		{
			get
			{
				return "pack";
			}
		}

		private IProject GetIdFromAssemblyName(string capture)
		{
			return Solution.Items.OfType<IProject>().FirstOrDefault(p => p.Data.AssemblyName == capture);
		}

	}
}
