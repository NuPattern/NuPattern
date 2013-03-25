using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Runtime.Extensibility;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UriProviders
{
    /// <summary>
    /// Provides support for uris that reference resources in a solution.
    /// </summary>
    // TODO: Fix FERT so that this class does not have ot be public to register itself.
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IFxrUriReferenceProvider))]
    [CLSCompliant(false)]
    public class PackUriProvider : IFxrUriReferenceProvider<ResourcePack>
    {
        private const string PackUriScheme = "pack";
        private const string PackUriFormat = PackUriScheme + "://application:,,,/{0};component/{1}";

        [Import]
        internal ISolution Solution { get; set; }

        static PackUriProvider()
        {
            if (!UriParser.IsKnownScheme(PackUriScheme))
                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), PackUriScheme, -1);
        }

        /// <summary>
        /// Creates the URI.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public Uri CreateUri(ResourcePack instance)
        {
            Guard.NotNull(() => instance, instance);

            return new Uri(string.Format(CultureInfo.InvariantCulture, PackUriFormat,
                instance.AssemblyName,
                instance.ResourcePath), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Opens the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Open(ResourcePack instance)
        {
            Guard.NotNull(() => instance, instance);

            if (instance.Type == ResourcePackType.ProjectItem)
            {
                instance.GetItem().Select();
            }
        }

        /// <summary>
        /// Resolves the URI.
        /// </summary>
        public ResourcePack ResolveUri(Uri uri)
        {
            Guard.NotNull(() => uri, uri);

            // Parse the URI 
            var packRegexExpr = string.Format(CultureInfo.InvariantCulture, PackUriFormat, "([^;]+)", "(.+)");
            var packRegex = new Regex(packRegexExpr);
            var match = packRegex.Match(uri.AbsoluteUri);
            if (match.Groups.Count != 3)
            {
                return null;
            }

            var assemblyName = Uri.UnescapeDataString(match.Groups[1].Value);
            var resourcePath = Uri.UnescapeDataString(match.Groups[2].Value);

            // First, search the solution for the assembly and resource
            var project = GetProjectFromSolutionByAssemblyName(assemblyName);
            if (project != null)
            {
                var localPath = resourcePath.Replace('/', '\\');
                var items = project.Find(localPath);
                if (items.Any())
                {
                    return new ResourcePack(items.FirstOrDefault().As<IItem>());
                }
            }
            else
            {
                // If project is not found, then search the current project's assembly references for the resource
                var currentProject = this.Solution.GetCurrentProjectScope();
                if (currentProject != null)
                {
                    var assembly = currentProject.GetAssemblyReferences()
                        .Where(ar => ar.AssemblyName.Equals(assemblyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (assembly != null)
                    {
                        var resource = assembly.Resources.Where(res => res.Name.Equals(resourcePath, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (resource != null)
                        {
                            return new ResourcePack(resource);
                        }
                    }
                }
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
                return PackUriScheme;
            }
        }

        private IProject GetProjectFromSolutionByAssemblyName(string capture)
        {
            return this.Solution.Find<IProject>(p => p.Data.AssemblyName == capture).FirstOrDefault();
        }

    }
}
