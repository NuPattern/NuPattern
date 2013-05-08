using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides a project property.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class VsProjectPropertyValueProvider : ValueProvider
    {
        private static readonly ITracer tracer = Tracer.Get<VsProjectPropertyValueProvider>();

        /// <summary>
        /// The element that owns this value provider.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement
        {
            get;
            set;
        }

        /// <summary>
        /// The URI service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IUriReferenceService UriService
        {
            get;
            set;
        }

        /// <summary>
        /// The current solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution
        {
            get;
            set;
        }

        /// <summary>
        /// The project path.
        /// </summary>
        [DisplayNameResource(@"VsProjectPropertyValueProvider_ProjectPath_DisplayName", typeof(Resources))]
        [DescriptionResource(@"VsProjectPropertyValueProvider_ProjectPath_Description", typeof(Resources))]
        public virtual string ProjectPath
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

            tracer.Info(
                Resources.VsProjectPropertyValueProvider_TraceInitial, this.CurrentElement.InstanceName, this.ProjectPath);

            var resolver = new PathResolver(this.CurrentElement, this.UriService, path: (!String.IsNullOrEmpty(this.ProjectPath)) ? this.ProjectPath : String.Empty);
            if (!resolver.TryResolve(i => i.Kind == ItemKind.Project))
            {
                tracer.Error(
                    Resources.VsProjectPropertyValueProvider_TraceNotResolved, this.ProjectPath);

                return string.Empty;
            }

            var item = this.Solution.Find(resolver.Path).FirstOrDefault();

            if (item == null)
            {
                tracer.Warn(
                    Resources.VsProjectPropertyValueProvider_TraceNoItemFound, this.ProjectPath, resolver.Path);

                return string.Empty;
            }
            else
            {
                if (item.Kind == ItemKind.Project)
                {
                    var project = (IProject)item;
                    var propValue = GetPropertyValue(project);
                    
                    tracer.Info(
                        Resources.VsProjectPropertyValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.ProjectPath, propValue);

                    return propValue;
                }
                else
                {
                    tracer.Warn(
                        Resources.VsProjectPropertyValueProvider_TraceItemNotProject, this.ProjectPath, resolver.Path, item.Kind);

                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Returns the value of the project property.
        /// </summary>
        protected abstract object GetPropertyValue(IProject project);
    }
}