using System;
using System.ComponentModel.Composition;

namespace NuPattern.Library
{
	/// <summary>
	/// Notification rule for command property changes. To use attribute your class with CommandChangeRule[typeof(MyCommand))]
	/// and implement this interface.
	/// </summary>
	[CLSCompliant(false)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	[MetadataAttribute]
	public sealed class CommandValidationRuleAttribute : InheritedExportAttribute, ICommandValidationRuleMetadata
	{
		/// <summary>
		/// Creates a new command change rule attribute
		/// </summary>
		/// <param name="commandType">The type of the command to subscribe</param>
		public CommandValidationRuleAttribute(Type commandType)
			: base(typeof(ICommandValidationRule))
		{
			CommandType = commandType;
		}

		/// <summary>
		/// The type of the command to subscribe
		/// </summary>
		public Type CommandType { get; private set; }
	}
}
