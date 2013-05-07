using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Wrapper for ICommandSettings to be used in chaining. This class should not be used directly.
    /// </summary>
    internal class CommandSettingsDelegator<T> : ICommandSettings<T>
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
        public IList<IPropertyBindingSettings> Properties
        {
            get { return internalSettings.Properties; }
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
    }
}
