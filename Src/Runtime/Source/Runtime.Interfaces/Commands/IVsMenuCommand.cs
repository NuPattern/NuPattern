using System;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Defines a Visual Studio command.
	/// </summary>
	public interface IVsMenuCommand
	{
		/// <summary>
		/// Gets the group of the command.
		/// </summary>
		/// <value>The group of the command.</value>
		Guid Group { get; }

		/// <summary>
		/// Gets the id of the command.
		/// </summary>
		/// <value>The id of the command.</value>
		int Id { get; }

		/// <summary>
		/// Queries the status of the command.
		/// </summary>
		/// <param name="adapter">The adapter to set the status.</param>
		void QueryStatus(IMenuCommand adapter);

		/// <summary>
		/// Executes the command.
		/// </summary>
		void Execute();
	}
}