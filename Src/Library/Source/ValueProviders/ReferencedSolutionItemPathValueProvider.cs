using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.References;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// Provides the physical path for the first resolved artifact link.
    /// </summary>
    [DisplayNameResource(@"ReferencedSolutionItemPathValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ReferencedSolutionItemPathValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_VisualStudio", typeof(Resources))]
    [CLSCompliant(false)]
    public class ReferencedSolutionItemPathValueProvider : ValueProvider
    {
        private static readonly ITracer tracer = Tracer.Get<ReferencedSolutionItemPathValueProvider>();

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
        /// The extension of the item to return
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"ReferencedSolutionItemPathValueProvider_Extension_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ReferencedSolutionItemPathValueProvider_Extension_Description", typeof(Resources))]
        public string Extension
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an optional tag to filter which solution items get considered
        /// </summary>
        [DisplayNameResource(@"ReferencedSolutionItemPathValueProvider_Tag_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ReferencedSolutionItemPathValueProvider_Tag_Description", typeof(Resources))]
        [DefaultValue("")]
        public string Tag { get; set; }

        /// <summary>
        /// Gets the first artifact link of given file extension.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.ReferencedSolutionItemPathValueProvider_TraceInitial, this.CurrentElement.InstanceName, this.Extension);

            var extension = this.Extension
                .Replace(@"*", string.Empty)
                .Replace(@" ", string.Empty);
            if (!extension.StartsWith(@".", StringComparison.OrdinalIgnoreCase))
            {
                extension = @"." + extension;
            }

            var tagFilter = new Func<IReference, bool>(x => true);
            if (!string.IsNullOrEmpty(this.Tag))
            {
                tagFilter = r => r.ContainsTag(this.Tag);
            }

            var item =
                SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement, this.UriReferenceService, tagFilter)
                .FirstOrDefault(r => r.PhysicalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                var result = item.PhysicalPath;

                tracer.Info(
                    Resources.ReferencedSolutionItemPathValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.Extension, result);

                return result;
            }
            else
            {
                tracer.Warn(
                    Resources.ReferencedSolutionItemPathValueProvider_TraceNoFile, this.CurrentElement.InstanceName, this.Extension);
                return null;
            }
        }
    }
}
