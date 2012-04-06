using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
	/// <summary>
	/// Validations for the <see cref="GenerateProductCodeCommand"/> command
	/// </summary>
	[CommandValidationRule(typeof(GenerateProductCodeCommand))]
	public class GenerateProductCodeCommandValidation : GenerateModelingCodeCommandValidation
	{
	}
}
