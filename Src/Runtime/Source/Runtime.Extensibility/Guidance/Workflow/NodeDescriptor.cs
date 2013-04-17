
namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Describes the nodes in a guidance workflow.
    /// </summary>
    internal class NodeDescriptor
    {
        public static readonly NodeDescriptor Initial = new NodeDescriptor
        {
            //Type = typeof(IInitialNode),
            FanIn = Cardinality.None,
            FanOut = Cardinality.One,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IDecisionNode) }
        };

        public static readonly NodeDescriptor Final = new NodeDescriptor
        {
            //Type = typeof(IFinalNode),
            FanIn = Cardinality.One,
            //ValidIncomings = new[] { typeof(IOpaqueAction), typeof(IJoinNode), typeof(IMergeNode) },
            FanOut = Cardinality.None
        };

        public static readonly NodeDescriptor Action = new NodeDescriptor
        {
            //Type = typeof(IOpaqueAction),
            FanIn = Cardinality.One,
            //ValidIncomings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IJoinNode), typeof(IDecisionNode), typeof(IMergeNode), typeof(IInitialNode) },
            FanOut = Cardinality.One,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IJoinNode), typeof(IDecisionNode), typeof(IMergeNode), typeof(IFinalNode) },
        };

        public static readonly NodeDescriptor Decision = new NodeDescriptor
        {
            //Type = typeof(IDecisionNode),
            FanIn = Cardinality.One,
            //ValidIncomings = new[] { typeof(IJoinNode), typeof(IMergeNode), typeof(IOpaqueAction), typeof(IInitialNode), typeof(IForkNode), typeof(IDecisionNode) },
            FanOut = Cardinality.OneOrMore,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IDecisionNode) }
        };

        public static readonly NodeDescriptor Merge = new NodeDescriptor
        {
            //Type = typeof(IMergeNode),
            FanIn = Cardinality.OneOrMore,
            //ValidIncomings = new[] { typeof(IOpaqueAction), typeof(IJoinNode), typeof(IMergeNode) },
            FanOut = Cardinality.One,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IJoinNode), typeof(IDecisionNode), typeof(IMergeNode), typeof(IFinalNode) }
        };

        public static readonly NodeDescriptor Fork = new NodeDescriptor
        {
            //Type = typeof(IForkNode),
            FanIn = Cardinality.One,
            //ValidIncomings = new[] { typeof(IJoinNode), typeof(IMergeNode), typeof(IOpaqueAction), typeof(IInitialNode), typeof(IForkNode), typeof(IDecisionNode) },
            FanOut = Cardinality.OneOrMore,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IDecisionNode) }
        };

        public static readonly NodeDescriptor Join = new NodeDescriptor
        {
            //Type = typeof(IJoinNode),
            FanIn = Cardinality.OneOrMore,
            //ValidIncomings = new[] { typeof(IOpaqueAction), typeof(IJoinNode), typeof(IMergeNode) },
            FanOut = Cardinality.One,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IJoinNode), typeof(IDecisionNode), typeof(IMergeNode), typeof(IFinalNode) }
        };

        private NodeDescriptor()
        {
        }

        //public Type Type { get; private set; }
        public Cardinality FanIn { get; private set; }
        public Cardinality FanOut { get; private set; }
        //public Type[] ValidIncomings { get; private set; }
        //public Type[] ValidOutgoings { get; private set; }
    }
}