using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Diagnostics;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.References;
using NuPattern.Runtime.Validation;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Validation rules for GuidanceReferences.
    /// </summary>
    internal class GuidanceReferenceValidation
    {
        private static readonly ITracer tracer = Tracer.Get<GuidanceReferenceValidation>();

        /// <summary>
        /// Gets the feature manager.
        /// </summary>
        [Import]
        internal IGuidanceManager GuidanceManager { get; set; }

        [RuntimeValidationExtension]
        [ValidationMethod(CustomCategory = ValidationConstants.RuntimeValidationCategory)]
        internal void ValidateGuidanceReference(ValidationContext context, IProductElement element)
        {
            try
            {
                if (this.GuidanceManager == null)
                {
                    return;
                }

                var reference = element.TryGetReference(ReferenceKindConstants.Guidance);
                if (!string.IsNullOrEmpty(reference))
                {
                    var uri = GuidanceReference.GetResolvedReferences(element, this.GuidanceManager).FirstOrDefault();
                    if (uri == null)
                    {
                        context.LogError(
                            string.Format(CultureInfo.InvariantCulture,
                                Properties.Resources.Validate_GuidanceReferenceNotFound,
                                element.InstanceName, reference, ReflectionExtensions.DisplayName(typeof(GuidanceReference))),
                            Properties.Resources.Validate_GuidanceReferenceNotFoundCode,
                            element as ModelElement);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.Error(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<GuidanceReferenceValidation>.GetMethod(n => n.ValidateGuidanceReference(context, element)).Name);

                throw;
            }
        }
    }
}
