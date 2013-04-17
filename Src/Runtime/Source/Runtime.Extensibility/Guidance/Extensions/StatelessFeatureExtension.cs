using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using NuPattern.VisualStudio.Solution;


namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    public class StatelessFeatureExtension<TGeneratedWorkflow> : StatelessFeatureExtension
        where TGeneratedWorkflow : GuidanceWorkflow, new()
    {

        public override IEnumerable<ICommandBinding> Commands
        {
            get { yield break; }
        }

        protected internal override IGuidanceWorkflow OnCreateWorkflow()
        {
            var workflow = new TGeneratedWorkflow();
            this.FeatureComposition.SatisfyImportsOnce(workflow);
            workflow.OwningFeature = this;
            workflow.Initialize();

            return workflow;
        }
    }

    /// <summary>
    /// Implements a Feature Extension base class that does not store workflow state
    /// </summary>
    public abstract class StatelessFeatureExtension : FeatureExtension, IDisposable
    {
        public const string FeatureProjectCategory = "Stateless";

        ~StatelessFeatureExtension()
        {
            this.Dispose(false);
        }

        public IProject IdeProject { get; private set; }

        protected internal abstract IGuidanceWorkflow OnCreateWorkflow();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override IGuidanceWorkflow CreateWorkflow()
        {
            IGuidanceWorkflow newWF = this.OnCreateWorkflow();
            return newWF;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected internal override void OnFinish()
        {
        }

        protected internal override void OnInitialize(Version persistedVersion)
        {
            base.OnInitialize(persistedVersion);
        }

        protected internal override void OnInstantiate()
        {
        }
    }
}