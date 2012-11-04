using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Library
{
    /// <summary>
    /// Implementing classes when attributed with <see cref="CommandValidationRuleAttribute"/>  will be part of the validation
    /// </summary>
    public interface ICommandValidationRule
    {
        /// <summary>
        /// Called when Validation is needed for the command
        /// </summary>
        /// <param name="context">Context where to add validation information</param>
        /// <param name="settingsElement">The settings element in the model being validated</param>
        /// <param name="settings">Command Settings for the command that is being validated</param>
		void Validate(ValidationContext context, IAutomationSettingsSchema settingsElement, ICommandSettings settings);
    }
}
