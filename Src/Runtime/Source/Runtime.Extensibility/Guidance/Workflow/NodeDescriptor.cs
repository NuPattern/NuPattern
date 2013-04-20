
namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Describes the nodes in a guidance workflow.
    /// </summary>
    internal class NodeDescriptor
    {
        public static readonly NodeDescriptor Initial = new NodeDescriptor
        {
            Type = NodeType.Initial,
            FanIn = Cardinality.None,
            FanOut = Cardinality.One,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IDecisionNode) }
        };

        public static readonly NodeDescriptor Final = new NodeDescriptor
        {
            Type = NodeType.Final,
            FanIn = Cardinality.One,
            //ValidIncomings = new[] { typeof(IOpaqueAction), typeof(IJoinNode), typeof(IMergeNode) },
            FanOut = Cardinality.None
        };

        public static readonly NodeDescriptor Action = new NodeDescriptor
        {
            Type = NodeType.Action,
            FanIn = Cardinality.One,
            //ValidIncomings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IJoinNode), typeof(IDecisionNode), typeof(IMergeNode), typeof(IInitialNode) },
            FanOut = Cardinality.One,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IJoinNode), typeof(IDecisionNode), typeof(IMergeNode), typeof(IFinalNode) },
        };

        public static readonly NodeDescriptor Decision = new NodeDescriptor
        {
            Type = NodeType.Decision,
            FanIn = Cardinality.One,
            //ValidIncomings = new[] { typeof(IJoinNode), typeof(IMergeNode), typeof(IOpaqueAction), typeof(IInitialNode), typeof(IForkNode), typeof(IDecisionNode) },
            FanOut = Cardinality.OneOrMore,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IDecisionNode) }
        };

        public static readonly NodeDescriptor Merge = new NodeDescriptor
        {
            Type = NodeType.Merge,
            FanIn = Cardinality.OneOrMore,
            //ValidIncomings = new[] { typeof(IOpaqueAction), typeof(IJoinNode), typeof(IMergeNode) },
            FanOut = Cardinality.One,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IJoinNode), typeof(IDecisionNode), typeof(IMergeNode), typeof(IFinalNode) }
        };

        public static readonly NodeDescriptor Fork = new NodeDescriptor
        {
            Type = NodeType.Fork,
            FanIn = Cardinality.One,
            //ValidIncomings = new[] { typeof(IJoinNode), typeof(IMergeNode), typeof(IOpaqueAction), typeof(IInitialNode), typeof(IForkNode), typeof(IDecisionNode) },
            FanOut = Cardinality.OneOrMore,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IDecisionNode) }
        };

        public static readonly NodeDescriptor Join = new NodeDescriptor
        {
            Type = NodeType.Join,
            FanIn = Cardinality.OneOrMore,
            //ValidIncomings = new[] { typeof(IOpaqueAction), typeof(IJoinNode), typeof(IMergeNode) },
            FanOut = Cardinality.One,
            //ValidOutgoings = new[] { typeof(IOpaqueAction), typeof(IForkNode), typeof(IJoinNode), typeof(IDecisionNode), typeof(IMergeNode), typeof(IFinalNode) }
        };

        private NodeDescriptor()
        {
        }

        public Cardinality FanIn { get; private set; }
        public Cardinality FanOut { get; private set; }
        public NodeType Type { get; private set; }
        //public Type[] ValidIncomings { get; private set; }
        //public Type[] ValidOutgoings { get; private set; }
    }
}