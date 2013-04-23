namespace NuPattern.Library.Commands
{
	/// <summary>
	/// Validations for the <see cref="GenerateProductCodeCommand"/> command
	/// </summary>
	[CommandValidationRule(typeof(GenerateProductCodeCommand))]
	public class GenerateProductCodeCommandValidation : GenerateModelingCodeCommandValidation
	{
	}
}
