using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Wrapper for IItem to be used in the <see cref="T:Microsoft.VisualStudio.Patterning.Runtime.UriProviders.PackUriProvider"/>
	/// </summary>
	[CLSCompliant(false)]
	public class ResourcePack
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourcePack"/> class.
		/// </summary>
		/// <param name="item">The item to wrap</param>
		public ResourcePack(IItem item)
		{
			this.Item = item;
		}

		/// <summary>
		/// Provides read/write access to the wrapped item
		/// </summary>
		public IItem Item { get; set; }
	}
}
