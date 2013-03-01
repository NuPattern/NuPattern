using System;

namespace NuPattern.Library.Automation
{
	/// <summary>
	/// Metadata for command change rules
	/// </summary>
	public interface ICommandChangeRuleMetadata
	{
		/// <summary>
		/// The type of the command to subscribe
		/// </summary>
		Type CommandType { get; }
	}
}
