using System.Collections.Generic;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Defines an action in a guidance workflow.
    /// </summary>
    public interface IGuidanceAction : IConditionalNode
    {
        /// <summary>
        /// Gets the launch point references associated with the activity.
        /// </summary>
        IEnumerable<string> LaunchPoints { get; }
    }
}