using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
	/// <summary>
	/// Extensions to be used in Command Settings to set command properties at runtime
	/// </summary>
	public static class CommandSettingExtensions
	{
		/// <summary>
		/// Sets a property in a command at runtime.
		/// </summary>
		public static ICommandSettings<TCommand> SetProperty<TCommand, TPropertyType>(this ICommandSettings<TCommand> commandSettings, Expression<Func<TCommand, TPropertyType>> propertyName, string value)
		{
			Guard.NotNull(() => commandSettings, commandSettings);

			var prop = commandSettings.CreatePropertySettings();
			prop.Name = Reflector<TCommand>.GetPropertyName(propertyName);
			prop.Value = value;
			return commandSettings;
		}

		/// <summary>
		/// Gets the value of the property, creating it with a default if needed
		/// </summary>
		public static T GetOrCreatePropertyValue<T>(this ICommandSettings settings, string propertyName, T defaultValue)
		{
			Guard.NotNull(() => settings, settings);

			var converter = TypeDescriptor.GetConverter(typeof(T));
			var prop = (IPropertyBindingSettings)settings.Properties.FirstOrDefault(p => p.Name == propertyName);
			if (prop == null)
			{
				prop = settings.CreatePropertySettings(p =>
				{
					p.Name = propertyName;
					p.Value = converter.ConvertToString(defaultValue);
				});
			}

			return (T)converter.ConvertFromString(prop.Value);
		}


	}

	/// <summary>
	/// Wrapper for ICommandSettings used in chaining.
	/// </summary>
	public interface ICommandSettings<T> : ICommandSettings
	{
	}

	/// <summary>
	/// Wrapper for ICommandSettings to be used in chaining. This class should not be used directly.
	/// </summary>
	public class CommandSettingsDelegator<T> : ICommandSettings, ICommandSettings<T>
	{
		private ICommandSettings internalSettings;

		/// <summary>
		/// Wrapper constructor
		/// </summary>
		/// <param name="internalSettings">Settings to be wrapped</param>
		public CommandSettingsDelegator(ICommandSettings internalSettings)
		{
			Guard.NotNull(() => internalSettings, internalSettings);

			this.internalSettings = internalSettings;
			internalSettings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(internalSettings_PropertyChanged);
		}

		void internalSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(sender, e);
			}
		}

		/// <summary>
		/// Wrapper for SuscribeChanged
		/// </summary>
		public IDisposable SubscribeChanged(Expression<Func<ICommandSettings, object>> propertyExpression, Action<ICommandSettings> callbackAction)
		{
			return SubscribeChanged(propertyExpression, callbackAction);
		}

		/// <summary>
		/// Wrapper for TypeId
		/// </summary>
		public string TypeId
		{
			get { return internalSettings.TypeId; }
			set { internalSettings.TypeId = value; }
		}

		/// <summary>
		/// Wrapper for Properties
		/// </summary>
		public IEnumerable<IPropertyBindingSettings> Properties
		{
			get { return internalSettings.Properties; }
		}

		/// <summary>
		/// Wrapper for CreatePropertySettings
		/// </summary>
		public Runtime.IPropertyBindingSettings CreatePropertySettings(Action<Runtime.IPropertyBindingSettings> initializer = null)
		{
			return internalSettings.CreatePropertySettings(initializer);
		}

		/// <summary>
		/// Wrapper for PropertyChanged event
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Wrapper for Id
		/// </summary>
		public Guid Id
		{
			get { return internalSettings.Id; }
		}

		/// <summary>
		/// Wrapper for Name
		/// </summary>
		public string Name
		{
			get { return internalSettings.Name; }
		}

		/// <summary>
		/// Wrapper for Owner
		/// </summary>
		public Runtime.IPatternElementSchema Owner
		{
			get { return internalSettings.Owner; }
		}

		/// <summary>
		/// Wrapper for CreateAutomation
		/// </summary>
		public Runtime.IAutomationExtension CreateAutomation(Runtime.IProductElement owner)
		{
			return internalSettings.CreateAutomation(owner);
		}

		/// <summary>
		/// Wrapper for Classification
		/// </summary>
		public Runtime.AutomationSettingsClassification Classification
		{
			get { return internalSettings.Classification; }
		}

		/// <summary>
		/// Wrapper for Properties
		/// </summary>
		IList<IPropertyBindingSettings> IBindingSettings.Properties
		{
			get { return new ReadOnlyCollection<IPropertyBindingSettings>(this.Properties.ToList()); }
		}
	}
}
