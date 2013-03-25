using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.UriProviders
{
    /// <summary>
    /// A <see cref="IFxrUriReferenceProvider"/> that resolves and creates Uris from 
    /// <see cref="IVsTemplate"/> instances. The Uri format is: 
    /// Example: <c>template://[Project | Item | ProjectGroup | Custom]/category/[id | name]</c>.
    /// </summary>
    // TODO: Fix FERT so that this class does not have ot be public to register itself.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "VsTemplate", Justification = "Can't get the dictionary entry right.")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IFxrUriReferenceProvider))]
    [CLSCompliant(false)]
    public class VsTemplateUriProvider : TemplateUriProviderBase<IVsTemplate>
    {
        private const string UriFormat = VsTemplateUri.HostFormat + "/{Category}/{IdOrName}";

        /// <summary>
        /// Initializes a new instance of the <see cref="VsTemplateUriProvider"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        [ImportingConstructor]
        public VsTemplateUriProvider([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Gets the URI scheme.
        /// </summary>
        /// <value>The URI scheme.</value>
        public override string UriScheme
        {
            get { return VsTemplateUri.UriScheme; }
        }

        /// <summary>
        /// Creates the URI.
        /// </summary>
        public override Uri CreateUri(IVsTemplate instance)
        {
            Guard.NotNull(() => instance, instance);

            var uri = UriFormat.NamedFormat(new
            {
                TemplateType = instance.TypeString,
                Category = instance.TemplateData.ProjectType,
                IdOrName = !string.IsNullOrEmpty(instance.TemplateData.TemplateID) ?
                    instance.TemplateData.TemplateID :
                    instance.TemplateData.Name.Value
            });

            return new Uri(uri);
        }

        /// <summary>
        /// Resolves the instance.
        /// </summary>
        /// <param name="templateType">The type of the template.</param>
        /// <param name="templatePath">The template path.</param>
        protected override IVsTemplate ResolveInstance(VsTemplateType templateType, string templatePath)
        {
            return VsTemplateFile.Read(templatePath);
        }
    }
}
