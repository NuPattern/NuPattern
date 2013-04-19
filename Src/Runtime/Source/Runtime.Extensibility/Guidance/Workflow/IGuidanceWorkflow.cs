using System;
using System.Collections.Generic;

namespace NuPattern.Runtime.Guidance.Workflow
{
    /// <summary>
    /// Defines a process guidance workflow.
    /// </summary>
    public interface IGuidanceWorkflow : IConditionalNode
    {
        /// <summary>
        /// Occurs when the focused activity changed.
        /// </summary>
        event EventHandler FocusedActionChanged;

        /// <summary>
        /// Clears the focus of the workflow
        /// </summary>
        void ClearFocused();

        /// <summary>
        /// Brings this workflow into focus
        /// </summary>
        /// <param name="activity"></param>
        void Focus(IGuidanceAction activity);

        /// <summary>
        /// Gets the focused action.
        /// </summary>
        IGuidanceAction FocusedAction { get; }

        /// <summary>
        /// Gets the initial node.
        /// </summary>
        IInitial InitialNode { get; }

        /// <summary>
        /// Gets all the nodes to evaluate in the workflow.
        /// </summary>
        IList<IConditionalNode> AllNodesToEvaluate { get; }

        /// <summary>
        /// Gets the identifier of the owning extension
        /// </summary>
        string OwnerId { get; set; }

        /// <summary>
        /// Gets the owning guidance extension.
        /// </summary>
        IGuidanceExtension OwningExtension { get; set; }
    }
}