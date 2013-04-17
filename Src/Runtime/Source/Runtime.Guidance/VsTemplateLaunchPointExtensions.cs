using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE80;
using NuPattern.VisualStudio.Solution;
using NuPattern.VisualStudio.Solution.Templates;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal static class VsTemplateLaunchPointExtensions
    {
        public static ITemplate Find(this IFxrTemplateService templates, VsTemplateLaunchPoint launchPoint)
        {
            return templates.Find(
                string.IsNullOrEmpty(launchPoint.Id) ? launchPoint.Name : launchPoint.Id,
                launchPoint.Category);
        }

        public static string GetTemplatePath(this ISolution solution, VsTemplateLaunchPoint launchPoint)
        {
            var dte = solution.As<Solution2>();
            if (dte != null)
            {
                return dte.GetProjectTemplate(
                    string.IsNullOrEmpty(launchPoint.Id) ? launchPoint.Name : launchPoint.Id,
                    launchPoint.Category);
            }

            return null;
        }

        public static VsTemplateLaunchPoint Find(
            this IEnumerable<Lazy<ILaunchPoint>> launchPoints,
            IVsTemplateData templateData)
        {
            return launchPoints
                .Select(lazy => lazy.Value)
                .OfType<VsTemplateLaunchPoint>()
                .FirstOrDefault(launchPoint =>
                                string.Equals(launchPoint.Category, templateData.ProjectType, StringComparison.InvariantCultureIgnoreCase) &&
                                (string.Equals(launchPoint.Id, templateData.TemplateID, StringComparison.InvariantCultureIgnoreCase) ||
                                 string.Equals(launchPoint.Name, templateData.Name.Value, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}