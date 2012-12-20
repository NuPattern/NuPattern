using System;

namespace NuPattern.Authoring.WorkflowDesign
{
    /// <summary>
    /// WorkflowDesign Designer.
    /// </summary>
    public partial class WorkflowDesignDomainModel
    {
        /// <summary>
        /// Gets the list of non-generated domain model types.
        /// </summary>
        /// <returns>List of types.</returns>
        protected override Type[] GetCustomDomainModelTypes()
        {
            return new[]
			{ 
                typeof(SuppliedAssetPropertyChangeRule),
                typeof(ProducedAssetPropertyChangeRule),
			};
        }
    }
}
