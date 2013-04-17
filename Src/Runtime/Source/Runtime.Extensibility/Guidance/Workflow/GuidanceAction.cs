using System.Collections.Generic;
using System.Linq;
using NuPattern;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Represents an activity in the guidance workflow.
    /// </summary>
    public class GuidanceAction : ConditionalNode, IGuidanceAction
    {
        //private static readonly ITraceSource tracer = Tracer.GetSourceFor<Node>();

        private bool hasBlockingPredecessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidanceAction"/> class.
        /// </summary>
        public GuidanceAction()
            : this(GuidanceActionStateComposer.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidanceAction"/> class.
        /// </summary>
        /// <param name="name">Name of the activity.</param>
        public GuidanceAction(string name)
            : this(GuidanceActionStateComposer.Instance)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidanceAction"/> class.
        /// </summary>
        /// <param name="stateComposer">The state composer.</param>
        protected GuidanceAction(IStateComposer stateComposer)
            : base(stateComposer, NodeDescriptor.Action.FanIn, NodeDescriptor.Action.FanOut)
        {
            this.LaunchPoints = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets or sets the launch points associated with the activity
        /// </summary>
        public IEnumerable<string> LaunchPoints { get; set; }

        /// <summary>
        /// Gets the state of the activity.
        /// </summary>
        /// <value>One of the <see cref="NodeState"/> members.</value>
        public override NodeState State
        {
            get { return this.hasBlockingPredecessor ? NodeState.Blocked : base.State; }
        }

        /// <summary>
        /// Connects this activity with the <see cref="GuidanceAction"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="GuidanceAction"/> to connect as successor of this activity.</param>
        public virtual void ConnectTo(GuidanceAction node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this activity with the <see cref="Fork"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Fork"/> to connect as successor of this activity.</param>
        public virtual void ConnectTo(Fork node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this activity with the <see cref="Join"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Join"/> to connect as successor of this activity.</param>
        public virtual void ConnectTo(Join node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this activity with the <see cref="Decision"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Decision"/> to connect as successor of this activity.</param>
        public virtual void ConnectTo(Decision node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this activity with the <see cref="Merge"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Merge"/> to connect as successor of this activity.</param>
        public virtual void ConnectTo(Merge node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this activity with the <see cref="Final"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Final"/> to connect as successor of this activity.</param>
        public virtual void ConnectTo(Final node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Refreshes the state depending on the state of the predecessors and successors.
        /// </summary>
        protected override void ComposeState(StateChangeSource changeSource)
        {
            var state = this.StateComposer.ComposeState(this, changeSource);
            if (this.State != state)
            {
                var previouslyHadPredecessorBlocking = this.hasBlockingPredecessor;
                this.hasBlockingPredecessor = state == NodeState.Blocked;

                if (base.State != NodeState.Blocked && (this.hasBlockingPredecessor || previouslyHadPredecessorBlocking))
                {
                    this.OnStateChanged();
                }
            }
        }

        private class GuidanceActionStateComposer : IStateComposer
        {
            internal static readonly IStateComposer Instance = new GuidanceActionStateComposer();

            private GuidanceActionStateComposer()
            {
            }

            public NodeState ComposeState(INode node, StateChangeSource changeSource)
            {
#if ImplicitPredecessorsCompleteCondition
                if (changeSource == StateChangeSource.Predecessor && node.Predecessors.Any())
                {
                    return node.Predecessors.All(n => n.State == NodeState.Completed) ? NodeState.Unknown : NodeState.Blocked;
                }
#endif
                // Ignores successor changes.
                return node.State;
            }
        }
    }
}