using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal static class ProjectExtensions
    {
        public static void ReloadProject(this Project project)
        {
            var dte = project.DTE;

            if (dte == null) return;
            using (var serviceProvider = new ServiceProvider((OLE.Interop.IServiceProvider)project.DTE))
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