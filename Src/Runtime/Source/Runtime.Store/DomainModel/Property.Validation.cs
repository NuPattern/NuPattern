using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Reflection;
using NuPattern.Runtime.Store.Properties;
using NuPattern.Runtime.Validation;

namespace NuPattern.Runtime.Store
{
    [ValidationState(ValidationState.Enabled)]
    partial class Property
    {
        /// <summary>
        /// Triggers this notification rule whether a <see cref="Property"/> is saved.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save)]
        public void OnSaved(ValidationContext context)
        {
            Guard.NotNull(() => context, context);

            this.SaveProvidedValue();
        }

        [ValidationMethod(ValidationCategories.Custom, CustomCategory = ValidationConstants.RuntimeValidationCategory)]
        private void ValidateInstance(ValidationContext context)
        {
            try
            {
                if (this.Info == null || this.Info.ValidationSettings == null || !this.Info.ValidationSettings.Any())
                    return;

                foreach (var binding in this.ValidationBindings)
                {
                    if (binding.Evaluate(this.BindingContext))
                    {
                        var results = binding.Value.Validate();
                        if (results != null) // null result is considered Success
                        {
                            foreach (var result in results)
                            {
                                context.LogError(result.ErrorMessage, Resources.Property_ValidationFailedErrorCode, this);
                            }
                        }
                    }
                    else
                    {
                        context.LogError(Resources.Property_BindingEvaluationFailed, Resources.Property_BindingEvaluationErrorCode, this);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<Property>.GetMethod(n => n.ValidateInstance(context)).Name);

                throw;
            }
        }
    }
}
