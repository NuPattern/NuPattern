using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Automation;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Design;

namespace NuPattern.Library.UnitTests.Automation.Event
{
    public class EventAutomationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        [TestClass]
        public class GivenAValidConfiguredEvent
        {
            protected Mock<IBindingFactory> bindingFactory;
            protected Mock<IDynamicBinding<ICondition>> conditionBinding;
            protected Mock<IDynamicBindingContext> dynamicContext;

            internal EventAutomation Automation;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "By Design.")]
            [CLSCompliant(false)]
            protected Mock<IProductElement> owner;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "By Design.")]
            internal Mock<IEventSettings> settings;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "By Design.")]
            [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By Design")]
            [CLSCompliant(false)]
            protected Mock<IObservable<IEvent<PropertyChangedEventArgs>>> observable;
            [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "By Design.")]
            [CLSCompliant(false)]
            protected Mock<IAutomationExtension> command;

            [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Test")]
            [TestInitialize]
            public virtual void Initialize()
            {
                Guid CommandId = Guid.NewGuid();

                this.settings = new Mock<IEventSettings>();
                this.settings.SetupAllProperties();
                this.settings.Setup(x => x.CommandId).Returns(CommandId);
                this.settings.Setup(x => x.EventId).Returns("EventId");
                this.settings.Setup(x => x.Owner.Name).Returns("EventAutomation");
                this.settings.Setup(x => x.Owner.AutomationSettings)
                    .Returns(new[]
                    {
                        Mocks.Of<IAutomationSettingsSchema>().First(s => s.Id == CommandId && s.Name == "CommandName"),
                    });

                Mock.Get(this.settings.Object.Owner.AutomationSettings.First())
                    .Setup(x => x.As<ICommandSettings>())
                    .Returns(Mocks.Of<ICommandSettings>().First(x => x.Name == "CommandName"));

                this.command = new Mock<IAutomationExtension>();
                this.command.Setup(x => x.Name).Returns("CommandName");

                var settingsInfo = Mocks.Of<IAutomationSettingsInfo>().First(s => s.Id == CommandId && s.Name == "CommandName");

                Mock.Get(settingsInfo).Setup(i => i.As<IAutomationSettings>()).Returns(
                    Mocks.Of<IAutomationSettings>().First(x => x.Id == CommandId && x.Name == "CommandName"));

                this.owner = new Mock<IProductElement>();
                this.owner.Setup(x => x.Info.AutomationSettings)
                    .Returns(new[]
                    {
                        settingsInfo,
                    });

                this.owner.Setup(x => x.BeginTransaction())
                    .Returns(Mock.Of<ITransaction>());

                this.owner.Setup(x => x.AutomationExtensions)
                    .Returns(new[]
                    {
                        this.command.Object,
                    });

                var ev = new Mock<IObservableEvent>();
                this.observable = ev.As<IObservable<IEvent<PropertyChangedEventArgs>>>();

                this.Automation = new EventAutomation(this.owner.Object, this.settings.Object);
                this.Automation.AllEvents = new[] 
                {
                    new Lazy<IObservableEvent, IIdMetadata>(
                        () => (IObservableEvent)this.observable.Object, 
                        Mocks.Of<IIdMetadata>().First(m => m.Id == "EventId"))
                };


                this.bindingFactory = new Mock<IBindingFactory>();
                this.conditionBinding = new Mock<IDynamicBinding<ICondition>>();

                this.bindingFactory
                    .Setup(x => x.CreateBinding<ICondition>(It.IsAny<IBindingSettings>()))
                    .Returns(this.conditionBinding.Object);

                this.Automation.BindingFactory = this.bindingFactory.Object;

                this.dynamicContext = new Mock<IDynamicBindingContext> { DefaultValue = DefaultValue.Mock };
                this.bindingFactory.Setup(x => x.CreateContext()).Returns(this.dynamicContext.Object);

            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEndInitCalled_ThenSubscribesToEvent()
            {
                this.Automation.EndInit();

                this.observable.Verify(x => x.Subscribe(It.IsAny<IObserver<IEvent<EventArgs>>>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoConditionsAndEventIsRaised_ThenExecutesCommand()
            {
                IObserver<IEvent<EventArgs>> observer = null;

                this.settings
                    .Setup(s => s.ConditionSettings)
                    .Returns(new IBindingSettings[0]);

                this.observable.Setup(x => x.Subscribe(It.IsAny<IObserver<IEvent<EventArgs>>>()))
                    .Callback<object>(h => observer = h as IObserver<IEvent<EventArgs>>)
                    .Returns(new Mock<IDisposable>().Object);

                this.Automation.EndInit();
                observer.OnNext(new Mock<IEvent<EventArgs>>().Object);

                this.command.Verify(x => x.Execute());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisposingAfterInitialized_ThenDisposesSubscription()
            {
                var subscription = new Mock<IDisposable>();

                this.observable.Setup(x => x.Subscribe(It.IsAny<IObserver<IEvent<EventArgs>>>()))
                    .Returns(subscription.Object);

                this.Automation.EndInit();
                this.Automation.Dispose();

                subscription.Verify(x => x.Dispose());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingStandardValuesForCommandReference_ThenRetrievesCommandSetting()
            {
                var context = Mocks.Of<ITypeDescriptorContext>().First(c => c.Instance == this.settings.Object);
                var property =
                    new SettingsReferencePropertyDescriptor<IEventSettings, ICommandSettings>(
                        TypeDescriptor.GetProperties(typeof(IEventSettings))[this.settings.Object.GetPropertyName(x => x.CommandId)],
                        ev => ev.CommandId);

                var converter = property.Converter;

                Assert.True(converter.GetStandardValuesSupported(context));
                Assert.True(converter.GetStandardValuesExclusive(context));

                var values = converter.GetStandardValues(context);

                Assert.Equal(1, values.Count);
                Assert.Equal("CommandName", values.Cast<IAutomationSettings>().First().Name);
            }
        }

        [TestClass]
        public class GivenAnInvalidConditionBinding : GivenAValidConfiguredEvent
        {
            private IObserver<IEvent<EventArgs>> eventObserver;

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.conditionBinding.Setup(x => x.CreateDynamicContext()).Returns(this.dynamicContext.Object);

                // All defaults to true so that base class tests all pass.
                this.conditionBinding.Setup(x => x.Evaluate(It.IsAny<IDynamicBindingContext>())).Returns(false);
                this.conditionBinding.Setup(x => x.Value.Evaluate()).Returns(true);

                this.settings.Setup(s => s.ConditionSettings)
                    .Returns(new[] { new ConditionBindingSettings { TypeId = "Foo" } });

                this.observable.Setup(x => x.Subscribe(It.IsAny<IObserver<IEvent<EventArgs>>>()))
                    .Callback<object>(h => this.eventObserver = h as IObserver<IEvent<EventArgs>>)
                    .Returns(new Mock<IDisposable>().Object);

                this.Automation.EndInit();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConditionBindingIsInvalid_ThenDoesNotInvokeCommand()
            {
                this.eventObserver.OnNext(new Mock<IEvent<EventArgs>>().Object);

                this.command.Verify(x => x.Execute(), Times.Never());
            }
        }

        [TestClass]
        public class GivenAConfiguredCondition : GivenAValidConfiguredEvent
        {
            private IObserver<IEvent<EventArgs>> eventObserver;

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.conditionBinding.Setup(x => x.CreateDynamicContext()).Returns(this.dynamicContext.Object);

                // All defaults to true so that base class tests all pass.
                this.conditionBinding.Setup(x => x.Evaluate(It.IsAny<IDynamicBindingContext>())).Returns(true);
                this.conditionBinding.Setup(x => x.Value.Evaluate()).Returns(true);

                this.settings.Setup(s => s.ConditionSettings)
                    .Returns(new[] { new ConditionBindingSettings { TypeId = "Foo" } });

                this.observable.Setup(x => x.Subscribe(It.IsAny<IObserver<IEvent<EventArgs>>>()))
                    .Callback<object>(h => this.eventObserver = h as IObserver<IEvent<EventArgs>>)
                    .Returns(new Mock<IDisposable>().Object);

                this.Automation.EndInit();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenInvokesBindingFactoryForCondition()
            {
                this.bindingFactory.Verify(x => x.CreateBinding<ICondition>(It.IsAny<IBindingSettings>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecutingAutomation_ThenConditionIsEvaluated()
            {
                this.Automation.Execute();

                this.conditionBinding.Verify(x => x.Evaluate(It.IsAny<IDynamicBindingContext>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventIsRaisedIfBindingValueReturnsFalse_ThenDoesNotInvokeCommand()
            {
                this.conditionBinding.Setup(x => x.Value.Evaluate()).Returns(false);

                this.eventObserver.OnNext(new Mock<IEvent<EventArgs>>().Object);

                this.command.Verify(x => x.Execute(), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventIsRaised_ThenEventIsNotAvailableToBindingAsImport()
            {
                var ev = new Mock<IEvent<EventArgs>>().Object;
                this.eventObserver.OnNext(ev);

                this.dynamicContext.Verify(x => x.AddExport(ev), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventIsRaised_ThenAutomationExtensionIsAvailableToBinding()
            {
                this.eventObserver.OnNext(new Mock<IEvent<EventArgs>>().Object);

                this.dynamicContext.Verify(x => x.AddExport(It.IsAny<IAutomationExtension>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventIsRaised_ThenAutomationExtensionOwnerElementIsAvailableToBinding()
            {
                this.eventObserver.OnNext(new Mock<IEvent<EventArgs>>().Object);

                this.dynamicContext.Verify(x => x.AddExport(It.IsAny<IInstanceBase>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventIsRaised_ThenAutomationExtensionOwnerElementIsAvailableToBindingAsAutomationContainer()
            {
                this.eventObserver.OnNext(new Mock<IEvent<EventArgs>>().Object);

                this.dynamicContext.Verify(x => x.AddExport(It.IsAny<IProductElement>()));
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
        public class GivenAnInvalidCommandId
        {
            private Mock<IProductElement> owner;
            private Mock<IEventSettings> settings;
            private EventAutomation automation;
            private Mock<IBindingFactory> bindingFactory;
            private Mock<IDynamicBinding<ICondition>> conditionBinding;
            private Mock<IDynamicBindingContext> dynamicContext;

            [TestInitialize]
            public void Initialize()
            {
                this.settings = new Mock<IEventSettings>();
                this.settings.SetupAllProperties();
                this.settings.Setup(x => x.CommandId).Returns(Guid.Empty);
                this.settings.Setup(x => x.EventId).Returns("EventId");
                this.settings.Setup(x => x.Owner.Name).Returns("EventAutomation");

                this.owner = new Mock<IProductElement>();
                this.owner.Setup(x => x.Info.AutomationSettings)
                    .Returns(new[]
                    {
                        // Note that this does not match the guid of the CommandId on the launchpoint 
                        // settings, therefore it will not be found.
                        Mocks.Of<IAutomationSettingsInfo>().First(s => s.Id == Guid.NewGuid() && s.Name == "CommandName"),
                    });

                this.automation = new EventAutomation(this.owner.Object, this.settings.Object);

                this.bindingFactory = new Mock<IBindingFactory>();
                this.conditionBinding = new Mock<IDynamicBinding<ICondition>>();

                this.bindingFactory
                    .Setup(x => x.CreateBinding<ICondition>(It.IsAny<IBindingSettings>()))
                    .Returns(this.conditionBinding.Object);

                this.automation.BindingFactory = this.bindingFactory.Object;

                this.dynamicContext = new Mock<IDynamicBindingContext> { DefaultValue = DefaultValue.Mock };
                this.bindingFactory.Setup(x => x.CreateContext()).Returns(this.dynamicContext.Object);

            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCommandIsInvalid_ThenDoesNotFail()
            {
                this.automation.Execute(this.dynamicContext.Object);
            }
        }
    }
}
