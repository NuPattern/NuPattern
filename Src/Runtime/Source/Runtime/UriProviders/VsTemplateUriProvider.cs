using System;
using System.ComponentModel.Composition;
using System.Globalization;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// A <see cref="IFxrUriReferenceProvider"/> that resolves and creates Uris from 
	/// <see cref="IVsTemplate"/> instances. The Uri format is: 
	/// Example: <c>template://[Project | Item | ProjectGroup | Custom]/category/[id | name]</c>.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "VsTemplate", Justification = "Can't get the dictionary entry right.")]
	[CLSCompliant(false)]
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IFxrUriReferenceProvider))]
	public class VsTemplateUriProvider : TemplateUriProviderBase<IVsTemplate>
	{
		/// <summary>
		/// The uri scheme used by this provider, which equals "template://".
		/// </summary>
		public const string TemplateUriScheme = "template";
        private const string TemplateHostFormat = TemplateUriScheme + "://{TemplateType}";
        private const string UriFormat = TemplateHostFormat + "/{Category}/{IdOrName}";

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
			get { return TemplateUriScheme; }
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
        /// Gets the base URI for the given template type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "The current code base treats URIs as lower case.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Must treat this uri as a string.")]
        public static string GetUriBase(VsTemplateType templateType)
        {
            if (templateType == VsTemplateType.Item ||
                templateType == VsTemplateType.Project ||
                templateType == VsTemplateType.ProjectGroup)
            {
                return TemplateHostFormat.NamedFormat(new
                    {
                        TemplateType = templateType.ToString().ToLower(CultureInfo.InvariantCulture),
                    });
            }
            else
            {
                throw new NotSupportedException();
            }
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
