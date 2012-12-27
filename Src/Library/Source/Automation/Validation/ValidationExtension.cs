using System;
using NuPattern.Library.Commands;
using NuPattern.Library.Events;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Customization for the <see cref="ValidationExtension"/> class.
    /// </summary>
    public partial class ValidationExtension
    {
        /// <summary>
        /// Ensures the associated commands and launchpoint automation are created and configured correctly.
        /// </summary>
        internal void EnsureValidationExtensionAutomation()
        {
            IPatternElementSchema element = this.Extends as IPatternElementSchema;

            // Configure the open command.
            var validateCommand = element.EnsureCommandAutomation<ValidateElementCommand>(
                Resources.ValidationExtension_ValidateCommandName,
                () => (this.ValidationOnBuild || this.ValidationOnMenu || this.ValidationOnSave || !String.IsNullOrEmpty(this.ValidationOnCustomEvent)));
            if (validateCommand != null)
            {
                validateCommand.SetPropertyValue<ValidateElementCommand, bool>(cmd => cmd.ValidateDescendants, true);
            }

            // Configure events.
            element.EnsureEventLaunchPoint<IOnProductStoreSavedEvent>(Resources.ValidationExtension_OnSaveEventName, validateCommand,
                false, () => this.ValidationOnSave);
            element.EnsureEventLaunchPoint<IOnBuildStartedEvent>(Resources.ValidationExtension_OnBuildEventName, validateCommand,
                false, () => this.ValidationOnBuild);
            element.EnsureEventLaunchPoint(this.ValidationOnCustomEvent, Resources.ValidationExtension_OnCustomEventName, validateCommand,
                false, () => !String.IsNullOrEmpty(this.ValidationOnCustomEvent));

            // Configure menu.
            element.EnsureMenuLaunchPoint(Resources.ValidationExtension_OnMenuContextMenuName, validateCommand,
                Resources.ValidationExtension_OnMenuMenuItemText, null,
                () => this.ValidationOnMenu);
        }
    }
}
