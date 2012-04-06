using System.Windows.Input;

namespace Microsoft.VisualStudio.Patterning.Common.Presentation
{
	/// <summary>
	/// Provides a standard set of navigation-related commands.
	/// </summary>
	public static class WizardCommands
	{
		private static RoutedUICommand finishWizard;

		/// <summary>
		/// Gets the value that represents the Next Page command.
		/// </summary>
		/// <value>The Next Page command.</value>
		public static RoutedUICommand NextPage
		{
			get { return NavigationCommands.BrowseForward; }
		}

		/// <summary>
		/// Gets the finish wizard command.
		/// </summary>
		/// <value>The finish wizard command.</value>
		public static RoutedUICommand FinishWizard
		{
			get
			{
				if (finishWizard == null)
				{
					finishWizard = new RoutedUICommand();
				}

				return finishWizard;
			}
		}

		/// <summary>
		/// Gets the value that represents the Previous Page command.
		/// </summary>
		/// <value>The Previous Page command.</value>
		public static RoutedUICommand PreviousPage
		{
			get { return NavigationCommands.BrowseBack; }
		}
	}
}