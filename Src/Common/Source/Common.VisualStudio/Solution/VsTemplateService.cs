using System;
using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;


namespace NuPattern.VisualStudio.Solution
{
    [Export(typeof(IFxrTemplateService))]
    internal class VsTemplateService : IFxrTemplateService
    {
        DTE dte;

        [Import]
        public Lazy<IUriReferenceService> UriReferenceService { get; set; }

        [ImportingConstructor]
        public VsTemplateService(
            [Import(typeof(SVsServiceProvider), AllowDefault = true)]IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.dte = (DTE)serviceProvider.GetService(typeof(DTE));

            Guard.NotNull(() => dte, dte);
        }

        public ITemplate Find(string nameOrId, string category)
        {
            Guard.NotNullOrEmpty(() => nameOrId, nameOrId);
            Guard.NotNullOrEmpty(() => category, category);

            try
            {
                var templatePath = ((Solution2)dte.Solution).GetProjectTemplate(nameOrId, category);
                return new VsProjectTemplate(templatePath);
            }
            catch (IOException)
            {
                try
                {
                    var templatePath = ((Solution2)dte.Solution).GetProjectItemTemplate(nameOrId, category);
                    return new VsItemTemplate(templatePath);
                }
                catch (IOException)
                {
                }
            }

            return null;
        }

        public ITemplate Find(Uri uri)
        {
            try
            {
                return this.UriReferenceService.Value.ResolveUri<ITemplate>(uri);
            }
            catch
            {
                return null;
            }
        }
    }
}
