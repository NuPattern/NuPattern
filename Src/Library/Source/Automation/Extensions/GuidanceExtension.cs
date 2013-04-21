using System;
using System.Collections.Generic;
using NuPattern.Library.Commands;
using NuPattern.Library.Conditions;
using NuPattern.Library.Events;
using NuPattern.Library.Properties;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.References;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Customizations for the <see cref="GuidanceExtension"/> class.
    /// </summary>
    partial class GuidanceExtension
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
            var instantiateCommand = element.EnsureCommandAutomation<InstantiateGuidanceWorkflowCommand>(Properties.Resources.GuidanceExtension_InstantiateCommandName,
                existanceCondition);
            if (instantiateCommand != null)
            {
                instantiateCommand.SetPropertyValue<InstantiateGuidanceWorkflowCommand, string>(cmd => cmd.GuidanceExtensionId, this.GuidanceFeatureId);
                instantiateCommand.SetPropertyValue<InstantiateGuidanceWorkflowCommand, string>(cmd => cmd.DefaultInstanceName, this.GuidanceInstanceName);
                instantiateCommand.SetPropertyValue<InstantiateGuidanceWorkflowCommand, bool>(cmd => cmd.SharedInstance, this.GuidanceSharedInstance);
                instantiateCommand.SetPropertyValue<InstantiateGuidanceWorkflowCommand, bool>(cmd => cmd.ActivateOnInstantiation, this.GuidanceActivateOnCreation);
            }

            element.EnsureEventLaunchPoint<IOnElementInstantiatedEvent>(Properties.Resources.GuidanceExtension_InstantiateEventName,
                instantiateCommand, () => !String.IsNullOrEmpty(this.GuidanceFeatureId));

            // Configure the activate command and menu.
            var activateCommand = element.EnsureCommandAutomation<ActivateGuidanceWorkflowCommand>(Properties.Resources.GuidanceExtension_ActivateCommandName,
                existanceCondition);

            var activateMenu = element.EnsureMenuLaunchPoint(Resources.GuidanceExtension_ActivateContextMenuName,
                activateCommand, Resources.GuidanceExtension_ActivateMenuItemText, guidanceIconPath,
                existanceCondition);
            if (activateMenu != null)
            {
                // Set the conditions
                activateMenu.Conditions = BindingSerializer.Serialize(
                    new List<ConditionBindingSettings>
                    {
                        new ConditionBindingSettings
                        {
                            TypeId = typeof(ElementReferenceExistsCondition).FullName,
                            Properties =
                            {
                                new PropertyBindingSettings
                                {
                                    Name = Reflector<ElementReferenceExistsCondition>.GetPropertyName(cond => cond.Kind),
                                    Value = ReferenceKindConstants.Guidance
                                },
                            }
                        }
                    });
            }
        }
    }
}
