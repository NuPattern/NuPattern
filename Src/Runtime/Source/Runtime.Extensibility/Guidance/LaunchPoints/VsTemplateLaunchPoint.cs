using System;
using System.Globalization;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.LaunchPoints
{
    /// <summary>
    /// Launch point that controls visibility of templates.
    /// </summary>
    /// <remarks>
    /// This base implementation verifies that the guidance extension where this launchpoint 
    /// is included has been instantiated, and then delegates to the derived 
    /// class to check additional conditions eventually.
    /// </remarks>
    [DisplayNameResource(@"VsTemplateLaunchPoint_DisplayName", typeof(Resources))]
    internal abstract class VsTemplateLaunchPoint : ILaunchPoint
    {
        protected VsTemplateLaunchPoint(IGuidanceManager guidanceManager, string id, string name, string category)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);

            this.GuidanceManager = guidanceManager;
            this.Id = id;
            this.Name = name;
            this.Category = category;

            this.QueryStatusStrategy = new DefaultQueryStatusStrategy(this.GetType().Name);
            this.GuidanceInstanceLocator = new DefaultGuidanceInstanceLocator(guidanceManager, this.GetType());
        }

        /// <summary>
        /// Gets the name of template
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the category of the template
        /// </summary>
        public string Category { get; private set; }

        /// <summary>
        /// Gets the template id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the default behavior of the guidance extension when it is unfolded.
        /// </summary>
        public virtual GuidanceInstantiation GuidanceInstantiation
        {
            get { return GuidanceInstantiation.None; }
        }

        /// <summary>
        /// Gets wheather the template is unfolded with the guidance extension.
        /// </summary>
        public virtual bool IsDefaultTemplate
        {
            get { return false; }
        }

        protected IGuidanceManager GuidanceManager { get; private set; }

        protected virtual IGuidanceInstanceLocator GuidanceInstanceLocator { get; set; }

        protected virtual IQueryStatusStrategy QueryStatusStrategy { get; set; }

        public virtual bool CanExecute(IGuidanceExtension extension)
        {
            if (extension == null)
            {
                extension = this.GuidanceInstanceLocator.LocateInstance();
            }

            if (this.GuidanceInstantiation != GuidanceInstantiation.None)
            {
                return true;
            }

            return this.QueryStatusStrategy.QueryStatus(extension).Enabled;
        }

        public virtual void Execute(IGuidanceExtension extension)
        {
        }

        /// <summary>
        /// This logic is executed before unfolding the template
        /// </summary>
        public virtual void OnBeforeUnfoldTemplate()
        {
        }

        /// <summary>
        /// This logic is executed after unfolding the template
        /// </summary>
        public virtual void OnAfterUnfoldTemplate()
        {
        }

        protected void OnUnfoldTemplate(string bindingName)
        {
            var registration = this.GuidanceManager.FindGuidanceExtension(this.GetType());
            var tracer = Tracer.Get<VsTemplateLaunchPoint>();

            var extension = this.GuidanceManager.InstantiatedGuidanceExtensions.FirstOrDefault(f => f.ExtensionId == registration.ExtensionId);
            if (extension != null)
            {
                var commandBinding = extension.Commands.FindByName(bindingName);
                if (commandBinding == null || !commandBinding.Evaluate())
                {
                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.VsTemplateLaunchPoint_ErrorEvaluateFailed,
                        bindingName));
                }

                using (tracer.StartActivity(Resources.VsTemplateLaunchPoint_TraceExecuteCommand, commandBinding.Name))
                {
                    commandBinding.Value.Execute();
                }
            }
        }
    }
}