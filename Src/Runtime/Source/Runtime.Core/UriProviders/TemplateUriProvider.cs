using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using NuPattern.VisualStudio.Solution;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Runtime.UriProviders
{
    /// <summary>
    /// A <see cref="IUriReferenceProvider"/> that resolves 
    /// uris to <see cref="ITemplate"/> instances that can be 
    /// used to unfold templates.
    /// Example: <c>template://[Project | Item | ProjectGroup | Custom]/category/[id | name]</c>.
    /// </summary>
    // TODO: Fix FERT so that this class does not have ot be public to register itself.
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IUriReferenceProvider))]
    [CLSCompliant(false)]
    public class TemplateUriProvider : TemplateUriProviderBase<ITemplate>
    {
        /// <summary>
        /// The uri scheme used by this provider, which equals "template://".
        /// </summary>
        public const string TemplateUriScheme = "template";
        private const string UriFormat = TemplateUriScheme + "://{TemplateType}/{Category}/{IdOrName}";

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateUriProvider"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        [ImportingConstructor]
        public TemplateUriProvider([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Gets the URI scheme.
        /// </summary>
        /// <value>The URI scheme.</value>
        public override string UriScheme
        {
            get { return TemplateUriScheme; }
        }

        /// <summary>
        /// Resolves the instance.
        /// </summary>
        /// <param name="templateType">Type of the template.</param>
        /// <param name="templatePath">The template path.</param>
        protected override ITemplate ResolveInstance(VsTemplateType templateType, string templatePath)
        {
            if (templateType == VsTemplateType.Project || templateType == VsTemplateType.ProjectGroup)
            {
                return new VsProjectTemplate(templatePath);
            }

            if (templateType == VsTemplateType.Item)
            {
                return new VsItemTemplate(templatePath);
            }

            return null;
        }
    }
}
