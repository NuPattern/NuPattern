using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Shell.Properties;

namespace NuPattern.Runtime.Shell.ToolWindows
{

    [Guid("c44b2e95-86f4-40dd-8fc8-bbc9725ea86b")]
    partial class SolutionBuilderToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SolutionBuilderToolWindow"/> class.
        /// </summary>
        public SolutionBuilderToolWindow() :
            base(null)
        {
            this.Caption = Resources.SolutionBuilderToolWindow_WindowTitle;

            this.BitmapResourceID = 301;
            this.BitmapIndex = 0;
        }
    }

    [Guid("6A2D1053-BEEB-4FDF-9FB0-133CE1FE6D25")]
    partial class GuidanceExplorerToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GuidanceExplorerToolWindow"/> class.
        /// </summary>
        public GuidanceExplorerToolWindow() :
            base(null)
        {
            this.Caption = Resources.GuidanceExplorerToolWindow_WindowTitle;

            this.BitmapResourceID = 302;
            this.BitmapIndex = 0;
        }
    }

    [Guid("167f0a44-f79b-4007-a084-8ecc1457776e")]
    partial class GuidanceBrowserToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GuidanceBrowserToolWindow"/> class.
        /// </summary>
        public GuidanceBrowserToolWindow() :
            base(null)
        {
            this.Caption = Resources.GuidanceBrowserToolWindow_WindowTitle;

            this.BitmapResourceID = 303;
            this.BitmapIndex = 0;
        }
    }
}
