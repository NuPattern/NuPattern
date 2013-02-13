using System;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Helper methods for <see cref="IVsHierarchy"/>.
	/// </summary>
    [CLSCompliant(false)]
    public static class IVsHierarchyExtensions
	{
		/// <summary>
		/// Gets the item id of the given hierarchy object.
		/// </summary>
		public static uint TryGetItemId(this IVsHierarchy hierarchy)
		{
            Guard.NotNull(() => hierarchy, hierarchy);
            
            object extObject;
			var itemId = uint.MaxValue;
			IVsHierarchy tempHierarchy;

			var succeeded = ErrorHandler.Succeeded(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_BrowseObject, out extObject));
			if (succeeded)
			{
				var browseObject = extObject as IVsBrowseObject;
				if (browseObject != null)
				{
					succeeded = Microsoft.VisualStudio.ErrorHandler.Succeeded(browseObject.GetProjectItem(out tempHierarchy, out itemId));
					if (succeeded)
					{
						return itemId;
					}
				}
			}

			return itemId;
		}

        /// <summary>
        /// Returns the <see cref="EnvDTE.Project"/> for the given <see cref="IVsHierarchy"/>.
        /// </summary>
        public static EnvDTE.Project ToProject(this IVsHierarchy hierarchy)
        {
            Guard.NotNull(() => hierarchy, hierarchy);

            object prjObject = null;
            var succeeded = ErrorHandler.Succeeded(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out prjObject));
            if (succeeded)
            {
                var project = (EnvDTE.Project)prjObject;
                if (project != null)
                {
                    return project;
                }
                else
                {
                    // Get containing project
                    var projectItem = (EnvDTE.ProjectItem)prjObject;
                    if (projectItem != null)
                    {
                        return projectItem.ContainingProject;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the equivalent <see cref="IItemContainer"/> for the given <see cref="IVsHierarchy"/>
        /// </summary>
        public static T As<T>(this IVsHierarchy hierarchy, ISolution solution) where T : IItemContainer
        {
            Guard.NotNull(() => hierarchy, hierarchy);
            Guard.NotNull(() => solution, solution);

            return solution.Find<T>(item => item.As<IVsHierarchy>() == hierarchy).FirstOrDefault();
        }

	}
}
