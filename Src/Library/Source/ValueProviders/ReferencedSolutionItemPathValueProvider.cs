using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// Provides the physical path for the first resolved artifact link.
    /// </summary>
    [DisplayNameResource("ReferencedSolutionItemPathValueProvider_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("ReferencedSolutionItemPathValueProvider_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ReferencedSolutionItemPathValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ReferencedSolutionItemPathValueProvider>();

        /// <summary>
        /// Gets the current element.
        /// </summary>
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the URI reference service.
        /// </summary>
        [Import(AllowDefault = true)]
        public IFxrUriReferenceService UriReferenceService
        {
            get;
            set;
        }

        /// <summary>
        /// The extension of the item to return
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("ReferencedSolutionItemPathValueProvider_Extension_DisplayName", typeof(Resources))]
        [DescriptionResource("ReferencedSolutionItemPathValueProvider_Extension_Description", typeof(Resources))]
        public string Extension
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the first artifact link of given file extension.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ReferencedSolutionItemPathValueProvider_TraceInitial, this.CurrentElement.InstanceName, this.Extension);

            var extension = this.Extension
                .Replace("*", string.Empty)
                .Replace(" ", string.Empty);
            if (!extension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
            {
                extension = "." + extension;
            }

            var item =
                SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement, this.UriReferenceService).FirstOrDefault(
                    r => r.PhysicalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                var result = item.PhysicalPath;

                tracer.TraceInformation(
                    Resources.ReferencedSolutionItemPathValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.Extension, result);

                return result;
            }
            else
            {
                tracer.TraceWarning(
                    Resources.ReferencedSolutionItemPathValueProvider_TraceNoFile, this.CurrentElement.InstanceName, this.Extension);
                return null;
            }
        }
    }
}
