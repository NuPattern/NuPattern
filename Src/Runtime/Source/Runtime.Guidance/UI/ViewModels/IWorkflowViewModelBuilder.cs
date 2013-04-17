using System.Collections.Generic;

namespace NuPattern.Runtime.Guidance.UI.ViewModels
{
    /// <summary>
    /// Allows implement a custom view of the nodes for the Guidance Explorer tool window. If the <see cref="IFeatureExtension"/>
    /// implements this interface that feature can override the default way to show the nodes in the tool window.
    /// </summary>
    internal interface IWorkflowViewModelBuilder
    {
        /// <summary>
        /// Gets the node hierarchy to show in the Guidance Explorer tool window.
        /// </summary>
        /// <returns>The collection with the root nodes of the hierarchy.</returns>
        IEnumerable<NodeViewModel> GetNodes();
    }
}