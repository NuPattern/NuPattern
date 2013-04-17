using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuPattern.VisualStudio.Solution.Hierarchy
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    internal class HierarchyNodeFactory : IHierarchyNodeFactory
    {
        private readonly IServiceProvider provider;
        private readonly IVsSolution solution;

        public HierarchyNodeFactory(IServiceProvider provider)
        {
            this.provider = provider;
            this.solution = provider.GetService(typeof(SVsSolution)) as IVsSolution;
        }

        public IHierarchyNode CreateFromProjectGuid(Guid projectGuid)
        {
            return new HierarchyNode(solution, projectGuid);
        }

        public IHierarchyNode GetSelectedProject()
        {
            IVsUIHierarchyWindow uiWindow = VsShellUtilities.GetUIHierarchyWindow(provider, new Guid(ToolWindowGuids.SolutionExplorer));
            IntPtr pHier;
            uint pItemId;
            IVsMultiItemSelect itemSelection;
            int hr = uiWindow.GetCurrentSelection(out pHier, out pItemId, out itemSelection);

            if (hr != VSConstants.S_OK)
            {
                throw new HierarchyNodeException("Could not retrieve tool window.");
            }

            IVsHierarchy selectedHier = Marshal.GetObjectForIUnknown(pHier) as IVsHierarchy;
            Debug.Assert(selectedHier != null);

            HierarchyNode node = new HierarchyNode(solution, selectedHier);

            return node;
        }
    }
}
