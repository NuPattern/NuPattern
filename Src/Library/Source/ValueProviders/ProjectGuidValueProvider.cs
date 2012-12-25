using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides the project guid.
    /// </summary>
    [DisplayNameResource("ProjectGuidValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("ProjectGuidValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class ProjectGuidValueProvider : VsProjectPropertyValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ProjectGuidValueProvider>();

        /// <summary>
        /// Creates a new instance of the <see cref="RandomGuidValueProvider"/> class.
        /// </summary>
        public ProjectGuidValueProvider()
            : base()
        {
            this.Format = GuidFormat.JustDigitsWithHyphens;
        }

        /// <summary>
        /// The format of the returned guid.
        /// </summary>
        [DisplayNameResource("ProjectGuidValueProvider_Format_DisplayName", typeof(Resources))]
        [DescriptionResource("ProjectGuidValueProvider_Format_Description", typeof(Resources))]
        [DefaultValue(GuidFormat.JustDigitsWithHyphens)]
        [Required]
        public GuidFormat Format
        {
            get;
            set;
        }

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ProjectGuidValueProvider_TraceInitial, this.CurrentElement.InstanceName, this.ProjectPath, this.Format);

            return base.Evaluate();
        }

        /// <summary>
        /// Returns property value.
        /// </summary>
        /// <returns></returns>
        protected override object GetPropertyValue(IProject project)
        {
            Guard.NotNull(() => project, project);

            var guidString = project.Data.ProjectGuid;
            if (guidString != null)
            {
                var result = new Guid(guidString).ToString(GuidHelper.GetFormat(this.Format));

                tracer.TraceInformation(
                    Resources.ProjectGuidValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.ProjectPath, result);

                return result;
            }

            return null;
        }
    }
}