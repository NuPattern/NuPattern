using NuPattern.Extensibility.Binding;
using NuPattern.Library.Commands;
using NuPattern.Library.Conditions;
using NuPattern.Library.Events;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Customizations for the <see cref="ArtifactExtension"/> class.
    /// </summary>
    public partial class ArtifactExtension
    {
        private static readonly string navigateIconPath = ""; //"Resources/CommandNavigateSolutionItem.png";

        /// <summary>
        /// Ensures the associated commands and launchpoint automation are created and configured correctly.
        /// </summary>
        internal void EnsureActivateArtifactExtensionAutomation()
        {
            IPatternElementSchema element = this.Extends as IPatternElementSchema;

            // Configure the open command and menu.
            var openCommand = element.EnsureCommandAutomation<ActivateArtifactCommand>(
                Resources.ArtifactExtension_OpenCommandName,
                () => this.OnArtifactActivation == ArtifactActivatedAction.Open);
            if (openCommand != null)
            {
                openCommand.SetPropertyValue<ActivateArtifactCommand, bool>(cmd => cmd.Open, true);
            }

            var openMenu = element.EnsureMenuLaunchPoint(
                Resources.ArtifactExtension_OpenContextMenuName,
                openCommand,
                Resources.ArtifactExtension_OpenMenuItemText,
                null,
                () => this.OnArtifactActivation == ArtifactActivatedAction.Open);
            if (openMenu != null)
            {
                // Set the conditions
                openMenu.Conditions = GetSerializedLinkExistsCondition();
            }

            // Configure the select command and menu.
            var selectCommand = element.EnsureCommandAutomation<ActivateArtifactCommand>(
                Resources.ArtifactExtension_SelectCommandName,
                () => (this.OnArtifactActivation == ArtifactActivatedAction.Select || this.OnArtifactActivation == ArtifactActivatedAction.Open));
            if (selectCommand != null)
            {
                selectCommand.SetPropertyValue<ActivateArtifactCommand, bool>(cmd => cmd.Open, false);
            }

            var selectMenu = element.EnsureMenuLaunchPoint(
                Resources.ArtifactExtension_SelectContextMenuName,
                selectCommand,
                Resources.ArtifactExtension_SelectMenuItemText,
                navigateIconPath,
                () => (this.OnArtifactActivation == ArtifactActivatedAction.Select || this.OnArtifactActivation == ArtifactActivatedAction.Open));
            if (selectMenu != null)
            {
                // Set the conditions
                selectMenu.Conditions = GetSerializedLinkExistsCondition();
            }

            // Configure the activation event.
            var eventCommand = (this.OnArtifactActivation == ArtifactActivatedAction.Open ? openCommand : (this.OnArtifactActivation == ArtifactActivatedAction.Select ? selectCommand : null));
            var activateEvent = element.EnsureEventLaunchPoint<IOnElementActivatedEvent>(
                Resources.ArtifactExtension_ActivateEventName,
                eventCommand,
                true,
                () => this.OnArtifactActivation != ArtifactActivatedAction.None);
        }

        private string GetSerializedLinkExistsCondition()
        {
            // Set the conditions
            var condition =
                new[]
				{
					new ConditionBindingSettings
					{
						TypeId = typeof(ArtifactReferenceExistsCondition).FullName,
					}
				};

            return BindingSerializer.Serialize(condition);
        }
    }
}
