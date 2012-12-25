using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Runtime;

namespace NuPattern.Authoring.PatternToolkit.Automation.UriProviders
{
    /// <summary>
    /// Defines a URI provider for Views
    /// </summary>
    [Export(typeof(IFxrUriReferenceProvider))]
    [CLSCompliant(false)]
    public class ViewSchemaUriProvider : IFxrUriReferenceProvider<IViewSchema>
    {
        /// <summary>
        /// The Uri Scheme Name
        /// </summary>
        public const string UriSchemeName = "view";

        /// <summary>
        /// The Uri Scheme Name
        /// </summary>
        public string UriScheme
        {
            get
            {
                return UriSchemeName;
            }
        }

        /// <summary>
        /// Creates a new Uri
        /// </summary>
        /// <param name="instance">The instance</param>
        public Uri CreateUri(IViewSchema instance)
        {
            Guard.NotNull(() => instance, instance);

            var builder = new UriBuilder();
            builder.Scheme = this.UriScheme;
            builder.Host = ((INamedElementSchema)instance).Id.ToString();

            return builder.Uri;
        }

        /// <summary>
        /// Opens the view
        /// </summary>
        /// <param name="instance">The view instance</param>
        public void Open(IViewSchema instance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves the Uri to a View
        /// </summary>
        /// <param name="uri">The Uri</param>
        /// <returns>The View</returns>
        public IViewSchema ResolveUri(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}