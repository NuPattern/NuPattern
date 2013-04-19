using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NuPattern.Presentation;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    internal class GuidanceWorkflowViewModel : ViewModel
    {
        private GuidanceWorkflowContext context;
        private IServiceProvider serviceProvider;
        public static GuidanceWorkflowViewModel Current;
        private IEnumerable<TreeNodeViewModel<INode>> nodes;
        private NodeViewModel currentNode;
        private bool hasCurrentNode;

        public GuidanceWorkflowViewModel(GuidanceWorkflowContext context, IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.context = context;
            this.serviceProvider = serviceProvider;
            this.Extension = context.Extension;
            Current = this;
        }
        public event EventHandler CurrentNodeChanged = (s, e) => { };

        public IGuidanceExtension Extension { get; private set; }

        public NodeViewModel CurrentNode
        {
            get
            {
                return this.currentNode;
            }
            set
            {
                if (this.currentNode != value)
                {
                    this.currentNode = value;
                    this.HasCurrentNode = value != null;
                    this.OnCurrentNodeChanged();
                }
            }
        }

        public bool HasCurrentNode
        {
            get
            {
                return this.hasCurrentNode;
            }
            private set
            {
                if (this.hasCurrentNode != value)
                {
                    this.hasCurrentNode = value;
                    this.OnPropertyChanged(() => this.HasCurrentNode);
                }
            }
        }

        public IGuidanceWorkflow Model
        {
            get { return this.Extension.GuidanceWorkflow; }
        }

        public string Name
        {
            get { return this.Extension.InstanceName; }
        }

        public IEnumerable<TreeNodeViewModel<INode>> Nodes
        {
            get
            {
                if (this.nodes == null)
                {
                    this.LoadGuidanceTree();
                }

                return this.nodes;
            }
        }

        public void GoToFocusedAction()
        {
            var focusedAction = this.Model.FocusedAction;
            if (focusedAction != null)
            {
                var node = this.Nodes
                    .Traverse(n => n.Nodes)
                    .First(a => a.Model == focusedAction);
                node.IsSelected = true;

                node = node.ParentNode;
                while (node != null)
                {
                    node.IsExpanded = true;
                    node = node.ParentNode;
                }
            }
        }

        public void RefreshGraphStates()
        {
            GuidanceConditionsEvaluator.EvaluateGraph(this.Extension);
        }

        private void LoadGuidanceTree()
        {
            var viewModelBuilder = this.Extension as IWorkflowViewModelBuilder ?? new DefaultWorkflowViewModelBuilder(this.Extension);
            this.nodes = new ObservableCollection<NodeViewModel>(viewModelBuilder.GetNodes());

            foreach (var node in this.nodes.Traverse(node => node.Nodes))
            {
                node.SetSelected = currentNode => this.CurrentNode = (NodeViewModel)currentNode;
            }

            this.GoToFocusedAction();
        }

        private void OnCurrentNodeChanged()
        {
            if (this.CurrentNode.Node is IGuidanceAction)
                Model.Focus(this.CurrentNode.Node as IGuidanceAction);
            this.OnPropertyChanged(() => this.CurrentNode);
            this.CurrentNodeChanged(this, EventArgs.Empty);
        }
    }
}