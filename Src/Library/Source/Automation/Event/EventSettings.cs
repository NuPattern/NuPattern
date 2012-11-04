using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Library.Conditions;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	[TypeDescriptionProvider(typeof(EventSettingsDescriptionProvider))]
	public partial class EventSettings
	{
		private static readonly string ConditionTypeId = typeof(EventSenderMatchesElementCondition).FullName;

		private List<ConditionBindingSettings> conditionSettings;

		/// <summary>
		/// Gets the classification of these settings.
		/// </summary>
		public AutomationSettingsClassification Classification
		{
			get { return AutomationSettingsClassification.LaunchPoint; }
		}

		/// <summary>
		/// Gets the condition settings.
		/// </summary>
		public IEnumerable<IBindingSettings> ConditionSettings
		{
			get { return this.conditionSettings ?? (this.conditionSettings = this.GetConditionSettings()); }
		}

		/// <summary>
		/// Creates the runtime automation element for this setting element.
		/// </summary>
		public IAutomationExtension CreateAutomation(IProductElement owner)
		{
			return new EventAutomation(owner, this);
		}

		/// <summary>
		/// Deletes the condition for filtering.
		/// </summary>
		internal void DeleteConditionForFiltering()
		{
			// Delete the condition (if exists)
			var condition = this.ConditionSettings.SingleOrDefault(cond => cond.TypeId == ConditionTypeId) as ConditionBindingSettings;
			if (condition != null)
			{
				this.conditionSettings.Remove(condition);
				this.Conditions = BindingSerializer.Serialize(this.conditionSettings);
			}
		}

		/// <summary>
		/// Ensures the condition for filtering is applied to element.
		/// </summary>
		internal void EnsureConditionForFiltering()
		{
			var condition = this.ConditionSettings.SingleOrDefault(cond => cond.TypeId == ConditionTypeId);
			if (condition == null)
			{
				// Add the condition
				this.conditionSettings.Add(new ConditionBindingSettings { TypeId = ConditionTypeId });
				this.Conditions = BindingSerializer.Serialize(this.conditionSettings);
			}
		}

		private List<ConditionBindingSettings> GetConditionSettings()
		{
			if (string.IsNullOrEmpty(this.Conditions))
			{
				return new List<ConditionBindingSettings>();
			}

			return BindingSerializer.Deserialize<List<ConditionBindingSettings>>(this.Conditions);
		}
	}
}