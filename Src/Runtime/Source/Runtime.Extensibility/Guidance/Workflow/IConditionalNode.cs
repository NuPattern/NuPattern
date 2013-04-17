using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Defines a conditional node in the guidance workflow
    /// </summary>
    public interface IConditionalNode : INode
    {
        /// <summary>
        /// Handles the override of the condition
        /// </summary>
        event EventHandler HasStateOverrideChanged;

        /// <summary>
        /// Handles when the user acceptance has changed
        /// </summary>
        event EventHandler IsUserAcceptedChanged;

        /// <summary>
        /// Gets a value to indicate whether this node can be overwridden.
        /// </summary>
        bool HasStateOverride { get; }

        /// <summary>
        /// Gets a value to indicate whether the user has accepted this node.
        /// </summary>
        bool IsUserAccepted { get; set; }

        /// <summary>
        /// Gets the preconditions of the node
        /// </summary>
        IEnumerable<IBinding<ICondition>> Preconditions { get; }

        /// <summary>
        /// Gets the post6 conditions of the node.
        /// </summary>
        IEnumerable<IBinding<ICondition>> Postconditions { get; }

        /// <summary>
        /// Clears the override state
        /// </summary>
        void ClearStateOverride();

        /// <summary>
        /// Sets the state of the node.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="isOverride"></param>
        void SetState(NodeState state, bool isOverride);
    }
}