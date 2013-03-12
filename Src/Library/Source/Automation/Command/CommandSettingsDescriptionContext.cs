namespace NuPattern.Library.Automation
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
