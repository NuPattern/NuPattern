using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using DataAnnotations = System.ComponentModel.DataAnnotations;

namespace Microsoft.VisualStudio.Patterning.Authoring.Guidance.IntegrationTests
{
	[TestClass]
	public class TocGuidanceProcessorSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		//[Ignore] // Cant be executed in Team Build. Word cannot be automated in non-interractive mode.
		[DeploymentItem("SampleGuidanceDocs", "SampleGuidanceDocs")]
		[TestClass]
		public class GivenADocumentWorkflow : IntegrationTest
		{
			private TocGuidanceProcessor processor;

			[TestCleanup]
			public void Cleanup()
			{
				VsIdeTestHostContext.Dte.Solution.Close();
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentEmpty_ThenCalculateWorkflowThrows()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\EmptyDocument.doc"), string.Empty, string.Empty);
				Assert.Throws<InvalidOperationException>(() =>
					this.processor.CalculateWorkflow());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasNoTitle_ThenCalculateWorkflowThrows()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\NotStartingWithTitle.doc"), string.Empty, string.Empty);
				Assert.Throws<InvalidOperationException>(() =>
					this.processor.CalculateWorkflow());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasOnlyTitle_ThenCalculateWorkflowCreatesAWorkflow()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\JustTitleNoPages.doc"), string.Empty, string.Empty);

				VerifyQueue(this.processor.CalculateWorkflow(), new[]
					{
						typeof(Initial),
						typeof(GuidanceAction),
						typeof(Final),
					});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasSingleHeadline_ThenCalculateWorkflowCreatesAWorkflow()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\SingleHeadlineSingleLine.doc"), string.Empty, string.Empty);

				VerifyQueue(this.processor.CalculateWorkflow(), new[]
					{
						typeof(Initial),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Final),
					});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasMultipleTopLevelHeadlines_ThenCalculateWorkflowCreatesAWorkflow()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\MultipleTopLevelHeadlines.doc"), string.Empty, string.Empty);

				VerifyQueue(this.processor.CalculateWorkflow(), new[]
					{
						typeof(Initial),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Final),
					});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasTopLevelHeadlineTwoTopics_ThenCalculateWorkflowCreatesAWorkflow()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\TopLevelHeadlineTwoTopics.doc"), string.Empty, string.Empty);

