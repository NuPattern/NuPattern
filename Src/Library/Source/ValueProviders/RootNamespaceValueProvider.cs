using System;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides the project root namespace.
    /// </summary>
    [DisplayNameResource(@"RootNamespaceValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"RootNamespaceValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_VisualStudio", typeof(Resources))]
    [CLSCompliant(false)]
    public class RootNamespaceValueProvider : VsProjectPropertyValueProvider
    {
        private static readonly ITracer tracer = Tracer.Get<RootNamespaceValueProvider>();

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.RootNamespaceValueProvider_TraceInitial, this.CurrentElement.InstanceName, this.TargetPath);

            return base.Evaluate();
        }

        /// <summary>
        /// Returns property value.
        /// </summary>
        /// <returns></returns>
        protected override object GetPropertyValue(IProject project)
        {
            Guard.NotNull(() => project, project);

            var result = (string)project.Data.RootNamespace;

            tracer.Info(
                Resources.RootNamespaceValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.TargetPath, result);

            return result;
        }
    }
}
