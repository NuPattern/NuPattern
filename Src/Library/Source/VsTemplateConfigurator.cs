using System;
using System.Globalization;
using System.IO;
using System.Linq;
using NuPattern.VisualStudio.Solution;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Library
{
    internal class VsTemplateConfigurator
    {
        private const int MaxTemplateIdLength = 64 - 1;
        private const int MaxUnicyLength = 12;
        private IServiceProvider provider;

        public VsTemplateConfigurator(IServiceProvider provider)
        {
            Guard.NotNull(() => provider, provider);
            this.provider = provider;
        }

        /// <summary>
        /// Configures the files included in the template for VSIX packaging,
        /// and returns the Uri for the template.
        /// </summary>
        public IVsTemplate Configure(IItem templateItem, string displayName, string description, string path)
        {
            Guard.NotNull(() => templateItem, templateItem);

            // Calculate the new Identifier
            var unicySeed = Guid.NewGuid().ToString(@"N");
            var unicyIdentifier = unicySeed.Substring(unicySeed.Length - MaxUnicyLength);
            var remainingNamedLength = MaxTemplateIdLength - MaxUnicyLength - 1;
            var namedIdentifier = path.Substring(path.Length <= remainingNamedLength ? 0 : (path.Length - remainingNamedLength));
            var templateId = string.Format(CultureInfo.InvariantCulture, @"{0}-{1}", unicyIdentifier, namedIdentifier);

            // Update the vstemplate
            var template = VsTemplateFile.Read(templateItem.PhysicalPath);
            template.SetTemplateId(templateId);
            template.SetDefaultName(SanitizeName(displayName));
            template.SetName(displayName);
            template.SetDescription(description);
            VsHelper.CheckOut(template.PhysicalPath);
            VsTemplateFile.Write(template);

            UpdateDirectoryProperties(templateItem);

            // Set VS attributes on the vstemplate file
            if (template.Type == VsTemplateType.Item)
            {
                templateItem.Data.ItemType = @"ItemTemplate";
            }
            else
            {
                templateItem.Data.ItemType = @"ProjectTemplate";
            }

            return template;
        }

        /// <summary>
        /// Sets VS project item attributes for the vstemplate, and all its items, for VSIX packaging.
        /// </summary>
        public static void UpdateDirectoryProperties(IItem templateItem)
        {
            // Set VS attributes on each item included in the vstemplate
            foreach (var item in templateItem.Parent.Traverse().OfType<IItem>().Where(t => (Path.GetExtension(t.PhysicalPath) ?? string.Empty) != @".vstemplate"))
            {
                var customTool = item.Data.CustomTool;
                item.Data.CopyToOutputDirectory = (int)CopyToOutput.PreserveNewest;
                item.Data.ItemType = @"Content";
                item.Data.CustomTool = customTool;
                item.Data.IncludeInVSIX = Boolean.FalseString.ToLower(CultureInfo.CurrentCulture);
            }
        }

        private static string SanitizeName(string value)
        {
            Guard.NotNull(() => value, value);

            return value.Replace(@" ", string.Empty);
        }
    }
}
