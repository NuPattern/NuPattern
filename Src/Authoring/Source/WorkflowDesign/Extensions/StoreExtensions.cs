using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign
{
	/// <summary>
	/// Store extension methods.
	/// </summary>
	internal static class StoreExtensions
	{
		/// <summary>
		/// Gets the root element.
		/// </summary>
		/// <param name="store">The store.</param>
		/// <returns>The root element.</returns>
		internal static Design GetRootElement(this Store store)
		{
			return store.DefaultPartition.ElementDirectory.AllElements.OfType<Design>().SingleOrDefault();
		}

		/// <summary>
		/// Gets the diagrams.
		/// </summary>
		/// <param name="store">The store.</param>
		/// <returns>The diagram list.</returns>
		internal static IEnumerable<WorkflowDesignDiagram> GetDiagrams(this Store store)
		{
			return store.DefaultPartition.ElementDirectory.FindElements<WorkflowDesignDiagram>();
		}

		/// <summary>
		/// Gets the diagram partition.
		/// </summary>
		/// <param name="store">The store.</param>
		internal static Partition GetDefaultDiagramPartition(this Store store)
		{
			return store.DefaultPartition;
		}
	}
}