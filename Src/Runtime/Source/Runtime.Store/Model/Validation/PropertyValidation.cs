using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// Performs runtime property validation.
    /// </summary>
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
    }
}