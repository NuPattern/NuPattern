using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Modeling.Diagrams.ExtensionEnablement;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal abstract partial class AuthoringGestureControllerBase : GestureExtensionController
	{
		private const string MetadataFilterProperty = "MetadataFilter";

		/// <summary>
		/// String-based Metadata Key that determines if a particular Exported Type can be imported or not.
		/// The default CanImport implementation filters imports based on this metadata key.
		/// The default value of this property is null indicating that no filter will be applied.
		/// </summary>
		protected override string MetadataFilter
		{
			get { return ExtensibilityConstants.MetadataFilter; }
		}

		/// <summary>
		/// Allow registrar that have a key in their metadata matching the MetadataFilter string to be imported.
		/// </summary>
		/// <param name="lazyImport">The imports to verify.</param>
		protected override bool CanImport(Lazy<IGestureExtension, Dictionary<string, object>> lazyImport)
		{
			Guard.NotNull(() => lazyImport, lazyImport);

			if (!string.IsNullOrEmpty(this.MetadataFilter))
			{
				return lazyImport.Metadata.ContainsKey(MetadataFilterProperty) &&
					lazyImport.Metadata[MetadataFilterProperty].Equals(this.MetadataFilter);
			}

			return true;
		}
	}
}