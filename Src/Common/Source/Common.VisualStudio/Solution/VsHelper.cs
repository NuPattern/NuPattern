using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.VisualStudio.Solution.Hierarchy;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// Helper for common calling patterns in VS-COM.
    /// </summary>
    internal static class VsHelper
    {
        /// <summary>
        /// Method signature for methods that retrieve properties.
        /// </summary>
        public delegate int GetPropertyHandler(int propid, out object pvar);

        /// <summary>
        /// Gets the given property as a typed value, or a the default value 
        /// of <typeparamref name="T"/> if the call does not succeed.
        /// </summary>
        public static T GetPropertyOrDefault<T>(GetPropertyHandler handler, int propId)
        {
            return GetPropertyOrDefault<T>(handler, propId, default(T));
        }

        /// <summary>
        /// Gets the given property as a typed value, or a default value if 
        /// the call does not succeed.
        /// </summary>
        public static T GetPropertyOrDefault<T>(GetPropertyHandler handler, int propId, T defaultValue)
        {
            object value = null;

            if (Microsoft.VisualStudio.ErrorHandler.Succeeded(handler(propId, out value)))
            {
                return (T)value;
            }

            return defaultValue;
        }


        /// <summary>
        /// Selected item in the solution explorer based on given hierarchyNode
        /// </summary>
        public static void Select(IServiceProvider serviceProvider, IHierarchyNode hierarchyNode)
        {
            Select(serviceProvider, new[] { hierarchyNode });
        }

        /// <summary>
        /// Selected item in the solution explorer based on given hierarchyNode
        /// </summary>
        public static void Select(IServiceProvider serviceProvider, IEnumerable<IHierarchyNode> hierarchyNodes)
        {
            Debug.Assert(serviceProvider != null && hierarchyNodes != null);

            if (serviceProvider != null && hierarchyNodes != null)
            {
                var uiShell = serviceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
                var slnExplorerGuid = new Guid(ToolWindowGuids80.SolutionExplorer);

                IVsWindowFrame frame;
                ErrorHandler.ThrowOnFailure(uiShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref slnExplorerGuid, out frame));

                object view;
                ErrorHandler.ThrowOnFailure(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out view));

                bool first = true;
                var hierarchyWindow = view as IVsUIHierarchyWindow;
                if (hierarchyWindow != null)
                {
                    foreach (var hierarchyNode in hierarchyNodes)
                    {
                        hierarchyWindow.ExpandItem(hierarchyNode.GetObject<IVsHierarchy>() as IVsUIHierarchy,
                                                   hierarchyNode.ItemId,
                                                   first ? EXPANDFLAGS.EXPF_SelectItem : EXPANDFLAGS.EXPF_AddSelectItem);

                        if (first) first = false;
                    }
                }
                frame.Show();
            }
        }   

        /// <summary>
        /// Checks-out the file from source control.
        /// </summary>
        public static void CheckOut(string fileName)
        {
            Guard.NotNullOrEmpty(() => fileName, fileName);

            if (File.Exists(fileName))
            {
                var vs = ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

                if (vs != null && vs.SourceControl != null &&
                    vs.SourceControl.IsItemUnderSCC(fileName) && !vs.SourceControl.IsItemCheckedOut(fileName))
                {
                    vs.SourceControl.CheckOutItem(fileName);
                }
                else
                {
                    File.SetAttributes(fileName, FileAttributes.Normal);
                }
            }
        }

        /// <summary>
        /// Executes an action while suspending file change notifcations.
        /// </summary>
        public static void WithoutFileChangeNotification(IServiceProvider serviceProvider, string fileName, Action action)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);
            Guard.NotNull(() => action, action);
            Guard.NotNullOrEmpty(() => fileName, fileName);

            var vsFileChangeEx = serviceProvider.GetService(typeof(IVsFileChangeEx)) as IVsFileChangeEx;

            // Suspend file change notifications
            if (vsFileChangeEx != null)
            {
                ErrorHandler.ThrowOnFailure(vsFileChangeEx.IgnoreFile(0u, fileName, 1));
            }

            // Do the action
            action();

            // Resume file change notifications
            if (vsFileChangeEx != null)
            {
                ErrorHandler.ThrowOnFailure(vsFileChangeEx.SyncFile(fileName));
                ErrorHandler.ThrowOnFailure(vsFileChangeEx.IgnoreFile(0u, fileName, 0));
            }
        }
    }
}
