using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Library.Automation;
using NuPattern.Library.Commands;
using NuPattern.Library.Conditions;
using NuPattern.Library.Events;
using NuPattern.Library.Properties;
using NuPattern.Modeling;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Bindings.Design;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.IntegrationTests.Automation.Artifact
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
    public class ArtifactExtensionChangeRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnElement
        {
            private Store store;

            private IArtifactExtension artifactExtension;
            private IPatternElementSchema container;

            [TestInitialize]
            public void Initialize()
            {
                ElementSchema element = null;

                this.store = new Store(VsIdeTestHostContext.ServiceProvider,
                    new Type[] { typeof(CoreDesignSurfaceDomainModel), typeof(PatternModelDomainModel), typeof(LibraryDomainModel) });

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    element = this.store.ElementFactory.CreateElement<ElementSchema>();
                });

                this.artifactExtension = element.GetExtensions<IArtifactExtension>().FirstOrDefault();
                this.container = element as IPatternElementSchema;
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnArtifactActivationIsOpen_ThenCreatesAutomationSettings()
            {
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.Open;

                AssertOpenCommand(true);
                AssertOpenContextMenu(true);
                AssertSelectCommand(true);
                AssertSelectContextMenu(true);
                AssertActivateEvent(true, AssertOpenCommand(false));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnArtifactActivationIsSelect_ThenUpdatedCommandSettings()
            {
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.Select;

                AssertSelectCommand(true);
                AssertSelectContextMenu(true);
                AssertActivateEvent(true, AssertSelectCommand(false));
            }


            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnArtifactDeletionIsDeleteAll_ThenCreatesAutomationSettings()
            {
                this.artifactExtension.OnArtifactDeletion = ArtifactDeletedAction.DeleteAll;

                AssertDeleteCommand(true, DeleteAction.DeleteAll);
                AssertDeleteEvent(true, AssertDeleteCommand(false, DeleteAction.DeleteAll));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenChangingOnArtifactActivationFromOpenToSelect_ThenUpdatedCommandSettings()
            {
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.Open;
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.Select;

                Assert.Equal(3, this.container.AutomationSettings.Count());

                AssertSelectCommand(true);
                AssertSelectContextMenu(true);
                AssertActivateEvent(true, AssertSelectCommand(false));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenChangingOnArtifactDeletionFromDeleteAllToPromptUser_ThenUpdatedCommandSettings()
            {
                this.artifactExtension.OnArtifactDeletion = ArtifactDeletedAction.DeleteAll;
                this.artifactExtension.OnArtifactDeletion = ArtifactDeletedAction.PromptUser;

                Assert.Equal(2, this.container.AutomationSettings.Count());

                AssertDeleteCommand(true, DeleteAction.PromptUser);
                AssertDeleteEvent(true, AssertDeleteCommand(false, DeleteAction.PromptUser));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenChangingOnArtifactActivationFromSelectToOpen_ThenUpdatedCommandSettings()
            {
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.Select;
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.Open;

                Assert.Equal(5, this.container.AutomationSettings.Count());

                AssertSelectCommand(true);
                AssertSelectContextMenu(true);
                AssertOpenCommand(true);
                AssertOpenContextMenu(true);
                AssertActivateEvent(true, AssertOpenCommand(false));
            }


            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenChangingOnArtifactDeletionFromPromptUserToDeleteAll_ThenUpdatedCommandSettings()
            {
                this.artifactExtension.OnArtifactDeletion = ArtifactDeletedAction.PromptUser;
                this.artifactExtension.OnArtifactDeletion = ArtifactDeletedAction.DeleteAll;

                Assert.Equal(2, this.container.AutomationSettings.Count());

                AssertDeleteCommand(true, DeleteAction.DeleteAll);
                AssertDeleteEvent(true, AssertDeleteCommand(false, DeleteAction.DeleteAll));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenResettingOnArtifactActivation_ThenDeletesCommandSettingAndEventSettings()
            {
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.Open;
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.None;

                Assert.Equal(0, this.container.AutomationSettings.Count());
                Assert.Equal(0, this.container.Properties.Count());

                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.Select;
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.None;

                Assert.Equal(0, this.container.AutomationSettings.Count());
                Assert.Equal(0, this.container.Properties.Count());
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenResettingOnArtifactDeletion_ThenDeletesCommandSettingAndEventSettings()
            {
                this.artifactExtension.OnArtifactDeletion = ArtifactDeletedAction.DeleteAll;
                this.artifactExtension.OnArtifactDeletion = ArtifactDeletedAction.None;

                Assert.Equal(0, this.container.AutomationSettings.Count());
                Assert.Equal(0, this.container.Properties.Count());

                this.artifactExtension.OnArtifactDeletion = ArtifactDeletedAction.PromptUser;
                this.artifactExtension.OnArtifactDeletion = ArtifactDeletedAction.None;

                Assert.Equal(0, this.container.AutomationSettings.Count());
                Assert.Equal(0, this.container.Properties.Count());
            }

            private IAutomationSettingsSchema AssertOpenCommand(bool verifySettings)
            {
                var command = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ArtifactExtension_OpenCommandName));
                Assert.NotNull(command);

                var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();
                Assert.NotNull(commandSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, command.IsCustomizable);
                    Assert.True(command.IsSystem);

                    Assert.Equal(commandSettings.TypeId, typeof(ActivateArtifactCommand).FullName);

                    var openProperty = TypeDescriptor.GetProperties(commandSettings)[Reflector<ActivateArtifactCommand>.GetPropertyName(c => c.Open)]
                        .GetValue(commandSettings) as DesignProperty;
                    Assert.Equal(true, openProperty.GetValue());
                }

                return command;
            }

            private IAutomationSettingsSchema AssertOpenContextMenu(bool verifySettings)
            {
                var menu = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ArtifactExtension_OpenContextMenuName));
                Assert.NotNull(menu);

                var menuSettings = menu.GetExtensions<IMenuSettings>().FirstOrDefault();
                Assert.NotNull(menuSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, menu.IsCustomizable);
                    Assert.True(menu.IsSystem);

                    var command = AssertOpenCommand(false);
                    var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();

                    Assert.Equal(menuSettings.Text, Properties.Resources.ArtifactExtension_OpenMenuItemText);
                    Assert.Equal(menuSettings.CommandId, commandSettings.Id);
                    Assert.Equal(menuSettings.WizardId, Guid.Empty);

                }

                return menu;
            }

            private IAutomationSettingsSchema AssertSelectCommand(bool verifySettings)
            {
                var command = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ArtifactExtension_SelectCommandName));
                Assert.NotNull(command);

                var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();
                Assert.NotNull(commandSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, command.IsCustomizable);
                    Assert.True(command.IsSystem);

                    Assert.Equal(commandSettings.TypeId, typeof(ActivateArtifactCommand).FullName);

                    var openProperty = TypeDescriptor.GetProperties(commandSettings)[Reflector<ActivateArtifactCommand>.GetPropertyName(c => c.Open)]
                        .GetValue(commandSettings) as DesignProperty;
                    Assert.Equal(false, openProperty.GetValue());
                }

                return command;
            }

            private IAutomationSettingsSchema AssertSelectContextMenu(bool verifySettings)
            {
                var menu = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ArtifactExtension_SelectContextMenuName));
                Assert.NotNull(menu);

                var menuSettings = menu.GetExtensions<MenuSettings>().FirstOrDefault();
                Assert.NotNull(menuSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, menu.IsCustomizable);
                    Assert.True(menu.IsSystem);

                    var command = AssertSelectCommand(false);
                    var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();

                    Assert.Equal(menuSettings.Text, Resources.ArtifactExtension_SelectMenuItemText);
                    Assert.Equal(menuSettings.CommandId, commandSettings.Id);
                    Assert.Equal(menuSettings.WizardId, Guid.Empty);

                    var conditions = BindingSerializer.Serialize(
                        new[]
                        {
                            new ConditionBindingSettings
                            {
                                TypeId = typeof(ArtifactReferenceExistsCondition).FullName,
                            }
                        });

                    Assert.Equal(menuSettings.Conditions, conditions);
                }

                return menu;
            }

            private IAutomationSettingsSchema AssertActivateEvent(bool verifySettings, IAutomationSettingsSchema command)
            {
                var event1 = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ArtifactExtension_ActivateEventName));
                Assert.NotNull(event1);

                var eventSettings = event1.GetExtensions<IEventSettings>().FirstOrDefault();
                Assert.NotNull(eventSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, event1.IsCustomizable);
                    Assert.True(event1.IsSystem);

                    Assert.Equal(eventSettings.EventId, typeof(IOnElementActivatedEvent).FullName);
                    Assert.Equal(eventSettings.FilterForCurrentElement, true);

                    var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();
                    Assert.NotNull(commandSettings);

                    Assert.Equal(eventSettings.CommandId, commandSettings.Id);
                }

                return event1;
            }

            private IAutomationSettingsSchema AssertDeleteCommand(bool verifySettings, DeleteAction action)
            {
                var command = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ArtifactExtension_DeleteCommandName));
                Assert.NotNull(command);

                var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();
                Assert.NotNull(commandSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, command.IsCustomizable);
                    Assert.True(command.IsSystem);

                    Assert.Equal(commandSettings.TypeId, typeof(DeleteArtifactsCommand).FullName);

                    var deleteProperty = (DeleteAction)TypeDescriptor.GetProperties(commandSettings)[Reflector<DeleteArtifactsCommand>.GetPropertyName(c => c.Action)]
                        .GetValue(commandSettings);
                    Assert.Equal(action, deleteProperty);
                }

                return command;
            }

            private IAutomationSettingsSchema AssertDeleteEvent(bool verifySettings, IAutomationSettingsSchema command)
            {
                var event1 = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ArtifactExtension_DeleteEventName));
                Assert.NotNull(event1);

                var eventSettings = event1.GetExtensions<IEventSettings>().FirstOrDefault();
                Assert.NotNull(eventSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, event1.IsCustomizable);
                    Assert.True(event1.IsSystem);

                    Assert.Equal(eventSettings.EventId, typeof(IOnElementDeletingEvent).FullName);
                    Assert.Equal(eventSettings.FilterForCurrentElement, true);

                    var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();
                    Assert.NotNull(commandSettings);

                    Assert.Equal(eventSettings.CommandId, commandSettings.Id);
                }

                return event1;
            }
        }
    }
}
