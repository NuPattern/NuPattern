using System;
using System.ComponentModel.Composition;
using System.Globalization;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Validation rules for ArtifactReferences.
    /// </summary>
    internal class ArtifactReferenceValidation
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ArtifactReferenceValidation>();

        public const string UriSchemeName = "solution";

        /// <summary>
        /// Gets the UriReferenceService.
        /// </summary>
        [Import]
        internal IFxrUriReferenceService UriReferenceService { get; set; }

        /// <summary>
        /// Verifies that artifact links on elements are valid.
        /// </summary>
        [RuntimeValidationExtension]
        [ValidationMethod(CustomCategory = ValidationConstants.RuntimeValidationCategory)]
        internal void ValidateArtifactReferences(ValidationContext context, IProductElement element)
        {
            if (this.UriReferenceService == null)
            {
                return;
            }

            try
            {
                var references = SolutionArtifactLinkReference.GetReferences(element);

                foreach (var reference in references)
                {
                    if (this.UriReferenceService.TryResolveUri<IItemContainer>(reference) == null)
                    {
                        context.LogError(
                            string.Format(CultureInfo.InvariantCulture,
                                Properties.Resources.Validate_ArtifactReferenceNotFound,
                                element.InstanceName, reference.ToString(), typeof(SolutionArtifactLinkReference).DisplayName()),
                            Properties.Resources.Validate_ArtifactReferenceNotFoundCode,
                            element as ModelElement);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<ArtifactReferenceValidation>.GetMethod(n => n.ValidateArtifactReferences(context, element)).Name);

                throw;
            }
        }
    }
}
