using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UriProviders
{
    /// <summary>
    /// Provides support for uris that reference visual studio installed extensions.
    /// </summary>
    /// <remarks>
    /// Uri format is: <c>vsix://{extension identifier}</c>.
    /// </remarks>
    // TODO: Fix FERT so that this class does not have ot be public to register itself.
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IUriReferenceProvider))]
    [CLSCompliant(false)]
    public class VsixExtensionUriProvider : IUriReferenceProvider<IInstalledExtension>
    {
        /// <summary>
        /// Uri scheme for this provider, which is <c>vsix://</c>.
        /// </summary>
        public const string UriSchemeName = "vsix";

        private IVsExtensionManager extensionManager;
        private Action<string> openFileAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="VsixExtensionUriProvider"/> class.
        /// </summary>
        [ImportingConstructor]
        internal VsixExtensionUriProvider([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : this(
                serviceProvider.GetService<SVsExtensionManager, IVsExtensionManager>(),
                fileName => serviceProvider.GetService<EnvDTE.DTE>().ItemOperations.OpenFile(fileName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VsixExtensionUriProvider"/> class.
        /// </summary>
        public VsixExtensionUriProvider(IVsExtensionManager extensionManager, Action<string> openFileAction)
        {
            Guard.NotNull(() => extensionManager, extensionManager);
            Guard.NotNull(() => openFileAction, openFileAction);

            this.extensionManager = extensionManager;
            this.openFileAction = openFileAction;
        }

        /// <summary>
        /// Creates a Uri for the given extension.
        /// </summary>
        public Uri CreateUri(IInstalledExtension instance)
        {
            Guard.NotNull(() => instance, instance);
            Guard.NotNull(() => instance.Header, instance.Header);
            Guard.NotNull(() => instance.Header.Identifier, instance.Header.Identifier);

            return new Uri(UriSchemeName + "://" + instance.Header.Identifier);
        }

        /// <summary>
        /// Opens the manifest file of the given extension.
        /// </summary>
        public void Open(IInstalledExtension instance)
        {
            Guard.NotNull(() => instance, instance);
            Guard.NotNull(() => instance.InstallPath, instance.InstallPath);

            var manifestInfo = new FileInfo(Path.Combine(instance.InstallPath, "extension.vsixmanifest"));
            if (!manifestInfo.Exists)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.VsixExtensionUriProvider_ManifestMissing,
                    instance.InstallPath,
                    instance.Header.Identifier));
            }

            this.openFileAction(manifestInfo.FullName);
        }

        /// <summary>
        /// Tries to resolve the Uri to an installed extension instance.
        /// </summary>
        public IInstalledExtension ResolveUri(Uri uri)
        {
            Guard.NotNull(() => uri, uri);
            this.ThrowIfNotSupportedUriScheme(uri);

            var extensionId = uri.Host;
            var installedExtension = this.extensionManager.GetInstalledExtensions().FirstOrDefault(
                extension => extensionId.Equals(extension.Header.Identifier, StringComparison.OrdinalIgnoreCase));

            // Can return null.
            return installedExtension;
        }

        /// <summary>
        /// Gets the Uri scheme for this provider.
        /// </summary>
        public string UriScheme
        {
            get { return UriSchemeName; }
        }
    }
}