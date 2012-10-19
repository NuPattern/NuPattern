using System;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ole = Microsoft.VisualStudio.OLE.Interop;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Defines extension methods related to Project.
    /// </summary>
    [CLSCompliant(false)]
    public static class ProjectExtensions
    {
        /// <summary>
        /// Reloads the project.
        /// </summary>
        /// <param name="project">The project.</param>
        public static void ReloadProject(this Project project)
        {
            Guard.NotNull(() => project, project);

            var dte = project.DTE;

            if (dte != null)
            {
                using (var serviceProvider = new ServiceProvider((Ole.IServiceProvider)project.DTE))
                {
                    if (serviceProvider != null)
                    {
                        var solution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;

                        if (solution != null)
                        {
                            IVsHierarchy hierarchy;

                            if (ErrorHandler.Succeeded(solution.GetProjectOfUniqueName(project.FullName, out hierarchy)) && hierarchy != null)
                            {
                                object hier = null;

                                if (ErrorHandler.Succeeded(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ParentHierarchy, out hier)) && hier != null)
                                {
                                    object itemId = null;

                                    if (ErrorHandler.Succeeded(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ParentHierarchyItemid, out itemId)) && itemId != null)
                                    {
                                        var persistHierarchy = hier as IVsPersistHierarchyItem2;

                                        if (persistHierarchy != null)
                                        {
                                            ErrorHandler.ThrowOnFailure(persistHierarchy.ReloadItem((uint)(int)itemId, 0));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}