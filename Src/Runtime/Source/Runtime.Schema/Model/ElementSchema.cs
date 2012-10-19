using Microsoft.VisualStudio.Modeling;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// DomainClass element.
	/// </summary>
	public partial class ElementSchema
	{
		/// <summary>
		/// Called by the Merge process to create a relationship between
		/// this target element and the specified source element.
		/// Typically, a parent-child relationship is established
		/// between the target element (the parent) and the source element
		/// (the child), but any relationship can be established.
		/// </summary>
		/// <param name="sourceElement">The element that is to be related to this model element.</param>
		/// <param name="elementGroup">The group of source ModelElements that have been rehydrated into the target state.</param>
		/// <remarks>
		/// This method is overriden to create the relationship between the target element and the specified source element.
		/// The base method does nothing.
		/// </remarks>
		protected override void MergeRelate(ModelElement sourceElement, ElementGroup elementGroup)
		{
			////TODO: \o/ Remove this once DSL team fixes merging of MEXes

			if (sourceElement is ExtensionElement)
			{
				return;
			}

			base.MergeRelate(sourceElement, elementGroup);
		}
	}
}