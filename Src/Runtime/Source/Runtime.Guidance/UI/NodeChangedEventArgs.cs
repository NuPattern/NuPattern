using System;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
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