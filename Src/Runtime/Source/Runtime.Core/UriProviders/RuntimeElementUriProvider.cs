using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace NuPattern.Runtime.UriProviders
{
    /// <summary>
    /// Uri provider that resolves runtime state element.
    /// </summary>
    // TODO: Fix FERT so that this class does not have ot be public to register itself.
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IUriReferenceProvider))]
    [CLSCompliant(false)]
    public class RuntimeElementUriProvider : IUriReferenceProvider<IInstanceBase>
    {
        private const string ElementUriScheme = "patternmanager";

        /// <summary>
        /// Gets the URI scheme.
        /// </summary>
        /// <value>The URI scheme.</value>
        public string UriScheme
        {
            get { return ElementUriScheme; }
        }

        [Import(typeof(SVsServiceProvider))]
        internal IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Creates the URI.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public Uri CreateUri(IInstanceBase instance)
        {
            Guard.NotNull(() => instance, instance);

            //// patternmanager://element_Id
            return new Uri(this.UriScheme + Uri.SchemeDelimiter + instance.Id);
        }

        /// <summary>
        /// Opens the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Open(IInstanceBase instance)
        {
            //// TODO: Implement selection behavior via PatternManager
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves the URI.
        /// </summary>
        public IInstanceBase ResolveUri(Uri uri)
        {
            Guard.NotNull(() => uri, uri);

            if (string.IsNullOrEmpty(uri.Host))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidProjectItemUri, uri));
            }

            var manager = this.ServiceProvider.GetService<IPatternManager>();
            var id = new Guid(uri.Host);

            return manager.Store.FindAll<IInstanceBase>().FirstOrDefault(p => p.Id.Equals(id));
        }
    }
}