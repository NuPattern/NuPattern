using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.VsTemplateSchema;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Automation
{
    internal class UnfoldPathHelper
    {
        private ISolution solution;

        public UnfoldPathHelper(ISolution solution)
        {
            this.solution = solution;
        }

        public string GetUniqueName(string baseName, IVsTemplate template, IItemContainer parent)
        {
            Guard.NotNull(() => baseName, baseName);
            Guard.NotNull(() => parent, parent);

            if (template.Type == VsTemplateType.ProjectGroup)
            {
                return baseName;
            }
            else if (template.Type == VsTemplateType.Project)
            {
                return GetProjectUniqueName(baseName);
            }
            else if (template.Type == VsTemplateType.Item)
            {
                return GetProjectItemUniqueName(baseName, template, parent);
            }

            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                Resources.UnfoldPathHelper_TemplateTypeIsNotSupported, template));
        }

        private string GetProjectUniqueName(string baseName)
        {
            Guard.NotNull(() => baseName, baseName);

            var directoryBase = System.IO.Path.GetDirectoryName(this.solution.PhysicalPath);
            IEnumerable<string> subDirectoriesNames =
                Directory.EnumerateDirectories(directoryBase).Select(dir => System.IO.Path.GetFileName(dir));

            var uniqueName = UniqueNameGenerator.EnsureUnique(
                baseName,
                newName => !subDirectoriesNames.Any(dir => dir.Equals(newName, StringComparison.OrdinalIgnoreCase)));

            return uniqueName;
        }

        private static string GetProjectItemUniqueName(string baseName, IVsTemplate template, IItemContainer parent)
        {
            Guard.NotNull(() => baseName, baseName);
            Guard.NotNull(() => template, template);
            Guard.NotNull(() => parent, parent);

            var directoryBase = System.IO.Path.GetDirectoryName(parent.PhysicalPath);
            var files = Directory.EnumerateFiles(directoryBase);

            var contentProjectItem = template.TemplateContent.Items.OfType<VSTemplateTemplateContentProjectItem>().FirstOrDefault();

            if (contentProjectItem == null)
            {
                return baseName;
            }

            var outputExtension = System.IO.Path.GetExtension(contentProjectItem.TargetFileName);

            var uniqueName = UniqueNameGenerator.EnsureUnique(
                baseName,
                newName => !files.Any(file => System.IO.Path.GetFileName(file).Equals(
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}", System.IO.Path.GetFileNameWithoutExtension(newName), outputExtension),
                    StringComparison.OrdinalIgnoreCase)));

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}",
                System.IO.Path.GetFileNameWithoutExtension(uniqueName),
                System.IO.Path.GetExtension(baseName));
        }
    }
}
