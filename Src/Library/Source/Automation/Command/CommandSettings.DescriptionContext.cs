using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Patterning.Library.Automation.Command
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
