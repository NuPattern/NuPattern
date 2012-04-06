using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Patterning.Library
{
	/// <summary>
	/// Metadata for the  <see cref="ICommandValidationRule"/>
	/// </summary>
	public interface ICommandValidationRuleMetadata
	{
		/// <summary>
		/// The type of the command to which the validations apply
		/// </summary>
		Type CommandType { get; }
	}
}
