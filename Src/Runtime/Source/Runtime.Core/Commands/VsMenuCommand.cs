using System;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using NuPattern.VisualStudio.Commands;

namespace NuPattern.Runtime.Commands
{
    /// <summary>
    /// Provides a generic way to executes commands in Visual Studio.
    /// </summary>
    internal class VsMenuCommand : IVsMenuCommand
    {
        private ICommand command;

        /// <summary>
        /// Initializes a new instance of the <see cref="VsMenuCommand"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="group">The group.</param>
        /// <param name="id">The id of the command.</param>
        public VsMenuCommand(ICommand command, Guid group, int id)
        {
            this.command = command;
            this.Group = group;
            this.Id = id;
        }

        /// <summary>
        /// Gets the group of the command.
        /// </summary>
        /// <value>The group of the command.</value>
        public Guid Group { get; private set; }

        /// <summary>
        /// Gets the id of the command.
        /// </summary>
        /// <value>The id of the command.</value>
        public int Id { get; private set; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public virtual void Execute()
        {
            this.command.Execute();
        }

        /// <summary>
        /// Queries the status of the command.
        /// </summary>
        /// <param name="adapter">The adapter to set the status.</param>
        public virtual void QueryStatus(IMenuCommand adapter)
        {
            Guard.NotNull(() => adapter, adapter);

            adapter.Enabled = adapter.Visible = true;
        }
    }
}