using System.Collections.Generic;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Represents a list of <see cref="IToolkitInfo"/> instances.
	/// </summary>
	//// TODO remove this as is only for serialization support
	public class ToolkitInfoCollection : List<IToolkitInfo>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ToolkitInfoCollection"/> class.
		/// </summary>
		public ToolkitInfoCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ToolkitInfoCollection"/> class from the given items.
		/// </summary>
		/// <param name="items">The items.</param>
		public ToolkitInfoCollection(IEnumerable<IToolkitInfo> items)
			: base(items)
		{
		}
	}
}