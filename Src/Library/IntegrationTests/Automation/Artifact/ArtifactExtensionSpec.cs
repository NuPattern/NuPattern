using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Library.Conditions;
using Microsoft.VisualStudio.Patterning.Library.Events;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Patterning.Runtime.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace Microsoft.VisualStudio.Patterning.Library.IntegrationTests.Automation.Artifact
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
            [TestMethod]
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
            [TestMethod]
            public void WhenOnArtifactActivationIsSelect_ThenUpdatedCommandSettings()
            {
                this.artifactExtension.OnArtifactActivation = ArtifactActivatedAction.Select;

                AssertSelectCommand(true);
                AssertSelectContextMenu(true);
                AssertActivateEvent(true, AssertSelectCommand(false));
            }

            [HostType("VS IDE")]
            [TestMethod]
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
            [TestMethod]
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
            [TestMethod]
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
                        .GetValue(commandSettings) as Microsoft.VisualStudio.Patterning.Library.Automation.DesignProperty;
                    Assert.Equal(true, openProperty.Value);
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
                        .GetValue(commandSettings) as Microsoft.VisualStudio.Patterning.Library.Automation.DesignProperty;
                    Assert.Equal(false, openProperty.Value);
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
        }
    }
}
