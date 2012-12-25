
namespace NuPattern.Runtime
{
    /// <summary>
    /// Interface to nteract with the VS status bar
    /// </summary>
    public interface IStatusBar
    {
		/// <summary>
		/// Displays a message in the status bar
		/// </summary>
		/// <param name="message">The message to be displayed</param>
        void DisplayMessage(string message);
    }
}
