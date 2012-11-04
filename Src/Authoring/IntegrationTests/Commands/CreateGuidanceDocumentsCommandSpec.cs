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

			//[TestMethod, TestCategory("Integration")]
			//public void WhenExecuteWithNoReferences_ThenThrows()
			//{
			//    Assert.Throws<InvalidOperationException>(() => command.Execute());
			//}

			//[TestMethod, TestCategory("Integration")]
			//public void WhenExecuteWithNoGuidanceDocumentReference_ThenThrows()
			//{
			//    Assert.Throws<InvalidOperationException>(() => command.Execute());
			//}

			//[TestMethod, TestCategory("Integration")]
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

			//[TestMethod, TestCategory("Integration")]
			//public void WhenExecuteAndContentContainerNotExist_ThenCreatesContentContainer()
			//{
			//}

			//[TestMethod, TestCategory("Integration")]
			//public void WhenExecuteContentFileNotIncludedInProject_ThenAddsToProjectContainer()
			//{
			//}

			//[TestMethod, TestCategory("Integration")]
			//public void WhenExecute_ThenAddsGeneratedFilesToProjectContainer()
			//{
			//}

			//[TestMethod, TestCategory("Integration")]
			//public void WhenExecuteAndExcludedFilesOnDisk_ThenAddsIncludesFilesToProjectContainer()
			//{
			//}

			//[TestMethod, TestCategory("Integration")]
			//public void WhenExecuteAndNewFilesGenerated_ThenAddsNewFilesToProjectContainer()
			//{
			//}

			//[TestMethod, TestCategory("Integration")]
			//public void WhenExecuteAndExistingFilesNotGenerated_ThenDeletesFilesFromProjectContainer()
			//{
			//}
		}
	}
}
