using System;
using System.ComponentModel.Composition;
using System.Globalization;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Diagnostics;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.Runtime.Validation;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Validation rules for ArtifactReferences.
    /// </summary>
    internal class ArtifactReferenceValidation
    {
        private static readonly ITracer tracer = Tracer.Get<ArtifactReferenceValidation>();

        public const string UriSchemeName = "solution";

        /// <summary>
        /// Gets the UriReferenceService.
        /// </summary>
        [Import]
        internal IUriReferenceService UriReferenceService { get; set; }

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
                var references = SolutionArtifactLinkReference.GetReferenceValues(element);

                foreach (var reference in references)
                {
                    if (this.UriReferenceService.TryResolveUri<IItemContainer>(reference) == null)
                    {
                        context.LogError(
                            string.Format(CultureInfo.InvariantCulture,
                                Properties.Resources.Validate_ArtifactReferenceNotFound,
                                element.InstanceName, reference.ToString(), ReflectionExtensions.DisplayName(typeof(SolutionArtifactLinkReference))),
                            Properties.Resources.Validate_ArtifactReferenceNotFoundCode,
                            element as ModelElement);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.Error(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<ArtifactReferenceValidation>.GetMethod(n => n.ValidateArtifactReferences(context, element)).Name);

                throw;
            }
        }
    }
}
