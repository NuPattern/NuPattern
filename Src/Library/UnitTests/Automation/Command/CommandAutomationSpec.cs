using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Automation.Command
{
	public class CommandAutomationSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenNoContext
		{
			[TestMethod]
			[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test Code")]
			public void WhenCreatingNewWithNullOwner_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => new CommandAutomation(null, new Mock<ICommandSettings>().Object));
			}

			[TestMethod]
			[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test Code")]
			public void WhenCreatingNewWithNullSettings_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => new CommandAutomation(new Mock<IProductElement>().Object, null));
			}
		}

		[TestClass]
		[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Code")]
		public class GivenACommandAutomation
		{
			private CommandAutomation commandAutomation;
			private IFeatureCommand featureCommand;
			private Mock<IDynamicBinding<IFeatureCommand>> binding;
			private Mock<IDynamicBindingContext> dynamicContext;

			[TestInitialize]
			public void Initialize()
			{
				this.commandAutomation = new CommandAutomation(
					new Mock<IProductElement>().Object,
					Mocks.Of<ICommandSettings>().First(s => s.Owner.Name == "Foo"));

				var factory = new Mock<IBindingFactory>();

				this.featureCommand = new Mock<IFeatureCommand>().Object;

				this.binding = new Mock<IDynamicBinding<IFeatureCommand>>();
				this.binding.Setup(b => b.Value).Returns(this.featureCommand);

				factory.Setup(f => f.CreateBinding<IFeatureCommand>(It.IsAny<IBindingSettings>())).Returns(this.binding.Object);

				this.dynamicContext = new Mock<IDynamicBindingContext> { DefaultValue = DefaultValue.Mock };
				this.binding.Setup(x => x.CreateDynamicContext()).Returns(this.dynamicContext.Object);

				this.commandAutomation.BindingFactory = factory.Object;

				this.commandAutomation.EndInit();
			}

			[TestMethod]
			public void WhenInitialized_ThenSetsCommandBinding()
			{
				Assert.NotNull(this.commandAutomation.CommandBinding);
			}

			[TestMethod]
			public void WhenExecutingCommandWithValidBindings_ThenExecutes()
			{
				this.binding.Setup(b => b.Evaluate(this.dynamicContext.Object)).Returns(true);
				this.commandAutomation.Execute();

				Mock.Get(this.featureCommand).Verify(f => f.Execute(), Times.Once());
			}

			[TestMethod]
			public void WhenExecutingCommandWithInvalidBindings_ThenDoesNothing()
			{
				this.binding.Setup(b => b.Evaluate(this.dynamicContext.Object)).Returns(false);
				this.commandAutomation.Execute();

				Mock.Get(this.featureCommand).Verify(f => f.Execute(), Times.Never());
			}

			[TestMethod]
			public void WhenExecutingCommand_ThenAutomationExtensionIsAvailableToBinding()
			{
				this.commandAutomation.Execute();

				this.dynamicContext.Verify(x => x.AddExport(It.IsAny<IAutomationExtension>()));
			}

			[TestMethod]
			public void WhenExecutingCommand_ThenAutomationExtensionOwnerElementIsAvailableToBinding()
			{
				this.commandAutomation.Execute();

				this.dynamicContext.Verify(x => x.AddExport(It.IsAny<IAutomationExtension>()));
			}

			[TestMethod]
			public void WhenExecutingCommand_ThenAutomationExtensionOwnerElementIsAvailableToBindingAsAutomationContainer()
			{
				this.commandAutomation.Execute();

				this.dynamicContext.Verify(x => x.AddExport(It.IsAny<IAutomationExtension>()));
			}
		}
	}
}