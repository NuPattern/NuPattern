using System;
using System.Collections.ObjectModel;
using NuPattern.Presentation;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal abstract class TreeNodeViewModel<TNode> : ViewModel<TNode>
        where TNode : class
    {
        private bool isExpanded;
        private bool isSelected;
        private TreeNodeViewModel<TNode> parentNode;
        private ObservableCollection<TreeNodeViewModel<TNode>> nodes = new ObservableCollection<TreeNodeViewModel<TNode>>();

        protected TreeNodeViewModel(TNode model)
            : base(model)
        {
            this.Nodes = new ReadOnlyObservableCollection<TreeNodeViewModel<TNode>>(nodes);
        }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
            set
            {
                if (this.isExpanded != value)
                {
                    this.isExpanded = value;
                    this.OnPropertyChanged(() => this.IsExpanded);
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(() => this.IsSelected);
                    if (value && this.SetSelected != null)
                    {
                        this.SetSelected(this);
                    }
                }
            }
        }

        public ReadOnlyObservableCollection<TreeNodeViewModel<TNode>> Nodes { get; private set; }

        public TreeNodeViewModel<TNode> ParentNode
        {
            get
            {
                return this.parentNode;
            }
            internal set
            {
                if (this.parentNode != null)
                {
                    this.parentNode.nodes.Remove(this);
                }

                this.parentNode = value;
                if (value != null)
                {
                    this.parentNode.nodes.Add(this);
                }
            }
        }

        public Action<TreeNodeViewModel<TNode>> SetSelected { get; set; }
    }
}