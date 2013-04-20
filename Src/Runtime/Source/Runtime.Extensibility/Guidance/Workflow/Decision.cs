using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Defines a guidance predecessor that allows choose between two activities in the workflow.
    /// </summary>
    [DisplayNameResource("GuidanceNode_Decision_DisplayName", typeof(Resources))]
    public class Decision : ConditionalNode, IDecision
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Decision"/> class.
        /// </summary>
        public Decision()
            : this(DefaultStateComposer.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Decision"/> class.
        /// </summary>
        /// <param name="stateComposer">The state composer to compose the node state.</param>
        protected Decision(IStateComposer stateComposer)
            : base(stateComposer, NodeDescriptor.Decision.FanIn, NodeDescriptor.Decision.FanOut)
        {
            this.LaunchPoints = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets or sets the launch points associated with the activity
        /// </summary>
        [Browsable(false)]
        public IEnumerable<string> LaunchPoints { get; set; }

        /// <summary>
        /// Connects this decision predecessor with the activities received as parameter.
        /// </summary>
        /// <param name="activities">The activities to connect as successors of this predecessor.</param>
        public virtual void ConnectTo(params GuidanceAction[] activities)
        {
            Guard.NotNull(() => activities, activities);

            this.ConnectTo(activities.Cast<Node>().ToArray());
        }

        /// <summary>
        /// Connects this decision predecessor with the forks received as parameter.
        /// </summary>
        /// <param name="nodes">The forks to connect as successors of this decision predecessor.</param>
        public virtual void ConnectTo(params Fork[] nodes)
        {
            Guard.NotNull(() => nodes, nodes);

            this.ConnectTo((Node[])nodes);
        }

        /// <summary>
        /// Connects this decision predecessor with the decisions received as parameter.
        /// </summary>
        /// <param name="nodes">The decisions to connect as successors of this decisions predecessor.</param>
        public virtual void ConnectTo(params Decision[] nodes)
        {
            Guard.NotNull(() => nodes, nodes);

            this.ConnectTo((Node[])nodes);
        }

    }
}