using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Shell;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provide an adapter between an <see cref="IVsMenuCommand"/> and a <see cref="IMenuCommand"/>.
	/// </summary>
    [CLSCompliant(false)]
	public class VsMenuCommandAdapter : OleMenuCommand, IMenuCommand
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VsMenuCommandAdapter"/> class.
		/// </summary>
		/// <param name="command">The command.</param>
		public VsMenuCommandAdapter(IVsMenuCommand command)
			: base((s, e) => command.Execute(), null, (s, e) => command.QueryStatus((IMenuCommand)s), GetCommandId(command))
		{
		}

		private static CommandID GetCommandId(IVsMenuCommand command)
		{
			Guard.NotNull(() => command, command);

			return new CommandID(command.Group, command.Id);
		}
	}
}