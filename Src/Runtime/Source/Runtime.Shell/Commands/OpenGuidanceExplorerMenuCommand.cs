using System;
using NuPattern.Runtime.Shell.ToolWindows;
using NuPattern.VisualStudio.Commands;

namespace NuPattern.Runtime.Shell.Commands
{
    /// <summary>
    /// Provides a Visual Studio command to open the guidance explorer tool window.
    /// </summary>
    internal class OpenGuidanceExplorerMenuCommand : VsMenuCommand
    {
        internal static readonly Guid CommandGroup = new Guid("86E00CF8-82FA-4C2B-A65D-CA5FEEDAB03F");
        internal const int CommandId = 0x0102;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGuidanceExplorerMenuCommand"/> class.
        /// </summary>
        /// <param name="toolWindow">The tool window.</param>
        public OpenGuidanceExplorerMenuCommand(IPackageToolWindow toolWindow)
            : base(new OpenGuidanceExplorerCommand(toolWindow), CommandGroup, CommandId)
        {
        }

        /// <summary>
        /// Defines a command to open the Guidance Explorer.
        /// </summary>
        private class OpenGuidanceExplorerCommand : NuPattern.VisualStudio.Commands.ICommand
        {
            private IPackageToolWindow toolWindow;

            /// <summary>
            /// Initializes a new instance of the <see cref="OpenGuidanceExplorerCommand"/> class.
            /// </summary>
            /// <param name="toolWindow">The tool window.</param>
            internal OpenGuidanceExplorerCommand(IPackageToolWindow toolWindow)
            {
                this.toolWindow = toolWindow;
            }

            /// <summary>
            /// Executes the command.
            /// </summary>
            public void Execute()
            {
                this.toolWindow.ShowWindow<GuidanceExplorerToolWindow>(true);
            }
        }
    }
}