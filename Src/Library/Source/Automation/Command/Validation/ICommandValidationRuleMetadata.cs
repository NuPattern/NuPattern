using System;

namespace NuPattern.Library
{
	/// <summary>
	/// Metadata for the  <see cref="ICommandValidationRule"/>
	/// </summary>
    internal interface ICommandValidationRuleMetadata
	{
		/// <summary>
		/// The type of the command to which the validations apply
		/// </summary>
		Type CommandType { get; }
	}
}
