using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Defines a guidance predecessor that allows merge different branchs that were separated by a <see cref="Decision"/> process element.
    /// </summary>
    [DisplayNameResource("GuidanceNode_Merge_DisplayName", typeof(Resources))]
    public class Merge : ConditionalNode, IMerge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Merge"/> class.
        /// </summary>
        public Merge()
            : this(DefaultStateComposer.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Merge"/> class.
        /// </summary>
        /// <param name="stateComposer">The state composer to compose the node state.</param>
        protected Merge(IStateComposer stateComposer)
            : base(stateComposer, NodeDescriptor.Merge.FanIn, NodeDescriptor.Merge.FanOut)
        {
            this.LaunchPoints = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets or sets the launch points associated with the activity
        /// </summary>
        [Browsable(false)]
        public IEnumerable<string> LaunchPoints { get; set; }

        /// <summary>
        /// Connects this merge predecessor with the <see cref="GuidanceAction"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="GuidanceAction"/> to connect as successor of this merge predecessor.</param>
        public virtual void ConnectTo(GuidanceAction node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this merge predecessor with the <see cref="Final"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Final"/> to connect as successor of this merge predecessor.</param>
        public virtual void ConnectTo(Final node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this merge predecessor with the <see cref="Decision"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Decision"/> to connect as successor of this merge predecessor.</param>
        public virtual void ConnectTo(Decision node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this merge predecessor with the <see cref="Fork"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Fork"/> to connect as successor of this merge predecessor.</param>
        public virtual void ConnectTo(Fork node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this merge predecessor with the <see cref="Join"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Join"/> to connect as successor of this merge predecessor.</param>
        public virtual void ConnectTo(Join node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this merge predecessor with the <see cref="Merge"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Merge"/> to connect as successor of this merge predecessor.</param>
        public virtual void ConnectTo(Merge node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }
    }
}