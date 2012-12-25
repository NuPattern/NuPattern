using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Extensions ot the <see cref="CustomizableElementSchema"/> class.
	/// </summary>
	internal static class CustomizableElementSchemaExtensions
	{
		/// <summary>
		/// Ensures the customization policy and default settings are created.
		/// </summary>
		public static void EnsurePolicyAndDefaultSettings(this CustomizableElementSchema element)
		{
			// Add the customization policy
			if (element.Policy == null)
			{
				element.WithTransaction(elem => elem.Create<CustomizationPolicySchema>());
			}

			// Add the customizable settings derived from attributes on this model.
			CustomizableSettingSchema.EnsurePolicyPopulated(element);
		}
	}
}
