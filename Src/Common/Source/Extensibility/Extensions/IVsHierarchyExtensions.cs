using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Helper methods for <see cref="IVsHierarchy"/>.
	/// </summary>
    [CLSCompliant(false)]
    public static class IVsHierarchyExtensions
	{
		/// <summary>
		/// Gets the item id of the given hierarchy object.
		/// </summary>
		public static uint TryGetItemId(this IVsHierarchy hierarchy)
		{
            Guard.NotNull(() => hierarchy, hierarchy);
            
            object extObject;
			var itemId = uint.MaxValue;
			IVsHierarchy tempHierarchy;

			var succeeded = ErrorHandler.Succeeded(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_BrowseObject, out extObject));

			if (succeeded)
			{
				var browseObject = extObject as IVsBrowseObject;
				if (browseObject != null)
				{
					succeeded = ErrorHandler.Succeeded(browseObject.GetProjectItem(out tempHierarchy, out itemId));
					if (succeeded)
					{
						return itemId;
					}
				}
			}

			return itemId;
		}
	}
}
