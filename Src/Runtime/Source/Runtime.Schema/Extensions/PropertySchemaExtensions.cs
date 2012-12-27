using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Immutability;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Extensions methods for the <see cref="PropertySchema"/> class.
	/// </summary>
	internal static class PropertySchemaExtensions
	{
		/// <summary>
		/// Determines if the Property usage is ExtensionContract
		/// </summary>
		public static bool IsUsageExtensionPoint(this PropertySchema property)
		{
			return ((property.PropertyUsage & Runtime.PropertyUsages.ExtensionContract) == Runtime.PropertyUsages.ExtensionContract);
		}

		/// <summary>
		/// Sets the property usage to ExtensionContract
		/// </summary>
		public static void SetUsageExtensionPoint(this PropertySchema property)
		{
			property.PropertyUsage |= Runtime.PropertyUsages.ExtensionContract;
		}

		/// <summary>
		/// Resets the property usage to General
		/// </summary>
		public static void SetUsageGeneral(this PropertySchema property)
		{
			property.PropertyUsage = Runtime.PropertyUsages.General;
		}

		/// <summary>
		/// Creates a copy of the given property as a <see cref="PropertyUsages.ExtensionContract"/> property.
		/// </summary>
		public static PropertySchema CloneAsExtensionContractProperty(this PropertySchema source, PatternElementSchema owner)
		{
			Guard.NotNull(() => source, source);
			Guard.NotNull(() => owner, owner);

			PropertySchema newProperty = null;
			owner.Store.TransactionManager.DoWithinTransaction(() =>
				{
					newProperty = owner.Store.ElementFactory.CreateElement<PropertySchema>()
						.With(p =>
						{
							p.BaseId = source.BaseId;
							p.Category = source.Category;
							p.RawDefaultValue = source.RawDefaultValue;
							p.Description = source.Description;
							p.DisplayName = source.DisplayName;
							p.EditorTypeName = source.EditorTypeName;
							p.IsCustomizable = source.IsCustomizable;
							p.IsReadOnly = source.IsReadOnly;
							p.IsSystem = source.IsSystem;
							p.IsVisible = source.IsVisible;
							p.Name = source.Name;
							p.Type = source.Type;
							p.TypeConverterTypeName = source.TypeConverterTypeName;
							p.RawValidationRules = source.RawValidationRules;
							p.EnsurePolicyAndDefaultSettings();
						});
				});

			// Copy customization settings
			foreach (var srcSetting in source.Policy.Settings)
			{
				var setting = newProperty.Policy.Settings.First(s => s.PropertyId == srcSetting.PropertyId);
				setting.Value = srcSetting.Value;
				if (!setting.Value)
				{
					setting.Disable();
				}
			}

			// Apply Tailoring customization rules
			if (source.IsCustomizable == CustomizationState.False
				|| (source.IsCustomizable == CustomizationState.Inherited && source.GetAncestorCustomizationState() == CustomizationState.False))
			{
				newProperty.DisableCustomization();
			}

			// Convert to a contract property
			newProperty.MakeContractProperty(source.Id.ToString());

			return newProperty;
		}

		/// <summary>
		/// Deletes all variable properties (regular or contract) from the given extension point.
		/// </summary>
		/// <param name="extensionPoint"></param>
		internal static void ClearAllProperties(this ExtensionPointSchema extensionPoint)
		{
			Guard.NotNull(() => extensionPoint, extensionPoint);

			var variableProperties = extensionPoint.Properties.ToList();

			foreach (var vp in variableProperties)
			{
				vp.SetLocks(Locks.None);
				extensionPoint.Properties.Remove(vp);
			}
		}

		/// <summary>
		/// Copies the properties from given source extension point to the given target extension point.
		/// </summary>
		internal static void CopyHostedContractProperties(this ExtensionPointSchema targetExtensionPoint, IExtensionPointSchema sourceExtensionPoint)
		{
			Guard.NotNull(() => targetExtensionPoint, targetExtensionPoint);
			Guard.NotNull(() => sourceExtensionPoint, sourceExtensionPoint);

			foreach (var property in sourceExtensionPoint.Properties)
			{
				var prop = property as PropertySchema;

				targetExtensionPoint.Properties.Add(prop.CloneAsExtensionContractProperty(targetExtensionPoint));
			}
		}

		/// <summary>
		/// Clears the collection of provided extension points in the pattern.
		/// </summary>
		internal static void ClearProvidedExtensionPoints(this PatternSchema product)
		{
			Guard.NotNull(() => product, product);

			product.ClearAllProvidedContractProperties();
			product.ProvidedExtensionPoints.Clear();
		}

		/// <summary>
		/// Adds the provided extension point to the given pattern.
		/// </summary>
		internal static void AddProvidedExtensionPoint(this PatternSchema product, IExtensionPointSchema extensionPoint)
		{
			Guard.NotNull(() => product, product);

			product.ProvidedExtensionPoints.Add(
				product.Store.ElementFactory.CreateElement<ProvidedExtensionPointSchema>()
					.With(ext => ext.ExtensionPointId = extensionPoint.RequiredExtensionPointId));
		}

		/// <summary>
		/// Adds the contract properties from the given extension point to the pattern.
		/// </summary>
		internal static void CopyProvidedContractProperties(this PatternSchema product, IExtensionPointSchema extensionPoint)
		{
			Guard.NotNull(() => product, product);
			Guard.NotNull(() => extensionPoint, extensionPoint);

			foreach (var property in extensionPoint.Properties)
			{
				if (!product.Properties.Any(
					p => p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase) &&
						 p.Type.Equals(property.Type, StringComparison.OrdinalIgnoreCase)))
				{
					var prop = property as PropertySchema;
					product.Properties.Add(prop.CloneAsExtensionContractProperty(product));
				}
			}
		}

		/// <summary>
		/// Deletes all the contract properties from the given pattern element.
		/// </summary>
		internal static void RevertAllContractProperties(this ExtensionPointSchema extensionPoint)
		{
			Guard.NotNull(() => extensionPoint, extensionPoint);

			var contractProperties = extensionPoint.GetContractProperties();
			foreach (var vp in contractProperties)
			{
				vp.MakeVariableProperty();
			}
		}

		/// <summary>
		/// Returns the contract properties for this element.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		private static IEnumerable<PropertySchema> GetContractProperties(this PatternElementSchema element)
		{
			return element.Properties.Where(vp => vp.IsUsageExtensionPoint()).ToList();
		}

		/// <summary>
		/// Deletes all the contract properties for the given providedExtension point.
		/// </summary>
		private static void ClearAllProvidedContractProperties(this PatternElementSchema element)
		{
			Guard.NotNull(() => element, element);

			var contractProperties = element.GetContractProperties();
			foreach (var vp in contractProperties)
			{
				vp.SetLocks(Locks.None);
				element.Properties.Remove(vp);
			}
		}

		private static void MakeVariableProperty(this PropertySchema property)
		{
			property.SetLocks(Locks.None);

			property.BaseId = string.Empty;
			property.SetUsageGeneral();
		}

		private static void MakeContractProperty(this PropertySchema property, string baseId)
		{
			property.BaseId = baseId;
			property.SetUsageExtensionPoint();

			property.SetLocks(Locks.Delete);
		}
	}
}
