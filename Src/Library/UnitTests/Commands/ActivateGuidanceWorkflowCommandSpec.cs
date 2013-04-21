using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Commands;
using NuPattern.Runtime;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.References;

namespace NuPattern.Library.UnitTests.Commands
{
    [TestClass]
    public class ActivateGuidanceWorkflowCommandSpec
    {
        //// internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAnInstantiatedGuidanceExtension
        {
            private Mock<IProductElement> mockOwner;
            private Mock<IGuidanceManager> mockManager = null;
            private ActivateGuidanceWorkflowCommand command;

            [TestInitialize]
            public void Initialize()
            {
                this.mockOwner = new Mock<IProductElement>();
                this.mockManager = new Mock<IGuidanceManager>();
                var testInstance = new Mock<IGuidanceExtension>();

                testInstance.SetupGet(reg => reg.ExtensionId).Returns("AnId");
                testInstance.SetupGet(reg => reg.InstanceName).Returns("InstanceName");

                this.mockManager.SetupGet(m => m.InstantiatedGuidanceExtensions).Returns(new IGuidanceExtension[] { testInstance.Object });

                this.command = new ActivateGuidanceWorkflowCommand();
                this.command.GuidanceManager = this.mockManager.Object;
                this.command.CurrentElement = mockOwner.Object;
                this.command.ServiceProvider = new Mock<IServiceProvider>().Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoReference_ThenDoesNotActivateGuidanceWorkflow()
            {
                this.command.Execute();

                this.mockManager.VerifySet(fm => fm.ActiveGuidanceExtension = It.IsAny<IGuidanceExtension>(), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGuidanceReferenceNotExistInSolution_ThenDoesNotActivateGuidanceWorkflow()
            {
                var mockRef = new Mock<IReference>();
                mockRef.Setup(r => r.Kind).Returns(ReferenceKindConstants.Guidance);
                mockRef.Setup(r => r.Value).Returns("Foo");
                this.mockOwner.Setup(o => o.References).Returns(new[] { mockRef.Object });

                this.command.Execute();

                this.mockManager.VerifySet(fm => fm.ActiveGuidanceExtension = It.IsAny<IGuidanceExtension>(), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGuidanceReferenceExistsInSolution_ThenActivatesGuidanceWorkflow()
            {
                var mockRef = new Mock<IReference>();
                mockRef.Setup(r => r.Kind).Returns(ReferenceKindConstants.Guidance);
                mockRef.Setup(r => r.Value).Returns("Foo");
                this.mockOwner.Setup(o => o.References).Returns(new[] { mockRef.Object });
                Mock<IGuidanceExtension> mockExtension = new Mock<IGuidanceExtension>();
                mockExtension.Setup(f => f.InstanceName).Returns("Foo");
                this.mockManager.Setup(m => m.InstantiatedGuidanceExtensions).Returns(new[] { mockExtension.Object });

                this.command.Execute();

                this.mockManager.VerifySet(fm => fm.ActiveGuidanceExtension = mockExtension.Object, Times.Once());
            }
        }
    }
}