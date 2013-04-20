using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Defines the exit point element for a process workflow.
    /// </summary>
    [DisplayNameResource("GuidanceNode_Final_DisplayName", typeof(Resources))]
    public class Final : Node, IFinal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Final"/> class.
        /// </summary>
        public Final()
            : this(DefaultStateComposer.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Final"/> class.
        /// </summary>
        /// <param name="stateComposer">The state composer to compose the node state.</param>
        protected Final(IStateComposer stateComposer)
            : base(stateComposer, NodeDescriptor.Final.FanIn, NodeDescriptor.Final.FanOut)
        {
        }
    }
}