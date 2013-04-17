using System.Linq;
using NuPattern;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Defines the entry point element for a guidance workflow.
    /// </summary>
    public class Initial : Node, IInitial
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Initial"/> class.
        /// </summary>
        public Initial()
            : this(FinishedStateComposer.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Initial"/> class.
        /// </summary>
        /// <param name="stateComposer">The state composer to compose the node state.</param>
        protected Initial(IStateComposer stateComposer)
            : base(stateComposer, Cardinality.One, NodeDescriptor.Initial.FanOut)
        {
        }

        /// <summary>
        /// Gets the initial activity contected to this initial node.
        /// </summary>
        public IGuidanceAction InitialActivity
        {
            get { return this.Successors.OfType<IGuidanceAction>().FirstOrDefault(); }
        }

        /// <summary>
        /// Connects this initial predecessor with the <see cref="GuidanceAction"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="GuidanceAction"/> to connect as successor of this initial predecessor.</param>
        public virtual void ConnectTo(GuidanceAction node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this initial predecessor with the <see cref="Fork"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Fork"/> to connect as successor of this initial predecessor.</param>
        public virtual void ConnectTo(Fork node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this initial predecessor with the <see cref="Decision"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Decision"/> to connect as successor of this initial predecessor.</param>
        public virtual void ConnectTo(Decision node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        private class FinishedStateComposer : IStateComposer
        {
            public static readonly IStateComposer Instance = new FinishedStateComposer();

            public NodeState ComposeState(INode node, StateChangeSource changeSource)
            {
                return node.Successors.Any() && node.Predecessors.Any() ? NodeState.Completed : NodeState.Unknown;
            }
        }
    }
}