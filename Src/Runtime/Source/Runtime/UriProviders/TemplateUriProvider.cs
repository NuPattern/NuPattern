using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.VsIde;

namespace NuPattern.Runtime
{
    /// <summary>
    /// A <see cref="IFxrUriReferenceProvider"/> that resolves 
    /// uris to <see cref="ITemplate"/> instances that can be 
    /// used to unfold templates.
    /// Example: <c>template://[Project | Item | ProjectGroup | Custom]/category/[id | name]</c>.
    /// </summary>
    // TODO: Fix FERT so that this class does not have ot be public to register itself.
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IFxrUriReferenceProvider))]
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
