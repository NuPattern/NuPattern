using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Defines a node in a guidance workflow
    /// </summary>
    public interface INode : INotifyPropertyChanged //, IFluentInterface
    {
        /// <summary>
        /// Handles a change in state of the node.
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// Gets the name of the node
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets hte parent of the node.
        /// </summary>
        object ParentObject { get; set; }

        /// <summary>
        /// Gets the description of the node.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the link to optional guidance associated with the element.
        /// </summary>
        string Link { get; }

        /// <summary>
        /// Gets the nodes predeccessors.
        /// </summary>
        IEnumerable<INode> Predecessors { get; }

        /// <summary>
        /// Gets the state of the node.
        /// </summary>
        NodeState State { get; }

        /// <summary>
        /// Gets the nodes successors.
        /// </summary>
        IEnumerable<INode> Successors { get; }
    }
}