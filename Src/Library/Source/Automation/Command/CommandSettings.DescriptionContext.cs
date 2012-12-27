namespace NuPattern.Library.Automation.Command
{
	internal class CommandSettingsDescriptionContext
	{
		private CommandSettingsDescriptionContext()
		{
		}

		static CommandSettingsDescriptionContext()
		{
			Current = new CommandSettingsDescriptionContext();

		}

		public static CommandSettingsDescriptionContext Current { get; internal set; }
	}
}
