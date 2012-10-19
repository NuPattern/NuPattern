using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.VisualStudio.Patterning.Extensibility.References;
using System;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Commands
{
	[TestClass]
	public class ActivateFeatureCommandSpec
	{
		//// internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenAnInstantiatedFeature
		{
			private Mock<IProductElement> mockOwner;
			private Mock<IFeatureManager> mockManager = null;
			private ActivateFeatureCommand command;

			[TestInitialize]
			public void Initialize()
			{
				this.mockOwner = new Mock<IProductElement>();
				this.mockManager = new Mock<IFeatureManager>();
				var testInstance = new Mock<IFeatureExtension>();

				testInstance.SetupGet(reg => reg.FeatureId).Returns("FeatureId");
				testInstance.SetupGet(reg => reg.InstanceName).Returns("InstanceName");

				this.mockManager.SetupGet(m => m.InstantiatedFeatures).Returns(new IFeatureExtension[] { testInstance.Object });

				this.command = new ActivateFeatureCommand();
				this.command.FeatureManager = this.mockManager.Object;
				this.command.CurrentElement = mockOwner.Object;
                this.command.ServiceProvider = new Mock<IServiceProvider>().Object;
			}

			[TestMethod]
			public void WhenNoReference_ThenDoesNotActivateFeature()
			{
				this.command.Execute();

				this.mockManager.VerifySet(fm => fm.ActiveFeature = It.IsAny<IFeatureExtension>(), Times.Never());
			}

			[TestMethod]
			public void WhenGuidanceReferenceNotExistInSolution_ThenDoesNotActivateFeature()
			{
				var mockRef = new Mock<IReference>();
				mockRef.Setup(r => r.Kind).Returns(ReferenceKindConstants.Guidance);
				mockRef.Setup(r => r.Value).Returns("Foo");
				this.mockOwner.Setup(o => o.References).Returns(new[] { mockRef.Object });

				this.command.Execute();

				this.mockManager.VerifySet(fm => fm.ActiveFeature = It.IsAny<IFeatureExtension>(), Times.Never());
			}

			[TestMethod]
			public void WhenGuidanceReferenceExistsInSolution_ThenActivatesFeature()
			{
				var mockRef = new Mock<IReference>();
				mockRef.Setup(r => r.Kind).Returns(ReferenceKindConstants.Guidance);
				mockRef.Setup(r => r.Value).Returns("Foo");
				this.mockOwner.Setup(o => o.References).Returns(new[] { mockRef.Object });
				Mock<IFeatureExtension> mockFeature = new Mock<IFeatureExtension>();
				mockFeature.Setup(f => f.InstanceName).Returns("Foo");
				this.mockManager.Setup(m => m.InstantiatedFeatures).Returns(new[] { mockFeature.Object });

				this.command.Execute();

				this.mockManager.VerifySet(fm => fm.ActiveFeature = mockFeature.Object, Times.Once());
			}
		}
	}
}