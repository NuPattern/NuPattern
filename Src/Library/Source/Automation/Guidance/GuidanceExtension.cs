using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility.References;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Library.Conditions;
using Microsoft.VisualStudio.Patterning.Library.Events;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Bindings = Microsoft.VisualStudio.Patterning.Extensibility.Binding;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
    /// <summary>
    /// Customizations for the <see cref="GuidanceExtension"/> class.
    /// </summary>
    public partial class GuidanceExtension
    {
        private static readonly string guidanceIconPath = ""; //"Resources/ShowGuidanceIcon.png";

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
                activateMenu.Conditions = Bindings.BindingSerializer.Serialize(
                    new List<Bindings.ConditionBindingSettings>
					{
						new Bindings.ConditionBindingSettings
						{
							TypeId = typeof(ElementReferenceExistsCondition).FullName,
							Properties =
							{
								new Bindings.PropertyBindingSettings
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
