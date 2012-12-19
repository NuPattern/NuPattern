using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal abstract partial class AuthoringValidationExtensionRegistrarBase : ValidationExtensionRegistrar
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
		/// Allow registrars that match the metadatafilter with a key in their metadata to be imported.
		/// </summary>
		/// <param name="lazyImport">The imports to verify.</param>
		protected override bool CanImport(Lazy<Delegate, IDictionary<string, object>> lazyImport)
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