using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Extension methods for AbstractComponent.
    /// </summary>
    internal static class AbstractComponentExtensions
    {
        /// <summary>
        /// Gets the view for abstract component.
        /// </summary>
        /// <param name="abstractElement">The component.</param>
        internal static ViewSchema GetViewForAbstractElement(this AbstractElementSchema abstractElement)
        {
            return abstractElement.Traverse(c1 => c1.Owner, c1 => c1.View != null).View;
        }
    }
}