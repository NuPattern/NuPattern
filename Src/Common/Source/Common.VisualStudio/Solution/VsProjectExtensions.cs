using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Extensions to <see cref="VsProject"/>
    /// </summary>
    internal static class VsProjectExtensions
    {
        /// <summary>
        /// Retrieves the sub project type ids
        /// </summary>
        public static string GetProjectTypeGuids(this VsProject project)
        {
            if (project == null) return string.Empty;

            var projectTypeGuids = string.Empty;
            var hierarchy = project.VsHierarchy;
            var aggregatableProject = (IVsAggregatableProject)hierarchy;

            var returnCode = aggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids);

            if (projectTypeGuids == null)
                return string.Empty;

            return projectTypeGuids;
        }

        public static bool IsFeatureProject(this VsProject project)
        {
            return false;
            //return project != null && project.GetProjectTypeGuids().ToLower().Contains(
            //    new Guid(ModelingFeatureExtension.FeatureProjectFlavorGuid).ToString().ToLower());
        }
    }
}