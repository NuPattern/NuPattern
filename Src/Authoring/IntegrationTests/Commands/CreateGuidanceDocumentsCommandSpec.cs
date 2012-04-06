using Microsoft.VisualStudio.Patterning.Authoring.Authoring;
using Microsoft.VisualStudio.Patterning.Authoring.Automation.Commands;
using Microsoft.VisualStudio.Patterning.Authoring.Guidance;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Authoring.IntegrationTests
{
	[TestClass]
	public class CreateGuidanceDocumentsCommandSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenNoContext
		{
			private CreateGuidanceDocumentsCommand command;
			private Mock<IGuidanceProcessor> processor;

			[TestInitialize]
			public void InitializeContext()
			{
				this.processor = new Mock<IGuidanceProcessor>();

				this.command = new CreateGuidanceDocumentsCommand(this.processor.Object);
				this.command.Solution = new Mock<ISolution>().Object;
				this.command.UriReferenceService = new Mock<IFxrUriReferenceService>().Object;
				this.command.CurrentElement = new Mock<IGuidance>().Object;
			}

			//[TestMethod]
			//public void WhenExecuteWithNoReferences_ThenThrows()
			//{
			//    Assert.Throws<InvalidOperationException>(() => command.Execute());
			//}

			//[TestMethod]
			//public void WhenExecuteWithNoGuidanceDocumentReference_ThenThrows()
			//{
			//    Assert.Throws<InvalidOperationException>(() => command.Execute());
			//}

			//[TestMethod]
			//public void WhenExecuteAndFailsToCreateContentContainer_ThenThrows()
			//{
			//    Assert.Throws<InvalidOperationException>(() => command.Execute());
			//}
		}

		[TestClass]
		public class GivenADocumentWithContent// : IntegrationTest
		{
			private CreateGuidanceDocumentsCommand command;
			private Mock<IGuidanceProcessor> processor;

			[TestInitialize]
			public void InitializeContext()
			{
				this.processor = new Mock<IGuidanceProcessor>();

				this.command = new CreateGuidanceDocumentsCommand(this.processor.Object);
				this.command.Solution = new Mock<ISolution>().Object;
				this.command.UriReferenceService = new Mock<IFxrUriReferenceService>().Object;
				this.command.CurrentElement = new Mock<IGuidance>().Object;
			}

			//[TestMethod]
			//public void WhenExecuteAndContentContainerNotExist_ThenCreatesContentContainer()
			//{
			//}

			//[TestMethod]
			//public void WhenExecuteContentFileNotIncludedInProject_ThenAddsToProjectContainer()
			//{
			//}

			//[TestMethod]
			//public void WhenExecute_ThenAddsGeneratedFilesToProjectContainer()
			//{
			//}

			//[TestMethod]
			//public void WhenExecuteAndExcludedFilesOnDisk_ThenAddsIncludesFilesToProjectContainer()
			//{
			//}

			//[TestMethod]
			//public void WhenExecuteAndNewFilesGenerated_ThenAddsNewFilesToProjectContainer()
			//{
			//}

			//[TestMethod]
			//public void WhenExecuteAndExistingFilesNotGenerated_ThenDeletesFilesFromProjectContainer()
			//{
			//}
		}
	}
}
