using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
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
