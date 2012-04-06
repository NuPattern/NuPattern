using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility.References;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
    /// <summary>
    /// Validation rules for GuidanceReferences.
    /// </summary>
    internal class GuidanceReferenceValidation
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<GuidanceReferenceValidation>();

        /// <summary>
        /// Gets the feature manager.
        /// </summary>
        [Import]
        internal IFeatureManager FeatureManager { get; set; }

        [RuntimeValidationExtension]
        [ValidationMethod(CustomCategory = ValidationConstants.RuntimeValidationCategory)]
        internal void ValidateGuidanceReference(ValidationContext context, IProductElement element)
        {
            try
            {
                if (this.FeatureManager == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(element.TryGetReference(ReferenceKindConstants.Guidance)))
                {
                    var reference = GuidanceReference.GetResolvedReferences(element, this.FeatureManager).FirstOrDefault();
                    if (reference == null)
                    {
                        context.LogError(
                            string.Format(CultureInfo.InvariantCulture,
                                Properties.Resources.Validate_GuidanceReferenceNotFound,
                                element.InstanceName, reference.ToString(), typeof(GuidanceReference).DisplayName()),
                            Properties.Resources.Validate_GuidanceReferenceNotFoundCode,
                            element as ModelElement);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<GuidanceReferenceValidation>.GetMethod(n => n.ValidateGuidanceReference(context, element)).Name);

                throw;
            }
        }
    }
}
