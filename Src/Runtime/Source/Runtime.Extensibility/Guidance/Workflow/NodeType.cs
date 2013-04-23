
namespace NuPattern.Runtime.Guidance.Workflow
{
    internal enum NodeType
    {
        Unknown,
        Workflow,
        Initial,
        Final,
        Action,
        Decision,
        Fork,
        Join,
        Merge,
    }
}
