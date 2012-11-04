using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Automation.Menu
{
	public class MenuAutomationSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
		public class GivenAMenuAutomationBase
		{
			internal MenuAutomation Automation;

			[CLSCompliant(false)]
			protected Mock<IProductElement> Owner { get; set; }

			[CLSCompliant(false)]
			protected Mock<IMenuSettings> Settings { get; set; }

			public virtual void Initialize()
			{
				this.Settings = new Mock<IMenuSettings>();
				this.Settings.SetupAllProperties();
				this.Settings.Setup(x => x.CommandId).Returns(Guid.Empty);
				this.Settings.Setup(x => x.Text).Returns("MenuText");
				this.Settings.Setup(x => x.Owner.Name).Returns("MenuAutomation");

				this.Owner = new Mock<IProductElement>();

				this.Automation = new MenuAutomation(this.Owner.Object, this.Settings.Object);
			}

			public virtual void Cleanup()
			{
				this.Automation.Dispose();
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
		[TestClass]
		public class GivenAMenuAutomation : GivenAMenuAutomationBase
		{
			[TestInitialize]
			public override void Initialize()
			{
				base.Initialize();
			}

			[TestCleanup]
			public override void Cleanup()
			{
				base.Cleanup();
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenTextChanged_ThenRaisesPropertyChanged()
			{
				var raised = false;
				var propertyName = "";
				this.Automation.PropertyChanged += (sender, args) =>
				{
					propertyName = args.PropertyName;
					raised = true;
				};

				this.Automation.Text = "foo";

				Assert.True(raised);
				Assert.Equal("foo", this.Automation.Text);
				Assert.Equal(Reflector<MenuAutomation>.GetProperty(x => x.Text).Name, propertyName);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenTextChangedToSameValue_ThenDoesNotRaisePropertyChanged()
			{
				var raised = false;
				this.Automation.Text = "foo";
				this.Automation.PropertyChanged += (sender, args) => raised = true;

				this.Automation.Text = "foo";

				Assert.False(raised);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenEnabledChanged_ThenRaisesPropertyChanged()
			{
				var raised = false;
				var propertyName = "";
				this.Automation.PropertyChanged += (sender, args) =>
				{
					propertyName = args.PropertyName;
					raised = true;
				};

				this.Automation.Enabled = !this.Automation.Enabled;

				Assert.True(raised);
				Assert.Equal(Reflector<MenuAutomation>.GetProperty(x => x.Enabled).Name, propertyName);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenVisibleChanged_ThenRaisesPropertyChanged()
			{
				var raised = false;
				var propertyName = "";
				this.Automation.PropertyChanged += (sender, args) =>
				{
					propertyName = args.PropertyName;
					raised = true;
				};

				this.Automation.Visible = !this.Automation.Enabled;

				Assert.True(raised);
				Assert.Equal(Reflector<MenuAutomation>.GetProperty(x => x.Visible).Name, propertyName);
			}
		}

		public class GivenAMenuAutomationWithCommandBase : GivenAMenuAutomationBase
		{
			internal static readonly Guid CommandId = new Guid("C8FE5B04-5F3C-4829-AEBE-BF9CB65AD9B0");

			protected IAutomationSettingsInfo CommandSettingsInfo { get; set; }

			[CLSCompliant(false)]
			protected Mock<IAutomationExtension> Command { get; set; }

			public override void Initialize()
			{
				base.Initialize();

				this.Settings.Setup(x => x.CommandId).Returns(CommandId);
				this.Settings.Setup(x => x.Owner.AutomationSettings)
					.Returns(new[] { Mocks.Of<IAutomationSettingsSchema>().First(s => s.Id == CommandId && s.Name == "CommandName") });

				Mock.Get(this.Settings.Object.Owner.AutomationSettings.First())
					.Setup(x => x.As<ICommandSettings>())
					.Returns(Mocks.Of<ICommandSettings>().First(x => x.Name == "CommandName"));

				this.Command = new Mock<IAutomationExtension>();
				this.Command.Setup(x => x.Name).Returns("CommandName");

				this.CommandSettingsInfo = Mocks.Of<IAutomationSettingsInfo>().First(s => s.Id == CommandId && s.Name == "CommandName");

				Mock.Get(this.CommandSettingsInfo).Setup(i => i.As<IAutomationSettings>()).Returns(
					Mocks.Of<IAutomationSettings>().First(x => x.Id == CommandId && x.Name == "CommandName"));

				this.Owner.Setup(x => x.Info.AutomationSettings).Returns(new[] { this.CommandSettingsInfo });
				this.Owner.Setup(x => x.AutomationExtensions).Returns(new[] { this.Command.Object });
			}
		}

		[TestClass]
		public class GivenAMenuAutomationWithCommand : GivenAMenuAutomationWithCommandBase
		{
			[TestInitialize]
			public override void Initialize()
			{
				base.Initialize();
			}

			[TestCleanup]
			public override void Cleanup()
			{
				base.Cleanup();
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenExecutingAutomation_ThenExecutesCommand()
			{
				this.Automation.EndInit();

				this.Automation.Execute();

				this.Command.Verify(x => x.Execute());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenCommandIdNotFound_ThenDoesNotThrow()
			{
				this.Settings.Setup(x => x.CommandId).Returns(Guid.NewGuid());
				this.Automation.EndInit();

				this.Automation.Execute();

				this.Command.Verify(x => x.Execute(), Times.Never());
			}
		}

		[TestClass]
		public class GivenAMenuAutomationWithWizard : GivenAMenuAutomationWithCommandBase
		{
			private static readonly Guid wizardId = new Guid("9A57D446-DD7A-480A-9ED9-9AB277AC8A39");

			protected Mock<IWizardAutomationExtension> Wizard { get; set; }

			[TestInitialize]
			public override void Initialize()
			{
				base.Initialize();

				this.Owner.Setup(x => x.BeginTransaction()).Returns(new Mock<ITransaction>().Object);

				this.Settings.Setup(x => x.WizardId).Returns(wizardId);
				this.Settings.Setup(x => x.Owner.AutomationSettings)
					.Returns(new[]
					{
						Mocks.Of<IAutomationSettingsSchema>().First(s => s.Id == CommandId && s.Name == "CommandName"),
						Mocks.Of<IAutomationSettingsSchema>().First(s => s.Id == wizardId && s.Name == "WizardName")
					});

				Mock.Get(this.Settings.Object.Owner.AutomationSettings.ElementAt(1))
					.Setup(x => x.As<IWizardSettings>())
					.Returns(Mocks.Of<IWizardSettings>().First(x => x.Name == "WizardName"));

				this.Wizard = new Mock<IWizardAutomationExtension>();
				this.Wizard.Setup(x => x.Name).Returns("WizardName");
				this.Wizard.Setup(x => x.IsCanceled).Returns(false);

				var settingsInfo = Mocks.Of<IAutomationSettingsInfo>().First(s => s.Id == wizardId && s.Name == "WizardName");

				Mock.Get(settingsInfo).Setup(i => i.As<IAutomationSettings>()).Returns(
					Mocks.Of<IAutomationSettings>().First(x => x.Id == wizardId && x.Name == "WizardName"));

				this.Owner.Setup(x => x.Info.AutomationSettings).Returns(new[] { this.CommandSettingsInfo, settingsInfo, });

				this.Owner.Setup(x => x.AutomationExtensions).Returns(new[] { this.Command.Object, this.Wizard.Object });
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenExecutingAutomation_ThenExecutesWizardAndCommand()
			{
				this.Automation.EndInit();

				this.Automation.Execute();

				this.Wizard.Verify(x => x.Execute(), Times.Once());
				this.Command.Verify(x => x.Execute(), Times.Once());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenWizardIdNotFound_ThenDoesNotThrowAndDoesNotExecuteCommand()
			{
				this.Settings.Setup(x => x.WizardId).Returns(Guid.NewGuid());
				this.Automation.EndInit();

				this.Automation.Execute();

				this.Wizard.Verify(x => x.Execute(), Times.Never());
				this.Command.Verify(x => x.Execute(), Times.Never());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenWizardCanceled_ThenDoesNotExecutCommand()
			{
				this.Wizard.Setup(x => x.IsCanceled).Returns(true);
				this.Automation.EndInit();

				this.Automation.Execute();

				this.Wizard.Verify(x => x.Execute(), Times.Once());
				this.Command.Verify(x => x.Execute(), Times.Never());
			}
		}

		[TestClass]
		public class GivenAMenuAutomationWithCustomStatus : GivenAMenuAutomationWithCommandBase
		{
			private Mock<ICommandStatus> status;
			private Mock<IDynamicBinding<ICommandStatus>> statusBinding;
			private Mock<IBindingFactory> bindingFactory;

			[TestInitialize]
			public override void Initialize()
			{
				base.Initialize();

				this.status = new Mock<ICommandStatus>();

				this.bindingFactory = new Mock<IBindingFactory>();
				this.statusBinding = new Mock<IDynamicBinding<ICommandStatus>>
				{
					DefaultValue = DefaultValue.Mock
				};
				this.statusBinding.Setup(b => b.Value).Returns(this.status.Object);
				this.statusBinding.Setup(b => b.Evaluate(It.IsAny<IDynamicBindingContext>())).Returns(true);

				this.bindingFactory.Setup(f => f.CreateBinding<ICommandStatus>(It.IsAny<IBindingSettings>())).Returns(this.statusBinding.Object);
				this.Settings.Setup(x => x.CustomStatus).Returns(BindingSerializer.Serialize(new Extensibility.Binding.BindingSettings { TypeId = "Foo" }));

				this.Automation.BindingFactory = this.bindingFactory.Object;
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenEndInit_ThenInvokesBindingFactoryForCustomQueryStatus()
			{
				this.Automation.EndInit();

				this.bindingFactory.Verify(x => x.CreateBinding<ICommandStatus>(It.IsAny<IBindingSettings>()));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenExecuting_ThenInvokesCustomQueryStatus()
			{
				this.Automation.EndInit();

				this.Automation.Execute();

				this.status.Verify(x => x.QueryStatus(It.IsAny<IMenuCommand>()));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenQueryingStatus_ThenInvokesCustomQueryStatus()
			{
				this.Automation.EndInit();

				this.Automation.QueryStatus(this.Automation);

				this.status.Verify(x => x.QueryStatus(this.Automation));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenQueryStatusSetsEnabledFalse_ThenCommandIsNotInvoked()
			{
				this.status.Setup(x => x.QueryStatus(It.IsAny<IMenuCommand>()))
					.Callback<IMenuCommand>(mc => mc.Enabled = false);
				this.Automation.EndInit();

				this.Automation.Execute();

				this.Command.Verify(x => x.Execute(), Times.Never());
			}
		}

		[TestClass]
		public class GivenAMenuAutomationWithCondition : GivenAMenuAutomationWithCommandBase
		{
			private Mock<IBindingFactory> bindingFactory;
			private Mock<ICondition> condition;
			private Mock<IDynamicBinding<ICondition>> conditionBinding;
			private Mock<IDynamicBindingContext> dynamicContext;

			[TestInitialize]
			public override void Initialize()
			{
				base.Initialize();

				this.bindingFactory = new Mock<IBindingFactory>();
				this.condition = new Mock<ICondition>();
				this.conditionBinding = new Mock<IDynamicBinding<ICondition>>();

				this.bindingFactory
					.Setup(x => x.CreateBinding<ICondition>(It.IsAny<IBindingSettings>()))
					.Returns(this.conditionBinding.Object);

				this.Automation.BindingFactory = this.bindingFactory.Object;

				this.dynamicContext = new Mock<IDynamicBindingContext> { DefaultValue = DefaultValue.Mock };
				this.conditionBinding.Setup(x => x.CreateDynamicContext()).Returns(this.dynamicContext.Object);

				// All defaults to true so that base class tests all pass.
				this.condition.Setup(x => x.Evaluate()).Returns(true);
				this.conditionBinding.Setup(x => x.Evaluate(It.IsAny<IDynamicBindingContext>())).Returns(true);
				this.conditionBinding.Setup(x => x.Value).Returns(this.condition.Object);

				this.Settings.Setup(s => s.ConditionSettings)
					.Returns(new[] { new ConditionBindingSettings { TypeId = "Foo" } });

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
				this.condition.Verify(x => x.Evaluate());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenQueryingStatus_ThenConditionIsEvaluated()
			{
				this.Automation.QueryStatus(this.Automation);

				this.conditionBinding.Verify(x => x.Evaluate(It.IsAny<IDynamicBindingContext>()));
				this.condition.Verify(x => x.Evaluate());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenBindingReturnsFalse_ThenDoesNotInvokeCommand()
			{
				this.conditionBinding.Setup(x => x.Evaluate(It.IsAny<IDynamicBindingContext>())).Returns(false);

				this.Automation.Execute();

				this.Command.Verify(x => x.Execute(), Times.Never());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenBindingValueReturnsFalse_ThenDoesNotInvokeCommand()
			{
				this.conditionBinding.Setup(x => x.Value.Evaluate()).Returns(false);

				this.Automation.Execute();

				this.Command.Verify(x => x.Execute(), Times.Never());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenConditionEvaluatesToFalse_ThenCommandIsNotExecuted()
			{
				this.condition.Setup(x => x.Evaluate()).Returns(false);

				this.Automation.Execute();

				this.Command.Verify(x => x.Execute(), Times.Never());
			}
		}
	}
}