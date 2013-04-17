using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides the project assembly name.
    /// </summary>
    [DisplayNameResource("AssemblyNameValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("AssemblyNameValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class AssemblyNameValueProvider : VsProjectPropertyValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<AssemblyNameValueProvider>();

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.AssemblyNameValueProvider_TraceInitial, this.CurrentElement.InstanceName, this.ProjectPath);

            return base.Evaluate();
        }

        /// <summary>
        /// Returns property value.
        /// </summary>
        /// <returns></returns>
        protected override object GetPropertyValue(IProject project)
        {
            Guard.NotNull(() => project, project);

            var result = project.Data.AssemblyName;

            tracer.TraceInformation(
                Resources.AssemblyNameValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.ProjectPath, result);

            return result;
        }
    }
}