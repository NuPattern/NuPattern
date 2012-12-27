using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Extensibility;
using NuPattern.Library.Automation;
using NuPattern.Library.Commands;
using NuPattern.Library.Events;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.IntegrationTests.Automation.Validation
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
    public class ValidationExtensionChangeRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAProduct
        {
            private Store store;

            private IValidationExtension validationExtension;
            private IPatternElementSchema container;

            [TestInitialize]
            public void Initialize()
            {
                PatternSchema product = null;

                this.store = new Store(VsIdeTestHostContext.ServiceProvider,
                    new Type[] { typeof(CoreDesignSurfaceDomainModel), typeof(PatternModelDomainModel), typeof(LibraryDomainModel) });

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    product = patternModel.Create<PatternSchema>();
                });

                this.validationExtension = product.GetExtensions<IValidationExtension>().FirstOrDefault();
                this.container = product as IPatternElementSchema;
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnBuildIsTrue_ThenCreatesAutomationSettings()
            {
                this.validationExtension.ValidationOnBuild = true;

                AssertValidateCommand(true);
                AssertOnBuildEvent(true, AssertValidateCommand(false));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnBuildIsFalse_ThenUpdatesAutomationSettings()
            {
                this.validationExtension.ValidationOnBuild = false;

                Assert.Equal(0, this.container.AutomationSettings.Count());
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnSaveIsTrue_ThenCreatesAutomationSettings()
            {
                this.validationExtension.ValidationOnSave = true;

                AssertValidateCommand(true);
                AssertOnSaveEvent(true, AssertValidateCommand(false));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnSaveIsFalse_ThenUpdatesAutomationSettings()
            {
                this.validationExtension.ValidationOnSave = false;

                Assert.Equal(0, this.container.AutomationSettings.Count());
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnMenuIsTrue_ThenCreatesAutomationSettings()
            {
                this.validationExtension.ValidationOnMenu = true;

                AssertValidateCommand(true);
                AssertOnMenuContextMenu(true);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnMenuIsFalse_ThenUpdatesAutomationSettings()
            {
                this.validationExtension.ValidationOnMenu = false;

                Assert.Equal(0, this.container.AutomationSettings.Count());
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnCustomEventIsNotEmpty_ThenCreatesAutomationSettings()
            {
                this.validationExtension.ValidationOnCustomEvent = "foo";

                AssertValidateCommand(true);
                AssertOnCustomEvent(true, AssertValidateCommand(false));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOnCustomEventIsEmpty_ThenUpdatesAutomationSettings()
            {
                this.validationExtension.ValidationOnCustomEvent = string.Empty;

                Assert.Equal(0, this.container.AutomationSettings.Count());
            }

            private IAutomationSettingsSchema AssertValidateCommand(bool verifySettings)
            {
                var command = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ValidationExtension_ValidateCommandName));
                Assert.NotNull(command);

                var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();
                Assert.NotNull(commandSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, command.IsCustomizable);
                    Assert.True(command.IsSystem);

                    Assert.Equal(commandSettings.TypeId, typeof(ValidateElementCommand).FullName);

                    var descendantsProperty = TypeDescriptor.GetProperties(commandSettings)[Reflector<ValidateElementCommand>.GetPropertyName(c => c.ValidateDescendants)]
                        .GetValue(commandSettings) as NuPattern.Library.Automation.DesignProperty;
                    Assert.Equal(true, descendantsProperty.Value);
                }

                return command;
            }

            private IAutomationSettingsSchema AssertOnBuildEvent(bool verifySettings, IAutomationSettingsSchema command)
            {
                var event1 = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ValidationExtension_OnBuildEventName));
                Assert.NotNull(event1);

                var eventSettings = event1.GetExtensions<IEventSettings>().FirstOrDefault();
                Assert.NotNull(eventSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, event1.IsCustomizable);
                    Assert.True(event1.IsSystem);

                    Assert.Equal(eventSettings.EventId, typeof(IOnBuildStartedEvent).FullName);
                    Assert.Equal(eventSettings.FilterForCurrentElement, false);

                    var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();
                    Assert.NotNull(commandSettings);

                    Assert.Equal(eventSettings.CommandId, commandSettings.Id);
                }

                return event1;
            }

            private IAutomationSettingsSchema AssertOnSaveEvent(bool verifySettings, IAutomationSettingsSchema command)
            {
                var event1 = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ValidationExtension_OnSaveEventName));
                Assert.NotNull(event1);

                var eventSettings = event1.GetExtensions<IEventSettings>().FirstOrDefault();
                Assert.NotNull(eventSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, event1.IsCustomizable);
                    Assert.True(event1.IsSystem);

                    Assert.Equal(eventSettings.EventId, typeof(IOnProductStoreSavedEvent).FullName);
                    Assert.Equal(eventSettings.FilterForCurrentElement, false);

                    var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();
                    Assert.NotNull(commandSettings);

                    Assert.Equal(eventSettings.CommandId, commandSettings.Id);
                }

                return event1;
            }

            private IAutomationSettingsSchema AssertOnCustomEvent(bool verifySettings, IAutomationSettingsSchema command)
            {
                var event1 = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ValidationExtension_OnCustomEventName));
                Assert.NotNull(event1);

                var eventSettings = event1.GetExtensions<IEventSettings>().FirstOrDefault();
                Assert.NotNull(eventSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, event1.IsCustomizable);
                    Assert.True(event1.IsSystem);

                    Assert.Equal(eventSettings.EventId, this.validationExtension.ValidationOnCustomEvent);
                    Assert.Equal(eventSettings.FilterForCurrentElement, false);

                    var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();
                    Assert.NotNull(commandSettings);

                    Assert.Equal(eventSettings.CommandId, commandSettings.Id);
                }

                return event1;
            }

            private IAutomationSettingsSchema AssertOnMenuContextMenu(bool verifySettings)
            {
                var menu = this.container.AutomationSettings.FirstOrDefault(set => set.Name.Equals(Properties.Resources.ValidationExtension_OnMenuContextMenuName));
                Assert.NotNull(menu);

                var menuSettings = menu.GetExtensions<MenuSettings>().FirstOrDefault();
                Assert.NotNull(menuSettings);

                if (verifySettings)
                {
                    Assert.Equal(CustomizationState.False, menu.IsCustomizable);
                    Assert.True(menu.IsSystem);

                    var command = AssertValidateCommand(false);
                    var commandSettings = command.GetExtensions<ICommandSettings>().FirstOrDefault();

                    Assert.Equal(menuSettings.Text, Resources.ValidationExtension_OnMenuMenuItemText);
                    Assert.Equal(menuSettings.CommandId, commandSettings.Id);
                    Assert.Equal(menuSettings.WizardId, Guid.Empty);
                }

                return menu;
            }
        }
    }
}
