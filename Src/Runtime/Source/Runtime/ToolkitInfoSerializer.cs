using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Helper class that reads and writes <see cref="IToolkitInfo"/> instances as XML.
	/// </summary>
	//// TODO convert this class to ExtensionInfoSerializer (Use JSON to get the serialized data).
	public static class ToolkitInfoSerializer
	{
		/// <summary>
		/// Serializes the given items and returns them as XML.
		/// </summary>
		public static string ToXml(IEnumerable<IToolkitInfo> items)
		{
			Guard.NotNull(() => items, items);

			return XamlWriter.Save(new ToolkitInfoCollection(
				items.Select(fi => new ToolkitInfo
				{
					Id = fi.Id,
					Name = fi.Name,
					Description = fi.Description,
					DownloadUri = fi.DownloadUri
				})));
		}

		/// <summary>
		/// Deserializes <see cref="IToolkitInfo"/> instances from the given 
		/// XML content.
		/// </summary>
		public static IEnumerable<IToolkitInfo> FromXml(string content)
		{
			Guard.NotNullOrEmpty(() => content, content);

			return (IEnumerable<IToolkitInfo>)XamlReader.Parse(content);
		}
	}
}