				VerifyQueue(this.processor.CalculateWorkflow(), new[]
					{
						typeof(Initial),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Final),
					});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasThreeTopLevelHeadlineVariousTopics_ThenCalculateWorkflowCreatesAWorkflow()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\ThreeTopLevelHeadlineVariousTopics.doc"), string.Empty, string.Empty);

				VerifyQueue(this.processor.CalculateWorkflow(), new[]
					{
						typeof(Initial),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Final),
					});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasTopLevelHeadlineTopLevelTopic_ThenCalculateWorkflowCreatesAWorkflow()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\TopLevelHeadlineTopLevelTopic.doc"), string.Empty, string.Empty);

				VerifyQueue(this.processor.CalculateWorkflow(), new[]
					{
						typeof(Initial),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(GuidanceAction),
						typeof(Final),
					});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasTopLevelHeadlineTopLevelTopicWithSubTopics_ThenCalculateWorkflowCreatesAWorkflow()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\TopLevelHeadlineTopLevelTopicWithSubTopics.doc"), string.Empty, string.Empty);

				VerifyQueue(this.processor.CalculateWorkflow(), new[]
					{
						typeof(Initial),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Final),
					});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasTopLevelHeadlineWithSubTopics_ThenCalculateWorkflowCreatesAWorkflow()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\TopLevelHeadlineWithSubTopics.doc"), string.Empty, string.Empty);

				VerifyQueue(this.processor.CalculateWorkflow(), new[]
					{
						typeof(Initial),
						typeof(Fork),
						typeof(Fork),
						typeof(GuidanceAction),
						typeof(Join),
						typeof(Join),
						typeof(Final),
					});
			}
		}

		//[Ignore] // Cant be executed in Team Build. Word cannot be automated in non-interractive mode.
		[DeploymentItem("SampleGuidanceDocs", "SampleGuidanceDocs")]
		[TestClass]
		public class GivenADocumentToShred : IntegrationTest
		{
			private TocGuidanceProcessor processor;
			private string shredDirectory;

			[TestInitialize]
			public void InitializeContext()
			{
				this.shredDirectory = Path.Combine(this.DeploymentDirectory, Path.GetRandomFileName());
			}

			[TestCleanup]
			public void Cleanup()
			{
				try
				{
					Directory.Delete(this.shredDirectory);
				}
				catch (IOException)
				{
					// Ignore cleanup
				}
				VsIdeTestHostContext.Dte.Solution.Close();
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentEmpty_ThenShredDocumentsThrows()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\EmptyDocument.doc"), string.Empty, string.Empty);
				Assert.Throws<InvalidOperationException>(() =>
					this.processor.GenerateWorkflowDocuments(this.shredDirectory));
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasNoTitle_ThenShredDocumentsThrows()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\NotStartingWithTitle.doc"), string.Empty, string.Empty);
				Assert.Throws<InvalidOperationException>(() =>
					this.processor.GenerateWorkflowDocuments(this.shredDirectory));
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasOnlyTitle_ThenShredDocumentsCreatesDocuments()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\JustTitleNoPages.doc"), string.Empty, string.Empty);

				VerifyDocuments(this.processor.GenerateWorkflowDocuments(this.shredDirectory), this.shredDirectory, null);
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasSingleHeadlineNoContent_ThenShredDocumentsCreatesDocuments()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\SingleHeadlineNoContent.doc"), string.Empty, string.Empty);

				VerifyDocuments(this.processor.GenerateWorkflowDocuments(this.shredDirectory), this.shredDirectory, null);
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasSingleHeadline_ThenShredDocumentsCreatesDocuments()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\SingleHeadlineSingleLine.doc"), string.Empty, string.Empty);

				VerifyDocuments(this.processor.GenerateWorkflowDocuments(this.shredDirectory), this.shredDirectory, null);
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasSingleTopic_ThenShredDocumentsCreatesDocuments()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\SingleTopic.doc"), string.Empty, string.Empty);

				VerifyDocuments(this.processor.GenerateWorkflowDocuments(this.shredDirectory), this.shredDirectory, new[]
				{
					"FirstTopic.MHT",
				});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasMultipleTopLevelHeadlines_ThenShredDocumentsCreatesDocuments()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\MultipleTopLevelHeadlines.doc"), string.Empty, string.Empty);

				VerifyDocuments(this.processor.GenerateWorkflowDocuments(this.shredDirectory), this.shredDirectory, null);
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasMultipleTopLevelTopics_ThenShredDocumentsCreatesDocuments()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\MultipleTopLevelTopics.doc"), string.Empty, string.Empty);

				VerifyDocuments(this.processor.GenerateWorkflowDocuments(this.shredDirectory), this.shredDirectory, new[]
				{
					"FirstTopic.MHT",
					"SecondTopic.MHT",
					"ThirdTopic.MHT",
				});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasMultipleHeadlinesAndTopics_ThenShredDocumentsCreatesDocuments()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\MultipleHeadlinesAndTopics.doc"), string.Empty, string.Empty);

				VerifyDocuments(this.processor.GenerateWorkflowDocuments(this.shredDirectory), this.shredDirectory, new[]
				{
					"SecondTopic.MHT",
					"ThirdTopic.MHT",
					"FifthTopic.MHT",
				});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasTwoTopicsWithHyperlinksToThirdTopic_ThenShredDocumentsCreatesDocuments()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\TwoTopicsWithHyperlinksToThirdTopic.doc"), "FooFeatureId", "Assets/Guidance/Content");

				VerifyDocuments(this.processor.GenerateWorkflowDocuments(this.shredDirectory), this.shredDirectory, new[]
				{
					"FirstTopic.MHT",
					"SecondTopic.MHT",
					"ThirdTopic.MHT",
				});
				VerifyDocumentContent(this.shredDirectory, "FirstTopic.MHT", new[] 
				{ 
					"content://FooFeatureId/Assets/Guidance/Content/SecondTopic.MHT" 
				});
				VerifyDocumentContent(this.shredDirectory, "ThirdTopic.MHT", new[] 
				{ 
					"content://FooFeatureId/Assets/Guidance/Content/SecondTopic.MHT" 
				});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasOneHeadlineWithHyperlinkToTopic_ThenShredDocumentsCreatesDocuments()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\OneHeadlineWithHyperlinkToTopic.doc"), "FooFeatureId", "Assets/Guidance/Content");

				VerifyDocuments(this.processor.GenerateWorkflowDocuments(this.shredDirectory), this.shredDirectory, new[]
				{
					"SecondTopic.MHT",
				});
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasMultipleTopicsAndHeadlinesWithDuplicates_ThenShredDocumentsThrows()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\MultipleTopicsAndHeadlinesWithDuplicates.doc"), "FooFeatureId", "Assets/Guidance/Content");

				Assert.Throws<InvalidOperationException>(() =>
					this.processor.GenerateWorkflowDocuments(this.shredDirectory));
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasTwoTopicsWithHyperlinkToContent_ThenShredDocumentsThrows()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\TwoTopicsWithHyperlinkToContent.doc"), "FooFeatureId", "Assets/Guidance/Content");

				Assert.Throws<InvalidOperationException>(() =>
					this.processor.GenerateWorkflowDocuments(this.shredDirectory));
			}
		}

		//[Ignore] // Cant be executed in Team Build. Word cannot be automated in non-interractive mode.
		[DeploymentItem("SampleGuidanceDocs", "SampleGuidanceDocs")]
		[TestClass]
		public class GivenADocumentToValidate : IntegrationTest
		{
			private TocGuidanceProcessor processor;

			[TestInitialize]
			public void InitializeContext()
			{
			}

			[TestCleanup]
			public void Cleanup()
			{
				VsIdeTestHostContext.Dte.Solution.Close();
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentEmpty_ThenValidateDocumentsFails()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\EmptyDocument.doc"), string.Empty, string.Empty);
				VerifyValidationErrors(this.processor.ValidateDocument(), 1);
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasNoTitle_ThenValidateDocumentsFails()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\NotStartingWithTitle.doc"), string.Empty, string.Empty);
				VerifyValidationErrors(this.processor.ValidateDocument(), 1);
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasOnlyTitle_ThenValidateDocumentsSucceeds()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\JustTitleNoPages.doc"), string.Empty, string.Empty);

				VerifyValidationErrors(this.processor.ValidateDocument());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasSingleHeadlineNoContent_ThenValidateDocumentsSucceeds()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\SingleHeadlineNoContent.doc"), string.Empty, string.Empty);

				VerifyValidationErrors(this.processor.ValidateDocument());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasSingleHeadline_ThenValidateDocumentsSucceeds()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\SingleHeadlineSingleLine.doc"), string.Empty, string.Empty);

				VerifyValidationErrors(this.processor.ValidateDocument());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasSingleTopic_ThenValidateDocumentsSucceeds()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\SingleTopic.doc"), string.Empty, string.Empty);

				VerifyValidationErrors(this.processor.ValidateDocument());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasMultipleTopLevelHeadlines_ThenValidateDocumentsSucceeds()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\MultipleTopLevelHeadlines.doc"), string.Empty, string.Empty);

				VerifyValidationErrors(this.processor.ValidateDocument());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasMultipleTopLevelTopics_ThenValidateDocumentsSucceeds()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\MultipleTopLevelTopics.doc"), string.Empty, string.Empty);

				VerifyValidationErrors(this.processor.ValidateDocument());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasMultipleHeadlinesAndTopics_ThenValidateDocumentsSucceeds()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\MultipleHeadlinesAndTopics.doc"), string.Empty, string.Empty);

				VerifyValidationErrors(this.processor.ValidateDocument());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasMultipleTopicsAndHeadlinesWithDuplicates_ThenValidateDocumentsFails()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\MultipleTopicsAndHeadlinesWithDuplicates.doc"), "FooFeatureId", "Assets/Guidance/Content");

				VerifyValidationErrors(this.processor.ValidateDocument(), 1);
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasTwoTopicsWithHyperlinksToThirdTopic_ThenValidateDocumentsSucceeds()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\TwoTopicsWithHyperlinksToThirdTopic.doc"), "FooFeatureId", "Assets/Guidance/Content");

				VerifyValidationErrors(this.processor.ValidateDocument());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasOneHeadlineWithHyperlinkToTopic_ThenValidateDocumentsSucceeds()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\OneHeadlineWithHyperlinkToTopic.doc"), "FooFeatureId", "Assets/Guidance/Content");

				VerifyValidationErrors(this.processor.ValidateDocument());
			}

			[HostType("VS IDE")]
			[TestMethod, TestCategory("Integration")]
			public void WhenDocumentHasTwoTopicsWithHyperlinkToContent_ThenValidateDocumentsFails()
			{
				this.processor = new TocGuidanceProcessor(
					PathTo("SampleGuidanceDocs\\TwoTopicsWithHyperlinkToContent.doc"), "FooFeatureId", "Assets/Guidance/Content");

				VerifyValidationErrors(this.processor.ValidateDocument(), 1);
			}
		}

		internal static void VerifyDocumentContent(string directoryPath, string fileName, string[] matchExpressions)
		{
			string fileText = File.ReadAllText(Path.Combine(directoryPath, fileName));
			foreach (var match in matchExpressions)
			{
				var regex = new Regex(match, RegexOptions.IgnoreCase);
				Assert.True(regex.IsMatch(fileText));
			}
		}

		internal static void VerifyQueue(Queue<INode> queue, Type[] nodeTypes)
		{
			foreach (var nodeType in nodeTypes)
			{
				var node = queue.Dequeue();
				Assert.True(node.GetType() == nodeType);
			}
		}

		internal static void VerifyDocuments(IEnumerable<string> documents, string directoryPath, string[] filenames)
		{
			if (filenames == null)
			{
				Assert.True(Directory.GetFiles(directoryPath).GetLength(0) == 0);
			}
			else
			{
				//Ensure correct number of files
				var filesOnDiskCount = Directory.GetFiles(directoryPath).GetLength(0);
				var fileNameCount = filenames.GetLength(0);
				Assert.True(filesOnDiskCount == fileNameCount,
					"Number of files found on disk ({0}) does not equal the number of expected files on disk ({1})", filesOnDiskCount, fileNameCount);

				foreach (var filename in filenames)
				{
					// Ensure it exists in collection
					Assert.NotNull(documents.FirstOrDefault(d => d.Equals(filename, StringComparison.OrdinalIgnoreCase)),
						"Expected file ({0}) not found in collection.", filename);

					//Ensure it exists on disk
					var filePath = Path.Combine(directoryPath, filename);
					Assert.True(File.Exists(filePath),
						"Expected file ({0}) not found on disk", filePath);
				}
			}
		}

		internal static void VerifyValidationErrors(IEnumerable<DataAnnotations.ValidationResult> errors, int results = 0)
		{
			Assert.True(errors.Count() == results,
				"Number of validation errors ({0}) does not equal the number of expected validation errors ({1})", errors.Count(), results);
		}
	}
}
