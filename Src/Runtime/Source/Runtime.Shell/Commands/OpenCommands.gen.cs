using System;
using NuPattern.Runtime.Shell.ToolWindows;
using NuPattern.VisualStudio.Commands;

namespace NuPattern.Runtime.Shell.Commands
{

    /// <summary>
    /// Provides a Visual Studio command to open the SolutionBuilder tool window.
    /// </summary>
    internal class OpenSolutionBuilderMenuCommand : VsMenuCommand
    {
        internal static readonly Guid CommandGroup = new Guid("43649871-de44-4b36-8745-73b8ffd737b3");
        internal const int CommandId = 0x0101;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenSolutionBuilderMenuCommand"/> class.
        /// </summary>
        /// <param name="toolWindow">The tool window.</param>
        public OpenSolutionBuilderMenuCommand(IPackageToolWindow toolWindow)
            : base(new OpenSolutionBuilderCommand(toolWindow), CommandGroup, CommandId)
        {
        }

        /// <summary>
        /// Defines a command to open the SolutionBuilder window.
        /// </summary>
        private class OpenSolutionBuilderCommand : NuPattern.VisualStudio.Commands.ICommand
        {
            private IPackageToolWindow toolWindow;

            /// <summary>
            /// Initializes a new instance of the <see cref="OpenSolutionBuilderCommand"/> class.
            /// </summary>
            /// <param name="toolWindow">The tool window.</param>
            internal OpenSolutionBuilderCommand(IPackageToolWindow toolWindow)
            {
                this.toolWindow = toolWindow;
            }

            /// <summary>
            /// Executes the command.
            /// </summary>
            public void Execute()
            {
                this.toolWindow.ShowWindow<SolutionBuilderToolWindow>(true);
            }
        }
    }

    /// <summary>
    /// Provides a Visual Studio command to open the GuidanceExplorer tool window.
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
        /// Defines a command to open the GuidanceExplorer window.
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

    /// <summary>
    /// Provides a Visual Studio command to open the GuidanceBrowser tool window.
    /// </summary>
    internal class OpenGuidanceBrowserMenuCommand : VsMenuCommand
    {
        internal static readonly Guid CommandGroup = new Guid("678B7F72-E43F-46DE-AFFC-365896827954");
        internal const int CommandId = 0x0103;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGuidanceBrowserMenuCommand"/> class.
        /// </summary>
        /// <param name="toolWindow">The tool window.</param>
        public OpenGuidanceBrowserMenuCommand(IPackageToolWindow toolWindow)
            : base(new OpenGuidanceBrowserCommand(toolWindow), CommandGroup, CommandId)
        {
        }

        /// <summary>
        /// Defines a command to open the GuidanceBrowser window.
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
                this.toolWindow.ShowWindow<GuidanceBrowserToolWindow>(true);
            }
        }
    }
}
