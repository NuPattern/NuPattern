using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Customizes the CustomizableElement domain class.
	/// </summary>
	[TypeDescriptionProvider(typeof(CustomizableElementTypeDescriptionProvider))]
	public partial class CustomizableElementSchemaBase
	{
		private CustomizationState customizable = CustomizationState.Inherited;

		/// <summary>
		/// Disables customization for the element.
		/// </summary>
		public void DisableCustomization()
		{
			// Update customization state.
			if (this.IsCustomizationEnabled)
			{
				this.IsCustomizable = CustomizationState.False;
			}

			// Disable customization on this element.
			this.IsCustomizationEnabled = false;

			// Disable all settings
			foreach (var setting in this.Policy.Settings)
			{
				setting.Disable();
			}
		}

		/// <summary>
		/// Returns the value of the IsCustomizationPolicyModifyable property.
		/// </summary>
		internal bool GetIsCustomizationPolicyModifyableValue()
		{
			if (this.IsCustomizationEnabled == false)
			{
				// Policy cannot be enabled if customization not enabled.
				return false;
			}
			else
			{
				switch (this.IsCustomizable)
				{
					case CustomizationState.Inherited:
						{
							// Calculate from parent (iterative)
							var parent = this.GetParentCustomizationElement();
							if (parent != null)
							{
								return parent.IsCustomizationPolicyModifyable;
							}
							else
							{
								// Should not be in this state (with no parent)
								throw new InvalidOperationException();
							}
						}

					case CustomizationState.True:
						return true;

					case CustomizationState.False:
						return false;

					default:
						throw new NotImplementedException();
				}
			}
		}

		/// <summary>
		/// Returns the value of the IsCustomizable property.
		/// </summary>
		internal CustomizationState GetIsCustomizableValue()
		{
			return this.customizable;
		}

		/// <summary>
		/// Sets the value of the IsCustomizable property.
		/// </summary>
		internal void SetIsCustomizableValue(CustomizationState value)
		{
			// Cannot update is already disabled.
			if (this.IsCustomizationEnabled == false)
			{
				return;
			}

			var parent = this.GetParentCustomizationElement();
			if (parent == null)
			{
				if (value == CustomizationState.Inherited)
				{
					// Cannot inherit from no parent.
					this.customizable = CustomizationState.True;
				}
				else
				{
					this.customizable = value;
				}
			}
			else
			{
				this.customizable = value;
			}
		}

		/// <summary>
		/// Returns the value of the IsCustomizationEnabledState value.
		/// </summary>
		internal CustomizationEnabledState GetIsCustomizationEnabledStateValue()
		{
			switch (this.IsCustomizable)
			{
				case Runtime.CustomizationState.False:
					return (this.IsCustomizationEnabled) ? CustomizationEnabledState.FalseEnabled : CustomizationEnabledState.FalseDisabled;
				case Runtime.CustomizationState.True:
					return (this.IsCustomizationEnabled) ? CustomizationEnabledState.TrueEnabled : CustomizationEnabledState.TrueDisabled;
				default:
					return (this.IsCustomizationEnabled) ? CustomizationEnabledState.InheritedEnabled : CustomizationEnabledState.InheritedDisabled;
			}
		}
	}
}
