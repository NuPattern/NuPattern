using Microsoft.VisualStudio.Modeling.ExtensionEnablement;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provides the interface for components that implement custom 
	/// query status behavior for menus.
	/// </summary>
	public interface ICommandStatus
	{
		/// <summary>
		/// Updates the status of the <paramref name="menu"/>.
		/// </summary>
		/// <param name="menu">The menu to update the status.</param>
		void QueryStatus(IMenuCommand menu);
	}
}
