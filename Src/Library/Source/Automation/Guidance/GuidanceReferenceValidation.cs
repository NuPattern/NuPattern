using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Extensibility;
using NuPattern.Runtime.References;
using NuPattern.Runtime.Validation;
using ReflectionExtensions = Microsoft.VisualStudio.TeamArchitect.PowerTools.ReflectionExtensions;

namespace NuPattern.Library.Automation
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

                var reference = element.TryGetReference(ReferenceKindConstants.Guidance);
                if (!string.IsNullOrEmpty(reference))
                {
                    var uri = GuidanceReference.GetResolvedReferences(element, this.FeatureManager).FirstOrDefault();
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
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<GuidanceReferenceValidation>.GetMethod(n => n.ValidateGuidanceReference(context, element)).Name);

                throw;
            }
        }
    }
}
