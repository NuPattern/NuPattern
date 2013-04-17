using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NuPattern;
using NuPattern.Diagnostics;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Launch point that controls visibility of templates.
    /// </summary>
    /// <remarks>
    /// This base implementation verifies that the feature where this launchpoint 
    /// is included has been instantiated, and then delegates to the derived 
    /// class to check additional conditions eventually.
    /// </remarks>
    [DisplayName("VS Template Launch Point")]
    internal abstract class VsTemplateLaunchPoint : ILaunchPoint
    {
        protected VsTemplateLaunchPoint(IFeatureManager featureManager, string id, string name, string category)
        {
            Guard.NotNull(() => featureManager, featureManager);

            this.FeatureManager = featureManager;
            this.Id = id;
            this.Name = name;
            this.Category = category;

            this.QueryStatusStrategy = new DefaultQueryStatusStrategy(this.GetType().Name);
            this.FeatureInstanceLocator = new DefaultFeatureInstanceLocator(featureManager, this.GetType());
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
        /// Gets the default behavior of the feature when it is unfolded.
        /// </summary>
        public virtual FeatureInstantiation FeatureInstantiation
        {
            get { return FeatureInstantiation.None; }
        }

        /// <summary>
        /// Gets wheather the template is unfolded with the feature.
        /// </summary>
        public virtual bool IsDefaultTemplate
        {
            get { return false; }
        }

        protected IFeatureManager FeatureManager { get; private set; }

        protected virtual IFeatureInstanceLocator FeatureInstanceLocator { get; set; }

        protected virtual IQueryStatusStrategy QueryStatusStrategy { get; set; }

        public virtual bool CanExecute(IFeatureExtension feature)
        {
            if (feature == null)
            {
                feature = this.FeatureInstanceLocator.LocateInstance();
            }

            if (this.FeatureInstantiation != FeatureInstantiation.None)
            {
                return true;
            }

            return this.QueryStatusStrategy.QueryStatus(feature).Enabled;
        }

        public virtual void Execute(IFeatureExtension feature)
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
            var registration = this.FeatureManager.FindFeature(this.GetType());
            var tracer = FeatureTracer.GetSourceFor(this, registration.FeatureId);

            var feature = this.FeatureManager.InstantiatedFeatures.FirstOrDefault(f => f.FeatureId == registration.FeatureId);
            if (feature != null)
            {
                var commandBinding = feature.Commands.FindByName(bindingName);
                if (commandBinding == null || !commandBinding.Evaluate())
                {
                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        "Command binding '{0}' can not be executed.",
                        bindingName));
                }

                using (tracer.StartActivity("Executing command {0}", commandBinding.Name))
                {
                    commandBinding.Value.Execute();
                }
            }
        }
    }
}