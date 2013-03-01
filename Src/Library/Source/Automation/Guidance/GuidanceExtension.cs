using System;
using System.Collections.Generic;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Library.Commands;
using NuPattern.Library.Conditions;
using NuPattern.Library.Events;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using Bindings = NuPattern.Extensibility.Binding;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Customizations for the <see cref="GuidanceExtension"/> class.
    /// </summary>
    public partial class GuidanceExtension
    {
        private static readonly string guidanceIconPath = ""; //"Resources/CommandShowGuidance.png";

        /// <summary>
        /// Ensures the associated commands and launchpoint automation are created and configured correctly.
        /// </summary>
        internal void EnsureGuidanceExtensionAutomation()
        {
            IPatternElementSchema element = this.Extends as IPatternElementSchema;
            Func<bool> existanceCondition = () => !string.IsNullOrEmpty(this.GuidanceFeatureId);

            // Configure the instantiate command, event.
            var instantiateCommand = element.EnsureCommandAutomation<InstantiateFeatureCommand>(Properties.Resources.GuidanceExtension_InstantiateCommandName,
                existanceCondition);
            if (instantiateCommand != null)
            {
                instantiateCommand.SetPropertyValue<InstantiateFeatureCommand, string>(cmd => cmd.FeatureId, this.GuidanceFeatureId);
                instantiateCommand.SetPropertyValue<InstantiateFeatureCommand, string>(cmd => cmd.DefaultInstanceName, this.GuidanceInstanceName);
                instantiateCommand.SetPropertyValue<InstantiateFeatureCommand, bool>(cmd => cmd.SharedInstance, this.GuidanceSharedInstance);
                instantiateCommand.SetPropertyValue<InstantiateFeatureCommand, bool>(cmd => cmd.ActivateOnInstantiation, this.GuidanceActivateOnCreation);
            }

            var instantiateEvent = element.EnsureEventLaunchPoint<IOnElementInstantiatedEvent>(Properties.Resources.GuidanceExtension_InstantiateEventName,
                instantiateCommand, true, () => !String.IsNullOrEmpty(this.GuidanceFeatureId));

            // Configure the activate command and menu.
            var activateCommand = element.EnsureCommandAutomation<ActivateFeatureCommand>(Properties.Resources.GuidanceExtension_ActivateCommandName,
                existanceCondition);

            var activateMenu = element.EnsureMenuLaunchPoint(Resources.GuidanceExtension_ActivateContextMenuName,
                activateCommand, Resources.GuidanceExtension_ActivateMenuItemText, guidanceIconPath,
                existanceCondition);
            if (activateMenu != null)
            {
                // Set the conditions
                var conditionBindings = new Bindings.ConditionBindingSettings { TypeId = typeof(ElementReferenceExistsCondition).FullName };
                var property = conditionBindings.AddProperty(Reflector<ElementReferenceExistsCondition>.GetPropertyName(cond => cond.Kind), typeof(string));
                property.Value = ReferenceKindConstants.Guidance;

                activateMenu.Conditions = Bindings.BindingSerializer.Serialize(
                    new List<Bindings.ConditionBindingSettings>
                    {
                        conditionBindings,
                    });
            }
        }
    }
}
