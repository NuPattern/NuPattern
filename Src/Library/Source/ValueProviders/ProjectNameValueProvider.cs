using System;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides the project name.
    /// </summary>
    [DisplayNameResource(@"ProjectNameValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ProjectNameValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_VisualStudio", typeof(Resources))]
    [CLSCompliant(false)]
    public class ProjectNameValueProvider : VsProjectPropertyValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ProjectNameValueProvider>();

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ProjectNameValueProvider_TraceInitial, this.CurrentElement.InstanceName, this.ProjectPath);

            return base.Evaluate();
        }

        /// <summary>
        /// Returns property value.
        /// </summary>
        /// <returns></returns>
        protected override object GetPropertyValue(IProject project)
        {
            Guard.NotNull(() => project, project);

            var result = project.Name;

            tracer.TraceInformation(
                Resources.ProjectNameValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.ProjectPath, result);

            return result;
        }
    }
}
