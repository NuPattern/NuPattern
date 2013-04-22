using System;
using System.ComponentModel.Composition;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Runtime;

namespace NuPattern.Authoring.PatternToolkit.Automation.ValueProviders
{
    /// <summary>
    /// Provides the full path to the associated guidance document.
    /// </summary>
    [DisplayNameResource("GuidanceDocumentPathProvider_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("GuidanceDocumentPathProvider_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class GuidanceDocumentPathProvider : NuPattern.Runtime.ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<GuidanceDocumentPathProvider>();

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
        public IUriReferenceService UriReferenceService
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the result of evaluation of this provider.
        /// </summary>
        /// <remarks></remarks>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.GuidanceDocumentPathProvider_TraceInitial, this.CurrentElement.InstanceName);

            var result = GuidanceDocumentHelper.GetDocumentPath(tracer, this.CurrentElement, this.UriReferenceService);

            tracer.TraceInformation(
                Resources.GuidanceDocumentPathProvider_TraceEvaluation, this.CurrentElement.InstanceName, result);

            return result;
        }
    }
}
