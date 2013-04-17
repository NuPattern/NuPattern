using System.Collections.Generic;
using System.Linq;
using NuPattern;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Defines a guidance predecessor that allows join the processing of multiple activities in a threads.
    /// </summary>
    public class Join : ConditionalNode, IJoin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Join"/> class.
        /// </summary>
        public Join()
            : this(DefaultStateComposer.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Join"/> class.
        /// </summary>
        /// <param name="stateComposer">The state composer to compose the node state.</param>
        protected Join(IStateComposer stateComposer)
            : base(stateComposer, NodeDescriptor.Join.FanIn, NodeDescriptor.Join.FanOut)
        {
            this.LaunchPoints = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets or sets the launch points associated with the activity
        /// </summary>
        public IEnumerable<string> LaunchPoints { get; set; }

        /// <summary>
        /// Connects this join predecessor with the <see cref="GuidanceAction"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="GuidanceAction"/> to connect as successor of this join predecessor.</param>
        public virtual void ConnectTo(GuidanceAction node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this join predecessor with the <see cref="Final"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Final"/> to connect as successor of this join predecessor.</param>
        public virtual void ConnectTo(Final node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this join predecessor with the <see cref="Decision"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Decision"/> to connect as successor of this join predecessor.</param>
        public virtual void ConnectTo(Decision node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this join predecessor with the <see cref="Merge"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Merge"/> to connect as successor of this join predecessor.</param>
        public virtual void ConnectTo(Merge node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this join predecessor with the <see cref="Fork"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Fork"/> to connect as successor of this join predecessor.</param>
        public virtual void ConnectTo(Fork node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }

        /// <summary>
        /// Connects this join predecessor with the <see cref="Join"/> received as parameter.
        /// </summary>
        /// <param name="node">The <see cref="Join"/> to connect as successor of this join predecessor.</param>
        public virtual void ConnectTo(Join node)
        {
            Guard.NotNull(() => node, node);

            this.ConnectTo(new Node[] { node });
        }
    }
}