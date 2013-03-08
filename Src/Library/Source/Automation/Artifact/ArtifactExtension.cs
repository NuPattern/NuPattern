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
        private const string OpenIconPath = "";//"resources/commandopensolutionitem.png";
        private const string NavigateIconPath = "";//"resources/commandnavigatesolutionitem.png";

        /// <summary>
        /// Ensures the associated commands and launchpoint automation are created and configured correctly.
        /// </summary>
        internal void EnsureActivateArtifactExtensionAutomation()
        {
            var element = this.Extends as IPatternElementSchema;

            // Configure the open command.
            var openCommand = element.EnsureCommandAutomation<ActivateArtifactCommand>(
                Resources.ArtifactExtension_OpenCommandName,
                () => this.OnArtifactActivation == ArtifactActivatedAction.Open);
            if (openCommand != null)
            {
                openCommand.SetPropertyValue<ActivateArtifactCommand, bool>(cmd => cmd.Open, true);
            }

            // Configure the open menu launchpoint.
            var openMenu = element.EnsureMenuLaunchPoint(
                Resources.ArtifactExtension_OpenContextMenuName,
                openCommand,
                Resources.ArtifactExtension_OpenMenuItemText,
                OpenIconPath,
                () => this.OnArtifactActivation == ArtifactActivatedAction.Open);
            if (openMenu != null)
            {
                // Set the conditions
                openMenu.Conditions = GetSerializedLinkExistsCondition();
            }

            // Configure the select command.
            var selectCommand = element.EnsureCommandAutomation<ActivateArtifactCommand>(
                Resources.ArtifactExtension_SelectCommandName,
                () => (this.OnArtifactActivation == ArtifactActivatedAction.Select || this.OnArtifactActivation == ArtifactActivatedAction.Open));
            if (selectCommand != null)
            {
                selectCommand.SetPropertyValue<ActivateArtifactCommand, bool>(cmd => cmd.Open, false);
            }

            // Configure the select menu launchpoint.
            var selectMenu = element.EnsureMenuLaunchPoint(
                Resources.ArtifactExtension_SelectContextMenuName,
                selectCommand,
                Resources.ArtifactExtension_SelectMenuItemText,
                NavigateIconPath,
                () => (this.OnArtifactActivation == ArtifactActivatedAction.Select || this.OnArtifactActivation == ArtifactActivatedAction.Open));
            if (selectMenu != null)
            {
                // Set the conditions
                selectMenu.Conditions = GetSerializedLinkExistsCondition();
            }

            // Configure the event launchpoint.
            var eventCommand = (this.OnArtifactActivation == ArtifactActivatedAction.Open ? openCommand : (this.OnArtifactActivation == ArtifactActivatedAction.Select ? selectCommand : null));
            element.EnsureEventLaunchPoint<IOnElementActivatedEvent>(
                Resources.ArtifactExtension_ActivateEventName,
                eventCommand,
                true,
                () => this.OnArtifactActivation != ArtifactActivatedAction.None);
        }

        /// <summary>
        /// Ensures the associated commands and launchpoint automation are created and configured correctly.
        /// </summary>
        internal void EnsureDeleteArtifactExtensionAutomation()
        {
            var element = this.Extends as IPatternElementSchema;

            // Configure the delete command.
            var deleteCommand = element.EnsureCommandAutomation<DeleteArtifactsCommand>(
                Resources.ArtifactExtension_DeleteCommandName,
                () => this.OnArtifactDeletion != ArtifactDeletedAction.None);
            if (deleteCommand != null)
            {
                var deleteAction = (this.OnArtifactDeletion == ArtifactDeletedAction.DeleteAll) ? DeleteAction.DeleteAll : DeleteAction.PromptUser;
                deleteCommand.SetPropertyValue<DeleteArtifactsCommand, DeleteAction>(cmd => cmd.Action, deleteAction);
            }

            // Configure the event launchpoint.
            element.EnsureEventLaunchPoint<IOnElementDeletingEvent>(
                Resources.ArtifactExtension_DeleteEventName,
                deleteCommand,
                true,
                () => this.OnArtifactDeletion != ArtifactDeletedAction.None);
        }
        private static string GetSerializedLinkExistsCondition()
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
