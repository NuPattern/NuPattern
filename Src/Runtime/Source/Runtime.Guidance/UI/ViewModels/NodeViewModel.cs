using System.Windows.Input;
using System.Windows.Media;
using NuPattern.Presentation;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    internal class NodeViewModel : TreeNodeViewModel<INode>
    {
        public NodeViewModel(INode node, ImageSource icon, string name = null)
            : base(node)
        {
            Guard.NotNull(() => node, node);
            Guard.NotNull(() => icon, icon);

            this.Icon = icon;
            this.Name = name ?? node.Name;

            this.Model.StateChanged += (s, e) => this.OnPropertyChanged(() => this.State);
        }

        public string Description
        {
            get { return this.Model.Description; }
        }

        public ImageSource Icon { get; private set; }

        public bool IsConditionalNode
        {
            get { return this.Model is IConditionalNode; }
        }

        public string Name { get; private set; }

        public INode Node
        {
            get { return this.Model; }
        }

        public ICommand RefreshDetailsCommand { get; protected set; }

        public virtual NodeState State
        {
            get { return this.Model.State; }
        }

        public string ToolTip
        {
            get { return string.IsNullOrEmpty(this.Description) ? this.Name : this.Description; }
        }

        protected override void RegisterCommands()
        {
            // To avoid WPF binding errors
            this.RefreshDetailsCommand = new RelayCommand(() => { }, () => false);
        }
    }
}