using System;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance.UI
{
    internal class NodeChangedEventArgs : EventArgs
    {
        public NodeChangedEventArgs(INode node)
        {
            this.CurrentNode = node;
        }

        public INode CurrentNode { get; private set; }
    }
}