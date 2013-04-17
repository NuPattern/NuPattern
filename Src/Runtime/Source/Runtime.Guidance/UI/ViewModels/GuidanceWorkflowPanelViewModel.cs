using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NuPattern.Presentation;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    internal class GuidanceWorkflowPanelViewModel : ViewModel
    {
        private GuidanceWorkflowContext context;
        private IServiceProvider serviceProvider;
        private GuidanceWorkflowViewModel currentWorkflow;
        private IFeatureManager featureManager;

        public GuidanceWorkflowPanelViewModel(GuidanceWorkflowContext context, IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.context = context;
            this.serviceProvider = serviceProvider;
            this.featureManager = context.FeatureExtension.FeatureManager;
            this.Workflows = new ObservableCollection<GuidanceWorkflowViewModel>();
            this.SubscribeToFeaturesChanges(featureManager);
            this.InitializeCommands();
        }

        public event EventHandler<NodeChangedEventArgs> CurrentNodeChanged = (s, e) => { };

        public ICommand CollapseAllCommand { get; private set; }

        public GuidanceWorkflowViewModel CurrentWorkflow
        {
            get
            {
                return this.currentWorkflow;
            }
            set
            {
                if (this.currentWorkflow != value)
                {
                    var previousWorkflow = this.currentWorkflow;
                    this.featureManager.ActiveFeature = value == null ? null : value.Feature;
                    this.currentWorkflow = value;
                    this.OnPropertyChanged(() => this.CurrentWorkflow);
                    if (previousWorkflow != null && previousWorkflow.CurrentNode != null && value == null)
                    {
                        this.CurrentNodeChanged(this, new NodeChangedEventArgs(null));
                    }
                }
            }
        }

        public ICommand ExpandAllCommand { get; private set; }

        public ObservableCollection<GuidanceWorkflowViewModel> Workflows { get; private set; }

        public ICommand GoToFocusedCommand { get; private set; }

        public ICommand RefreshGraphCommand { get; private set; }

        private void InitializeCommands()
        {
            this.ExpandAllCommand = new RelayCommand(
                () => this.ChangeIsExpanded(true),
                () => this.currentWorkflow != null && this.currentWorkflow.Nodes.Any());

            this.CollapseAllCommand = new RelayCommand(
                () => this.ChangeIsExpanded(false),
                () => this.currentWorkflow != null && this.currentWorkflow.Nodes.Any());

            this.RefreshGraphCommand = new RelayCommand(
                () => this.currentWorkflow.RefreshGraphStates(),
                () => this.currentWorkflow != null && this.currentWorkflow.Nodes.Any());

            this.GoToFocusedCommand = new RelayCommand(
                () => this.currentWorkflow.GoToFocusedAction(),
                () => this.currentWorkflow != null && this.currentWorkflow.Model.FocusedAction != null);
        }

        private void ChangeIsExpanded(bool isExpanded)
        {
            foreach (var node in this.CurrentWorkflow.Nodes.Traverse(n => n.Nodes))
            {
                node.IsExpanded = isExpanded;
            }
        }

        private void OnCurrentNodeChanged(object sender, EventArgs e)
        {
            this.CurrentNodeChanged(this, new NodeChangedEventArgs(this.CurrentWorkflow.CurrentNode.Node));
        }

        private void SetWorkflows(IEnumerable<IFeatureExtension> features)
        {
            // Match the untouched ViewModels
            var inmutedWorkflows = this.Workflows.Where(w => features.Any(f => f.GuidanceWorkflow == w.Model));

            // Remove all features removed
            foreach (var workflow in this.Workflows.Except(inmutedWorkflows).ToArray())
            {
                workflow.CurrentNodeChanged -= this.OnCurrentNodeChanged;
                this.Workflows.Remove(workflow);
            }

            // Add all features added
            var addedFeatures = features.Where(f => !this.Workflows.Any(w => w.Model == f.GuidanceWorkflow));
            foreach (var feature in addedFeatures)
            {
                if (feature.GuidanceWorkflow != null)
                {
                    var workflow = new GuidanceWorkflowViewModel(new GuidanceWorkflowContext
                        {
                            FeatureExtension = feature,
                            UserMessageService = this.context.UserMessageService,
                        }, this.serviceProvider);
                    this.Workflows.Add(workflow);
                    workflow.CurrentNodeChanged += this.OnCurrentNodeChanged;
                }
            }

            if (this.CurrentWorkflow != null && !features.Any(f => f.GuidanceWorkflow == this.CurrentWorkflow.Model))
            {
                this.CurrentWorkflow = null;
            }
        }

        private void SubscribeToFeaturesChanges(IFeatureManager featureManager)
        {
            featureManager.InstantiatedFeaturesChanged += (s, e) => this.SetWorkflows(featureManager.InstantiatedFeatures);
            this.SetWorkflows(featureManager.InstantiatedFeatures);

            featureManager.ActiveFeatureChanged += (s, e) =>
            {
                if (featureManager.ActiveFeature == null)
                    this.CurrentWorkflow = null;
                else
                {
                    if (featureManager.ActiveFeature.GuidanceWorkflow != null)
                    {
                        this.CurrentWorkflow = this.Workflows.FirstOrDefault(w => w.Model == featureManager.ActiveFeature.GuidanceWorkflow);
                        GuidanceConditionsEvaluator.EvaluateGraph(featureManager.ActiveFeature);
                    }
                }
            };
        }
    }
}
