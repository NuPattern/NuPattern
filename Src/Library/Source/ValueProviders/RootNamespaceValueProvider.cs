using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides the project root namespace.
    /// </summary>
    [DisplayNameResource("RootNamespaceValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("RootNamespaceValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class RootNamespaceValueProvider : VsProjectPropertyValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<RootNamespaceValueProvider>();

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.RootNamespaceValueProvider_TraceInitial, this.CurrentElement.InstanceName, this.ProjectPath);

            return base.Evaluate();
        }

        /// <summary>
        /// Returns property value.
        /// </summary>
        /// <returns></returns>
        protected override object GetPropertyValue(IProject project)
        {
            Guard.NotNull(() => project, project);

            var result = project.Data.RootNamespace;

            tracer.TraceInformation(
                Resources.RootNamespaceValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.ProjectPath, result);

            return result;
        }
    }
}
