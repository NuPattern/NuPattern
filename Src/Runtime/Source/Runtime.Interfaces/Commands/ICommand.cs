namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Defines a command to abstract the execution of a Visual Studio command.
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// Executes the command.
		/// </summary>
		void Execute();
	}
}