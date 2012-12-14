
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Modeling.Integration;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	[System.CodeDom.Compiler.GeneratedCode("T4", "1.0")]
	public partial class AdapterManager
	{
		/// <summary>
		/// Gets the file extension.
		/// </summary>
		public string FileExtension
		{
			get { return ".patterndefinition"; }
		}

		/// <summary>
		/// Gets the exposed element types.
		/// </summary>
		public override IEnumerable<SupportedType> GetExposedElementTypes(string adapterId)
		{
			if (!GetSupportedLogicalAdapterIds().Contains(adapterId))
				yield break;

			yield return new SupportedType(typeof(IPatternModelSchema), "PatternModelSchema");
			yield return new SupportedType(typeof(IPatternModelInfo), "PatternModelSchema");
			yield return new SupportedType(typeof(IPatternSchema), "PatternSchema");
			yield return new SupportedType(typeof(IPatternInfo), "PatternSchema");
			yield return new SupportedType(typeof(IPropertySchema), "PropertySchema");
			yield return new SupportedType(typeof(IPropertyInfo), "PropertySchema");
			yield return new SupportedType(typeof(IViewSchema), "ViewSchema");
			yield return new SupportedType(typeof(IViewInfo), "ViewSchema");
			yield return new SupportedType(typeof(ICollectionSchema), "CollectionSchema");
			yield return new SupportedType(typeof(ICollectionInfo), "CollectionSchema");
			yield return new SupportedType(typeof(IElementSchema), "ElementSchema");
			yield return new SupportedType(typeof(IElementInfo), "ElementSchema");
			yield return new SupportedType(typeof(ICustomizationPolicySchema), "CustomizationPolicySchema");
			yield return new SupportedType(typeof(ICustomizationPolicyInfo), "CustomizationPolicySchema");
			yield return new SupportedType(typeof(ICustomizableSettingSchema), "CustomizableSettingSchema");
			yield return new SupportedType(typeof(ICustomizableSettingInfo), "CustomizableSettingSchema");
			yield return new SupportedType(typeof(IAutomationSettingsSchema), "AutomationSettingsSchema");
			yield return new SupportedType(typeof(IAutomationSettingsInfo), "AutomationSettingsSchema");
			yield return new SupportedType(typeof(IProvidedExtensionPointSchema), "ProvidedExtensionPointSchema");
			yield return new SupportedType(typeof(IProvidedExtensionPointInfo), "ProvidedExtensionPointSchema");
			yield return new SupportedType(typeof(IExtensionPointSchema), "ExtensionPointSchema");
			yield return new SupportedType(typeof(IExtensionPointInfo), "ExtensionPointSchema");
		}
	}
}