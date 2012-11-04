using System;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility.References;
using Microsoft.VisualStudio.Patterning.Library.ValidationRules;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests
{
	[TestClass]
	public class ArtifactsSavedValidationRuleSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenNoArtifacts
		{
			private ArtifactsSavedValidationRule rule;

			[TestInitialize]
			public void InitializeContext()
			{
				Mock<IFxrUriReferenceService> mockUriReferenceService = new Mock<IFxrUriReferenceService>();
				mockUriReferenceService.Setup(uriReferenceService => uriReferenceService.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
					.Returns((IItemContainer)null);

				this.rule = new ArtifactsSavedValidationRule();
				this.rule.UriReferenceService = mockUriReferenceService.Object;
				this.rule.CurrentElement = new Mock<IProductElement>().Object;
			}

			[TestMethod]
			public void ThenNoValidationError()
			{
				var result = this.rule.Validate();

				Assert.False(result.Any());
			}
		}

		[TestClass]
		public class GivenSingleArtifact
		{
			private ArtifactsSavedValidationRule rule;
			private Mock<EnvDTE.ProjectItem> mockProjectItem;
			private Mock<IFxrUriReferenceService> mockUriReferenceService;

			[TestInitialize]
			public void InitializeContext()
			{
				this.mockProjectItem = new Mock<EnvDTE.ProjectItem>();
				this.mockProjectItem.Setup(pi => pi.Saved).Returns(true);
				var mockItem = new Mock<IItem>();
				mockItem.Setup(i => i.As<EnvDTE.ProjectItem>()).Returns(this.mockProjectItem.Object);
				this.mockUriReferenceService = new Mock<IFxrUriReferenceService>();
				this.mockUriReferenceService.Setup(s => s.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
					.Returns(mockItem.Object);

				var mockElement = new Mock<IProductElement>();
				var mockReference = new Mock<IReference>();
				mockReference.SetupGet(r => r.Kind).Returns(typeof(SolutionArtifactLinkReference).FullName);
				mockReference.SetupGet(r => r.Value).Returns("solution://");
				mockElement.SetupGet(e => e.References).Returns(new[] { mockReference.Object });

				this.rule = new ArtifactsSavedValidationRule();
				this.rule.UriReferenceService = this.mockUriReferenceService.Object;
				this.rule.CurrentElement = mockElement.Object;
			}

			[TestMethod]
			public void WhenArtifactIsNotDirty_ThenNoValidationError()
			{
				var result = this.rule.Validate();

				Assert.False(result.Any());
			}

			[TestMethod]
			public void WhenArtifactIsDirty_ThenValidationError()
			{
				this.mockProjectItem.SetupGet(pi => pi.Saved).Returns(false);
				var result = this.rule.Validate();

				Assert.Equal(1, result.Count());
			}

			[TestMethod]
			public void WhenArtifactIsNotAnItem_ThenNoValidationError()
			{
				this.mockUriReferenceService.Setup(s => s.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(new Mock<ISolution>().Object);
				var result = this.rule.Validate();

				Assert.False(result.Any());
			}
		}
	}
}