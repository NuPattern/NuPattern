namespace NuPattern.Library.Design
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
