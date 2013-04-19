using System;
using NuPattern.Runtime.Shell.ToolWindows;
using NuPattern.VisualStudio.Commands;

namespace NuPattern.Runtime.Shell.Commands
{
    /// <summary>
    /// Provides a Visual Studio command to open the guidance browser tool window.
    /// </summary>
    internal class OpenGuidanceBrowserMenuCommand : VsMenuCommand
    {
        internal static readonly Guid CommandGroup = new Guid("678B7F72-E43F-46DE-AFFC-365896827954");
        internal const int CommandId = 0x0101;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGuidanceBrowserMenuCommand"/> class.
        /// </summary>
        /// <param name="toolWindow">The tool window.</param>
        public OpenGuidanceBrowserMenuCommand(IPackageToolWindow toolWindow)
            : base(new OpenGuidanceBrowserCommand(toolWindow), CommandGroup, CommandId)
        {
        }

        /// <summary>
        /// Defines a command to open the Guidance Browser.
        /// </summary>
        private class OpenGuidanceBrowserCommand : NuPattern.VisualStudio.Commands.ICommand
        {
            private IPackageToolWindow toolWindow;

            /// <summary>
            /// Initializes a new instance of the <see cref="OpenGuidanceBrowserCommand"/> class.
            /// </summary>
            /// <param name="toolWindow">The tool window.</param>
            internal OpenGuidanceBrowserCommand(IPackageToolWindow toolWindow)
            {
                this.toolWindow = toolWindow;
            }

            /// <summary>
            /// Executes the command.
            /// </summary>
            public void Execute()
            {
                this.toolWindow.ShowWindow<GuidanceBrowserToolWindow>();
            }
        }
    }
}