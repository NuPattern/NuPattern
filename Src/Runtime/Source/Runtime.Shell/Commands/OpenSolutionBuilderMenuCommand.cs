using System;
using NuPattern.Runtime.Shell.ToolWindows;
using NuPattern.VisualStudio.Commands;

namespace NuPattern.Runtime.Shell.Commands
{
    /// <summary>
    /// Provides a Visual Studio command to open the pattern explorer tool window.
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
        /// Defines a command to open the Pattern Explorer.
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
}