using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;

namespace NuPattern.Runtime.Store
{
	/// <summary>
	/// Runtime Validation Extension Registration.
	/// </summary>
	internal partial class RuntimeValidationExtensionRegistrar : ValidationExtensionRegistrar
	{
		/// <summary>
		/// String-based Metadata Key that determines if a particular Exported Type can be imported or not.
		/// The default CanImport implementation filters imports based on this metadata key.
		/// The default value of this property is null indicating that no filter will be applied.
		/// </summary>
		protected override string MetadataFilter
		{
			get
			{
				return ValidationConstants.MetadataFilter;
			}
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
				return lazyImport.Metadata.ContainsKey("MetadataFilter") &&
					lazyImport.Metadata["MetadataFilter"].Equals(this.MetadataFilter);
			}

			return true;
		}
	}
}
