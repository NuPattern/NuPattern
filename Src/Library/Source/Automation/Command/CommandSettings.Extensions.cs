using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <summary>
	/// Extensions for <see cref="CommandSettings"/>.
	/// </summary>
	internal static class CommandSettingsExtensions
	{
		/// <summary>
		/// Sets the specified property of the given CommandSettings to the specified value.
		/// </summary>
		public static void SetPropertyValue<TFeatureCommand, TProperty>(
			this CommandSettings commandSettings,
			Expression<Func<TFeatureCommand, TProperty>> expression,
			object value) where TFeatureCommand : TeamArchitect.PowerTools.Features.IFeatureCommand
		{
			var descriptors = TypeDescriptor.GetProperties(commandSettings);
			var descriptor = descriptors[Reflector<TFeatureCommand>.GetPropertyName(expression)];
			if (descriptor != null)
			{
				var designProperty = descriptor.GetValue(commandSettings) as DesignProperty;
				if (designProperty != null)
				{
					designProperty.Value = value;
				}
			}
		}
	}
}