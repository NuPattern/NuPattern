using System;
using System.Globalization;
using System.IO;
using EnvDTE;
using EnvDTE80;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Runtime.UriProviders
{
    /// <summary>
    /// Defines the base class for all URI reference providers based in templates.
    /// </summary>
    /// <typeparam name="T">The type representing a template.</typeparam>
    [CLSCompliant(false)]
    public abstract class TemplateUriProviderBase<T> : IUriReferenceProvider<T>
    {
        private IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateUriProviderBase{T}"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected TemplateUriProviderBase(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the URI scheme.
        /// </summary>
        public abstract string UriScheme { get; }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        protected IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Creates the URI.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public virtual Uri CreateUri(T instance)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Opens the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public virtual void Open(T instance)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Resolves the URI.
        /// </summary>
        /// <param name="uri">The URI to resolve.</param>
        public virtual T ResolveUri(Uri uri)
        {
            Guard.NotNull(() => uri, uri);

            if (uri.Segments.Length != 3)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.SolutionUriProvider_InvalidProjectItemUri, uri));
            }

            var templateType = (VsTemplateType)Enum.Parse(typeof(VsTemplateType), uri.Host, true);
            var segments = uri.PathAndQuery.Split(new[] { Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            if (templateType == VsTemplateType.Project || templateType == VsTemplateType.ProjectGroup)
            {
                var templatePath = ((Solution2)this.serviceProvider.GetService<DTE>().Solution).GetProjectTemplate(segments[1], segments[0]);
                return this.ResolveInstance(templateType, templatePath);
            }
            else if (templateType == VsTemplateType.Item)
            {
                var templatePath = ((Solution2)this.serviceProvider.GetService<DTE>().Solution).GetProjectItemTemplate(segments[1], segments[0]);
                return this.ResolveInstance(templateType, templatePath);
            }

            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.TemplateTypeNotSupported, uri.Host));
        }

        /// <summary>
        /// Resolves the instance.
        /// </summary>
        /// <param name="templateType">The type of the template.</param>
        /// <param name="templatePath">The template path.</param>
        protected abstract T ResolveInstance(VsTemplateType templateType, string templatePath);
    }
}
