using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Reflection;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Represents the guidance workflow.
    /// </summary>
    [DisplayNameResource(@"GuidanceNode_GuidanceWorkflow_DisplayName", typeof(Resources))]
    public class GuidanceWorkflow : ConditionalNode, IGuidanceWorkflow
    {
        private string focusedActionName;
        private List<IConditionalNode> allNodesToEvaluate = null;

        /// <summary>
        /// Creates a new instance of the <see cref="GuidanceWorkflow"/> class.
        /// </summary>
        public GuidanceWorkflow()
            : base(WorkflowStateComposer.Instance, Cardinality.None, Cardinality.One)
        {
        }

        internal override NodeType Type
        {
            get { return NodeType.Workflow; }
        }

        /// <summary>
        /// Gets the identifier of the owning feature
        /// </summary>
        [Browsable(false)]
        public virtual string OwnerId { get; set; }

        /// <summary>
        /// Gets the Owning feature
        /// </summary>
        [Browsable(false)]
        public virtual IGuidanceExtension OwningExtension { get; set; }

        /// <summary>
        /// Gets all the nodes to evaluate in the workflow
        /// </summary>
        [Browsable(false)]
        public virtual IList<IConditionalNode> AllNodesToEvaluate
        {
            get
            {
                if (allNodesToEvaluate == null)
                {
                    allNodesToEvaluate = new List<IConditionalNode>();

                    IList<IConditionalNode> successors =
                        this.Successors.Traverse(vm => vm.Successors).OfType<IConditionalNode>().ToList();
                    Hashtable h = new Hashtable();

                    foreach (var node in successors)
                    {
                        if (!h.Contains(node))
                        {
                            allNodesToEvaluate.Add(node);
                            h.Add(node, "");
                        }
                    }
                }

                return allNodesToEvaluate;
            }
        }

        /// <summary>
        /// Initializes the Guidance Workflow after satifying imports.
        /// </summary>
        public void Initialize()
        {
            this.OnInitialize();

            var final = this.Successors.Traverse<INode>(s => s.Successors).OfType<IFinal>().FirstOrDefault();
            if (final != null)
            {
                final.StateChanged += (s, e) => this.ComposeState(StateChangeSource.Successor);
            }
        }

        /// <summary>
        /// Deinitializes hte guidance workflow
        /// </summary>
        public void Deinitialize()
        {
            this.OnDeinitialize();
        }

        /// <summary>
        /// If an author overrides this and returns true then
        /// by ignoring all Post Conditions, nodes in the workflow will still
        /// not enable until the Pre Conditions are true but then will stay "Enabled"
        /// forever (i.e. never move to Completed status)
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        public virtual bool IgnorePostConditions
        {
            get { return false; }
        }

        /// <summary>
        /// Occurs when the focused activity changed.
        /// </summary>
        public event EventHandler FocusedActionChanged = (s, e) => { };

        /// <summary>
        /// Gets the entry point to the guidance workflow.
        /// </summary>
        /// <value>The entry point to the guidance workflow.</value>
        [Browsable(false)]
        public IInitial InitialNode
        {
            get { return (IInitial)this.Successors.FirstOrDefault(); }
        }

        /// <summary>
        /// Gets the focused activity.
        /// </summary>
        /// <value>The focused activity.</value>
        [Browsable(false)]
        public IGuidanceAction FocusedAction
        {
            get
            {
                return string.IsNullOrEmpty(focusedActionName) ?
                    null :
                    this.Successors.Traverse(n => n.Successors)
                        .OfType<IGuidanceAction>()
                        .FirstOrDefault(n => n.Name == focusedActionName);
            }
        }

        /// <summary>
        /// Clears the focused activity if there is one.
        /// </summary>
        public virtual void ClearFocused()
        {
            var previousFocusedAction = focusedActionName;
            focusedActionName = null;

            if (previousFocusedAction != null)
                this.OnFocusedActionChanged();
        }

        /// <summary>
        /// Connects this workflow with the <see cref="Initial"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Initial"/> to connect as successor of this activity.</param>
        public virtual void ConnectTo(Initial node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Focuses the specified activity.
        /// </summary>
        /// <param name="action">The action to be focused.</param>
        public virtual void Focus(IGuidanceAction action)
        {
            Guard.NotNull(() => action, action);

            var previousFocusedAction = this.FocusedAction;
            focusedActionName = action.Name;

            if (previousFocusedAction != action)
                this.OnFocusedActionChanged();
        }

        /// <summary>
        /// Called to initialize the workflow, by default setting its 
        /// state to <see cref="NodeState.Enabled"/>.
        /// </summary>
        protected virtual void OnInitialize()
        {
            this.State = NodeState.Enabled;
        }

        /// <summary>
        /// Called to de-initialize the workflow
        /// </summary>
        protected virtual void OnDeinitialize()
        {

        }

        /// <summary>
        /// Called when the focused activity changed
        /// </summary>
        protected void OnFocusedActionChanged()
        {
            this.OnPropertyChanged(Reflect<GuidanceWorkflow>.GetProperty(g => g.FocusedAction).Name);
            this.FocusedActionChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Subscribe to state changes of this node.
        /// </summary>
        protected override void SubscribeStateChanged(Node successor)
        {
            // No need to subscribe to state changes of the initial node
        }

        private class WorkflowStateComposer : IStateComposer
        {
            internal static readonly IStateComposer Instance = new WorkflowStateComposer();

            private WorkflowStateComposer()
            {
            }

            public NodeState ComposeState(INode node, StateChangeSource changeSource)
            {
                var final = node.Successors.Traverse<INode>(s => s.Successors).OfType<IFinal>().FirstOrDefault();
                if (final == null)
                    return node.State;

                return final.State == NodeState.Completed ? NodeState.Completed : NodeState.Enabled;
            }
        }
    }
}