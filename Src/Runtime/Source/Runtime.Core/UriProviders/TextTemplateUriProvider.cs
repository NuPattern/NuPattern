using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UriProviders
{
    /// <summary>
    /// Provides support for uris that reference T4 templates.
    /// </summary>
    /// <example>
    /// <para>
    /// The following uri represents a T4 that exists in a path relative to 
    /// an installed VSIX location:
    /// 	<code>t4://extension/{id}/{relative-path-to-t4-file}</code>
    /// </para>
    /// <para>
    /// The following uri represents a T4 that exists in a path relative to 
    /// the current solution:
    /// 	<code>t4://solution/{solution uri provider supported path}</code>
    /// </para>
    /// </example>
    // TODO: Fix FERT so that this class does not have ot be public to register itself.
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IUriReferenceProvider))]
    [CLSCompliant(false)]
    public class TextTemplateUriProvider : IUriReferenceProvider<ITemplate>
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<TextTemplateUriProvider>();

        /// <summary>
        /// The host to use in a uri that is resolved relative to the current solution.
        /// </summary>
        public const string SolutionRelativeHost = SolutionUriProvider.UriSchemeName;

        private ITextTemplating templating;
        private IModelBus modelBus;
        /// <devdoc>
        /// This has to be lazy because otherwise it would cause a circular dependency.
        /// </devdoc>
        private Lazy<IUriReferenceService> uriService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTemplateUriProvider"/> class.
        /// </summary>
        [ImportingConstructor]
        public TextTemplateUriProvider([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider, Lazy<IUriReferenceService> uriService)
            : this(serviceProvider.GetService<STextTemplating, ITextTemplating>(), serviceProvider.GetService<SModelBus, IModelBus>(), uriService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTemplateUriProvider"/> class.
        /// </summary>
        public TextTemplateUriProvider(ITextTemplating templating, IModelBus modelBus, Lazy<IUriReferenceService> uriService)
        {
            Guard.NotNull(() => templating, templating);
            Guard.NotNull(() => modelBus, modelBus);
            Guard.NotNull(() => uriService, uriService);

            this.templating = templating;
            this.modelBus = modelBus;
            this.uriService = uriService;
        }

        /// <summary>
        /// Creates the Uri to represent the instance.
        /// </summary>
        public Uri CreateUri(ITemplate instance)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Opens the specified instance.
        /// </summary>
        public void Open(ITemplate instance)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Resolves the Uri to an instance of a template.
        /// </summary>
        public ITemplate ResolveUri(Uri uri)
        {
            Guard.NotNull(() => uri, uri);
            this.ThrowIfNotSupportedUriScheme(uri);

            if (uri.Host == TextTemplateUri.ExtensionRelativeHost)
            {
                return TryResolveExtensionRelative(uri);
            }
            else if (uri.Host == SolutionRelativeHost)
            {
                return TryResolveSolutionRelative(uri);
            }
            else
            {
                // Treat as a full path reference.
                return TryResolveFullPath(uri);
            }
        }

        /// <devdoc>
        /// 	<code>t4://extension/{id}/{relative-path-to-t4-file}</code>
        /// </devdoc>
        private ITemplate TryResolveExtensionRelative(Uri uri)
        {
            var segments = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split(Path.AltDirectorySeparatorChar);
            if (segments.Length < 2)
            {
                tracer.TraceWarning(Resources.TextTemplateUriProvider_TraceInvalidExtensionRelative, uri);
                return null;
            }

            var vsixUri = new Uri(VsixExtensionUriProvider.UriSchemeName + "://" + segments[0]);
            var extension = uriService.Value.ResolveUri<IInstalledExtension>(vsixUri);
            if (extension == null)
            {
                tracer.TraceWarning(Resources.TextTemplateUriProvider_ExtensionNotFound,
                    uri, uri.Segments[0]);
                return null;
            }

            tracer.TraceInformation(Resources.TextTemplateUriProvider_TraceExtensionPath, uri, extension.InstallPath);

            var fileInfo = new FileInfo(Path.Combine(
                new[] { extension.InstallPath }
                .Concat(segments.Skip(1))
                .ToArray()));

            if (!fileInfo.Exists)
            {
                tracer.TraceWarning(Resources.TextTemplateUriProvider_TraceTemplateNotFound, fileInfo.FullName, uri);
                return null;
            }

            return new TextTemplate(this.templating, this.modelBus, fileInfo.FullName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private ITemplate TryResolveSolutionRelative(Uri uri)
        {
            throw new NotImplementedException(uri.ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private ITemplate TryResolveFullPath(Uri uri)
        {
            throw new NotImplementedException(uri.ToString());
        }

        /// <summary>
        /// Gets the Uri scheme for this provider.
        /// </summary>
        public string UriScheme
        {
            get { return TextTemplateUri.UriScheme; }
        }
    }
}
