using System.ComponentModel.Design;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Define extension methods related to <see cref="IMenuCommandService"/>.
	/// </summary>
	public static class MenuCommandServiceExtensions
	{
		/// <summary>
		/// Adds the command.
		/// </summary>
		/// <param name="menuCommandService">The menu command service.</param>
		/// <param name="command">The command.</param>
		public static void AddCommand(this IMenuCommandService menuCommandService, IVsMenuCommand command)
		{
			Guard.NotNull(() => menuCommandService, menuCommandService);
			Guard.NotNull(() => command, command);

			menuCommandService.AddCommand(new VsMenuCommandAdapter(command));
		}
	}
}