using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NuPattern.Presentation;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    internal class GuidanceExplorerViewModel : ViewModel
    {
        private GuidanceWorkflowContext context;
        private IServiceProvider serviceProvider;
        private GuidanceWorkflowViewModel currentWorkflow;
        private IGuidanceManager guidanceManager;

        public GuidanceExplorerViewModel(GuidanceWorkflowContext context, IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.context = context;
            this.serviceProvider = serviceProvider;
            this.guidanceManager = context.GuidanceManager;
            this.Workflows = new ObservableCollection<GuidanceWorkflowViewModel>();
            this.SubscribeToExtensionChanges(guidanceManager);
            this.InitializeCommands();
        }

        public event EventHandler<NodeChangedEventArgs> CurrentNodeChanged = (s, e) => { };

        public System.Windows.Input.ICommand CollapseAllCommand { get; private set; }

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
                    this.guidanceManager.ActiveGuidanceExtension = value == null ? null : value.Extension;
                    this.currentWorkflow = value;
                    this.OnPropertyChanged(() => this.CurrentWorkflow);
                    if (previousWorkflow != null && previousWorkflow.CurrentNode != null && value == null)
                    {
                        this.CurrentNodeChanged(this, new NodeChangedEventArgs(null));
                    }
                }
            }
        }

        public System.Windows.Input.ICommand ExpandAllCommand { get; private set; }

        public ObservableCollection<GuidanceWorkflowViewModel> Workflows { get; private set; }

        public System.Windows.Input.ICommand GoToFocusedCommand { get; private set; }

        public System.Windows.Input.ICommand RefreshGraphCommand { get; private set; }

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

        private void SetWorkflows(IEnumerable<IGuidanceExtension> extensions)
        {
            // Match the untouched ViewModels
            var inmutedWorkflows = this.Workflows.Where(w => extensions.Any(f => f.GuidanceWorkflow == w.Model));

            // Remove all extension removed
            foreach (var workflow in this.Workflows.Except(inmutedWorkflows).ToArray())
            {
                workflow.CurrentNodeChanged -= this.OnCurrentNodeChanged;
                this.Workflows.Remove(workflow);
            }

            // Add all extensions added
            var addedExtensions = extensions.Where(f => !this.Workflows.Any(w => w.Model == f.GuidanceWorkflow));
            foreach (var extension in addedExtensions)
            {
                if (extension.GuidanceWorkflow != null)
                {
                    var workflow = new GuidanceWorkflowViewModel(new GuidanceWorkflowContext
                        {
                            GuidanceManager = this.context.GuidanceManager,
                            Extension = extension,
                            UserMessageService = this.context.UserMessageService,
                        }, this.serviceProvider);
                    this.Workflows.Add(workflow);
                    workflow.CurrentNodeChanged += this.OnCurrentNodeChanged;
                }
            }

            if (this.CurrentWorkflow != null && !extensions.Any(f => f.GuidanceWorkflow == this.CurrentWorkflow.Model))
            {
                this.CurrentWorkflow = null;
            }
        }

        private void SubscribeToExtensionChanges(IGuidanceManager guidanceManager)
        {
            guidanceManager.InstantiatedExtensionsChanged += (s, e) => this.SetWorkflows(guidanceManager.InstantiatedGuidanceExtensions);
            this.SetWorkflows(guidanceManager.InstantiatedGuidanceExtensions);

            guidanceManager.ActiveExtensionChanged += (s, e) =>
            {
                if (guidanceManager.ActiveGuidanceExtension == null)
                    this.CurrentWorkflow = null;
                else
                {
                    if (guidanceManager.ActiveGuidanceExtension.GuidanceWorkflow != null)
                    {
                        this.CurrentWorkflow = this.Workflows.FirstOrDefault(w => w.Model == guidanceManager.ActiveGuidanceExtension.GuidanceWorkflow);
                        GuidanceConditionsEvaluator.EvaluateGraph(guidanceManager.ActiveGuidanceExtension);
                    }
                }
            };
        }
    }
}
