using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Automation;
using NuPattern.Library.Commands;
using NuPattern.Library.Events;
using NuPattern.Runtime;

namespace NuPattern.Library.UnitTests
{
	[TestClass]
	public class TemplateAutomationSpec
	{
		//// internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
		public class GivenATemplateSettings
		{
			private static readonly Guid CommandId = new Guid("59196AE3-9A05-4A2E-8483-42FADAA71557");
			private static readonly Guid WizardId = new Guid("59196AE3-9A05-4A2E-8483-42FADAA71557");

			private Mock<IProductElement> owner;
			private Mock<ITemplateSettings> settings;
			private Mock<IAutomationExtension> command;
			private Mock<IWizardAutomationExtension> wizard;
			private Mock<IOnElementInstantiatedEvent> onInstantiated;
			private Mock<IFxrUriReferenceService> uriService;
			private Mock<IVsTemplate> assetVsTemplate;
			private Mock<ITemplate> assetUnfoldTemplate;
			private ReferenceTag tag;
			private Solution solution;

			private TemplateAutomation automation;

			[TestInitialize]
			public void Initialize()
			{
				this.settings = Mock.Get(Mock.Of<ITemplateSettings>(s =>
					s.Owner.Name == "ElementWithTemplateAutomation" &&
					s.CommandId == CommandId &&
					s.WizardId == WizardId &&
					s.TemplateUri == "template://item/CSharp/DataContract" &&
					s.TargetFileName == Mock.Of<IPropertyBindingSettings>(binding => binding.Value == "MyItem.cs") &&
					s.TargetPath == Mock.Of<IPropertyBindingSettings>(binding => binding.Value == "")
				));
				var commandSettings = Mock.Of<IAutomationSettingsSchema>(s =>
					s.Id == CommandId &&
					s.Name == "Command" &&
					s.As<ICommandSettings>() == Mock.Of<ICommandSettings>(c =>
						c.Id == CommandId &&
						c.Name == "Command")
				);
				var commandInfo = Mock.Of<IAutomationSettingsInfo>(i =>
					i.Id == commandSettings.Id &&
					i.Name == commandSettings.Name &&
					i.As<IAutomationSettings>() == Mock.Of<IAutomationSettings>(s =>
						s.Id == commandSettings.Id &&
						s.Name == commandSettings.Name)
				);
				var wizardSettings = Mock.Of<IAutomationSettingsSchema>(s =>
					s.Id == WizardId &&
					s.Name == "Wizard" &&
					s.As<IWizardSettings>() == Mock.Of<IWizardSettings>(c =>
						c.Id == WizardId &&
						c.Name == "Wizard")
				);
				var wizardInfo = Mock.Of<IAutomationSettingsInfo>(i =>
					i.Id == wizardSettings.Id &&
					i.Name == wizardSettings.Name &&
					i.As<IAutomationSettings>() == Mock.Of<IAutomationSettings>(s =>
						s.Id == wizardSettings.Id &&
						s.Name == wizardSettings.Name)
				);

				this.settings.Setup(x => x.Owner.AutomationSettings).Returns(new[] { commandSettings, wizardSettings });

				this.command = Mock.Get(Mock.Of<IAutomationExtension>(c => c.Name == "Command"));
				this.wizard = Mock.Get(Mock.Of<IWizardAutomationExtension>(w => w.Name == "Wizard" && w.IsCanceled == false));

				this.owner = Mock.Get(Mock.Of<IProductElement>(o =>
					o.BeginTransaction() == Mock.Of<NuPattern.Runtime.ITransaction>() &&
					o.InstanceName == "ElementWithTemplateAutomation" &&
					o.Info.AutomationSettings == new[] { commandInfo, wizardInfo } &&
					o.AutomationExtensions == new[] { this.command.Object, this.wizard.Object }
				));

				this.owner.DefaultValue = DefaultValue.Mock;

				this.onInstantiated = new Mock<IOnElementInstantiatedEvent>();

				this.automation = new TemplateAutomation(this.owner.Object, this.settings.Object);
				this.automation.OnInstantiated = this.onInstantiated.Object;

				this.solution = new Solution
				{
					PhysicalPath = Path.GetTempFileName(),
					Items =
					{
						new SolutionFolder 
						{
							PhysicalPath = Path.GetTempPath() + "\\Solution Items",
							Name = "Solution Items"
						}, 
						new Project
						{
							PhysicalPath = Path.GetTempPath() + "\\MyProject.csproj",
							Name = "MyProject"
						}
					}
				};

				this.automation.Solution = this.solution;
				this.assetUnfoldTemplate = new Mock<ITemplate> { DefaultValue = DefaultValue.Mock };

				this.uriService = Mock.Get(Mock.Of<IFxrUriReferenceService>(s =>
					s.ResolveUri<ITemplate>(It.IsAny<Uri>()) == this.assetUnfoldTemplate.Object &&
					s.CreateUri(It.IsAny<IItemContainer>(), null) == new Uri("solution://" + Guid.NewGuid().ToString())
				));

				this.assetVsTemplate = new Mock<IVsTemplate> { DefaultValue = DefaultValue.Mock };
				this.assetVsTemplate.Setup(x => x.Type).Returns(VsTemplateType.Item);
				this.uriService.Setup(x => x.ResolveUri<IVsTemplate>(It.IsAny<Uri>()))
					.Returns(this.assetVsTemplate.Object);

				this.automation.UriService = this.uriService.Object;

				this.tag = new ReferenceTag
				{
					SyncNames = this.settings.Object.SyncName,
					TargetFileName = this.settings.Object.TargetFileName.Value,
					Id = this.settings.Object.Id,
				};
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnfoldOnElementCreatedAndEventAvailable_ThenSubscribesToOnInstantiated()
			{
				this.settings.Setup(x => x.UnfoldOnElementCreated).Returns(true);
				this.automation.EndInit();

				this.onInstantiated.Verify(x => x.Subscribe(It.IsAny<IObserver<IEvent<EventArgs>>>()));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnfoldScopeIsActive_ThenDoesNotExecuteCommandDirectlyBecauseTemplateWizardDoes()
			{
				this.automation.EndInit();

				using (new UnfoldScope(this.automation, tag, this.settings.Object.TemplateUri))
				{
					this.automation.Execute();

					this.command.Verify(x => x.Execute(), Times.Never());
				}
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnfoldScopeIsActive_ThenDoesNotExecuteWizardDirectlyBecauseTemplateWizardDoes()
			{
				this.automation.EndInit();

				using (new UnfoldScope(this.automation, tag, this.settings.Object.TemplateUri))
				{
					this.automation.Execute();

					this.wizard.Verify(x => x.Execute(), Times.Never());
				}
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenNoUnfoldScopeIsActiveAndNoUnfoldOnElementCreated_ThenDoesNotExecuteCommand()
			{
				this.settings.Setup(x => x.UnfoldOnElementCreated).Returns(false);
				this.automation.EndInit();

				using (new UnfoldScope(this.automation, tag, this.settings.Object.TemplateUri))
				{
					this.automation.Execute();

					this.command.Verify(x => x.Execute(), Times.Never());
				}
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenSettingsCommandIdCannotBeResolved_ThenNoOps()
			{
				this.settings.Setup(x => x.UnfoldOnElementCreated).Returns(false);
				this.settings.Setup(x => x.CommandId).Returns(Guid.NewGuid());
				this.automation.EndInit();

				using (new UnfoldScope(this.automation, tag, this.settings.Object.TemplateUri))
				{
					this.automation.Execute();

					this.command.Verify(x => x.Execute(), Times.Never());
				}
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnfoldOnElementCreatedAndEventAvailable_ThenRaisingEventUnfoldsTemplate()
			{
				IObserver<IEvent<EventArgs>> observer = null;
				var subscription = new Mock<IDisposable>();
				this.settings.Setup(x => x.UnfoldOnElementCreated).Returns(true);

				this.onInstantiated
					.Setup(x => x.Subscribe(It.IsAny<IObserver<IEvent<EventArgs>>>()))
					.Callback<object>(h => observer = h as IObserver<IEvent<EventArgs>>)
					.Returns(subscription.Object);

				this.automation.EndInit();

				observer.OnNext(Mocks.Of<IEvent<EventArgs>>().First(e => e.Sender == this.owner.Object && e.EventArgs == EventArgs.Empty));

				this.assetUnfoldTemplate.Verify(x => x.Unfold(It.IsAny<string>(), It.IsAny<IItemContainer>()));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenExecutedAndWizardIsCanceled_ThenCommandIsNotExecuted()
			{
				this.wizard.Setup(x => x.IsCanceled).Returns(true);
				this.automation.EndInit();

				this.automation.Execute();

				this.command.Verify(x => x.Execute(), Times.Never());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenExecutedAndWizardIsCanceled_ThenElementIsDeleted()
			{
				this.wizard.Setup(x => x.IsCanceled).Returns(true);
				this.automation.EndInit();

				this.automation.Execute();

				this.owner.Verify(x => x.Delete());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenExecutedAndUnfoldOnElementCreatedIsFalseAndWizardIsNotCanceled_ThenCommandIsExecuted()
			{
				this.wizard.Setup(x => x.IsCanceled).Returns(false);
				this.automation.EndInit();

				this.automation.Execute();

				this.command.Verify(x => x.Execute());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenExecutedAndUnfoldOnElementCreatedIsTrueAndWizardIsNotCanceled_ThenCommandIsExecuted()
			{
				this.wizard.Setup(x => x.IsCanceled).Returns(false);
				this.automation.EndInit();

				this.automation.Execute();

				this.command.Verify(x => x.Execute());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnfoldingProjectTemplateReference_ThenTemplateIsUnfoldedOnSolutionRoot()
			{
				this.settings.Setup(x => x.UnfoldOnElementCreated).Returns(true);
				this.assetVsTemplate.Setup(x => x.Type).Returns(VsTemplateType.Project);
				this.automation.EndInit();

				this.automation.Execute();

				this.assetUnfoldTemplate.Verify(x => x.Unfold(It.IsAny<string>(), this.solution));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnfoldingProjectItemReference_ThenTemplateIsUnfoldedOnSolutionItems()
			{
				this.settings.Setup(x => x.UnfoldOnElementCreated).Returns(true);
				this.assetVsTemplate.Setup(x => x.Type).Returns(VsTemplateType.Item);
				this.automation.EndInit();

				this.automation.Execute();

				this.assetUnfoldTemplate.Verify(x => x.Unfold(It.IsAny<string>(), this.solution.Items.First()));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenExecutingFromTemplateUnfold_ThenTemplateIsNotUnfoldedAgain()
			{
				this.settings.Setup(x => x.UnfoldOnElementCreated).Returns(true);
				this.automation.EndInit();

				using (new UnfoldScope(this.automation, tag, this.settings.Object.TemplateUri))
				{
					this.automation.Execute();

					this.assetUnfoldTemplate.Verify(x => x.Unfold(It.IsAny<string>(), It.IsAny<IItemContainer>()), Times.Never());
				}
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnfolding_ThenEvaluatesFileAndPathPropertyBindings()
			{
				this.settings.Setup(x => x.UnfoldOnElementCreated).Returns(true);
				this.settings
					.Setup(x => x.TargetFileName)
					.Returns(Mock.Of<IPropertyBindingSettings>(binding =>
						binding.Value == "MyItem.cs"));

				this.settings
					.Setup(x => x.TargetPath)
					.Returns(Mock.Of<IPropertyBindingSettings>(binding =>
						binding.Value == "MyProject"));

				this.assetVsTemplate.Setup(x => x.Type).Returns(VsTemplateType.Item);
				this.automation.EndInit();

				this.automation.Execute();

				this.assetUnfoldTemplate.Verify(x => x.Unfold(
					"MyItem.cs", It.Is<IProject>(p => p.Name == "MyProject")));
			}
		}
	}
}
