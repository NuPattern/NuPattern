using System;
using Microsoft.VisualStudio.Modeling.Shell;
using VSWindows = NuPattern.VisualStudio.Windows;

namespace NuPattern.Modeling.Windows
{
    /// <summary>
    /// Extensions for managing DSL designer windows.
    /// </summary>
    [CLSCompliant(false)]
    public static class WindowExtensions
    {
        /// <summary>
        /// Performs the action on the specified file while open in its default editor.
        /// </summary>
        /// <param name="fileName">The physical path to the file to open.</param>
        /// <param name="action">The action to execute</param>
        /// <param name="leaveOpen">Whether to leave the file open in editor or close it after complete</param>
        public static void DoActionOnDesigner(string fileName, Action<ModelingDocData> action, bool leaveOpen = false)
        {
            VSWindows.WindowExtensions.DoActionOnWindow(fileName, action, leaveOpen);
        }

        /// <summary>
        /// Gets the designer from the specified file while open in its default editor.
        /// </summary>
        /// <returns></returns>
        public static ModelingDocData GetDesignerData(string fileName)
        {
            return VSWindows.WindowExtensions.GetDesignerData<ModelingDocData>(fileName);
        }
    }
}