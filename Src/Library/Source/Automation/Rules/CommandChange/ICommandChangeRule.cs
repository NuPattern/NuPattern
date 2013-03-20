using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Library.Automation
{
	/// <summary>
	/// Notification rule for command property changes. To use attribute your class with CommandChangeRule[typeof(MyCommand))]
	/// and implement this interface.
	/// </summary>
	public interface ICommandChangeRule
	{
		/// <summary>
		/// Called when a property in the command changes.
		/// </summary>
		/// <param name="e"></param>
		void Change(ElementPropertyChangedEventArgs e);
	}
}
