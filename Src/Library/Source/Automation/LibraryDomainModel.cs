using System;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Library domain model customization.
    /// </summary>
    public partial class LibraryDomainModel
    {
        /// <summary>
        /// Gets the list of non-generated domain model types.
        /// </summary>
        /// <returns>List of types.</returns>
        protected override Type[] GetCustomDomainModelTypes()
        {
            return new[]
			{ 
				typeof(PatternElementSchemaAddRule),
				typeof(GuidanceExtensionChangeRule),
				typeof(ArtifactExtensionChangeRule),
				typeof(ValidationExtensionChangeRule),
                typeof(EventSettingsChangeRule),
                typeof(AggregatorCommandCommandSettingsDeletingRule),
				typeof(TemplateSettingsChangeRule),
                typeof(UnfoldVsTemplateCommandChangeRule),
			};
        }
    }
}