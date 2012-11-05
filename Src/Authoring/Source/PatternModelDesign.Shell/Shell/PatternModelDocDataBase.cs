using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Validation;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal abstract partial class PatternModelDocDataBase
	{
		/// <summary>
		/// Sets the validation extension registrar.
		/// </summary>
		/// <param name="validationController">The validation controller.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", Justification = "Not Applicable", MessageId = "validationController"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Not Applicable")]
		partial void SetValidationExtensionRegistrar(ValidationController validationController)
		{
			var validationExtensionRegistrar = new AuthoringValidationExtensionRegistrar();

			if (ModelingCompositionContainer.CompositionService != null)
			{
				ModelingCompositionContainer.CompositionService.SatisfyImportsOnce(validationExtensionRegistrar);
			}

			if (validationController != null)
			{
				validationController.ValidationExtensionRegistrar = validationExtensionRegistrar;
			}
		}
	}
}