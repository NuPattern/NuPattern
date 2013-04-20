using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Defines a guidance predecessor that allows split the processing in multiple threads.
    /// </summary>
    [DisplayNameResource("GuidanceNode_Fork_DisplayName", typeof(Resources))]
    public class Fork : ConditionalNode, IFork
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Fork"/> class.
        /// </summary>
        public Fork()
            : this(DefaultStateComposer.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fork"/> class.
        /// </summary>
        /// <param name="stateComposer">The state composer to compose the node state.</param>
        protected Fork(IStateComposer stateComposer)
            : base(stateComposer, NodeDescriptor.Fork.FanIn, NodeDescriptor.Fork.FanOut)
        {
            this.LaunchPoints = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets or sets the launch points associated with the activity
        /// </summary>
        [Browsable(false)]
        public IEnumerable<string> LaunchPoints { get; set; }

        /// <summary>
        /// Connects this fork predecessor with the activities received as parameter.
        /// </summary>
        /// <param name="nodes">The guidance actions to connect as successors of this fork predecessor.</param>
        public virtual void ConnectTo(params GuidanceAction[] nodes)
        {
            Guard.NotNull(() => nodes, nodes);

            this.ConnectTo((Node[])nodes);
        }

        /// <summary>
        /// Connects this fork predecessor with the forks received as parameter.
        /// </summary>
        /// <param name="nodes">The forks to connect as successors of this fork predecessor.</param>
        public virtual void ConnectTo(params Fork[] nodes)
        {
            Guard.NotNull(() => nodes, nodes);

            this.ConnectTo((Node[])nodes);
        }

        /// <summary>
        /// Connects this fork predecessor with the decisions received as parameter.
        /// </summary>
        /// <param name="nodes">The decisions to connect as successors of this fork predecessor.</param>
        public virtual void ConnectTo(params Decision[] nodes)
        {
            Guard.NotNull(() => nodes, nodes);

            this.ConnectTo((Node[])nodes);
        }

        /// <summary>
        /// Connects this fork predecessor with the merges received as parameter.
        /// </summary>
        /// <param name="nodes">The merges to connect as successors of this fork predecessor.</param>
        public virtual void ConnectTo(params Merge[] nodes)
        {
            Guard.NotNull(() => nodes, nodes);

            this.ConnectTo((Node[])nodes);
        }

    }
}