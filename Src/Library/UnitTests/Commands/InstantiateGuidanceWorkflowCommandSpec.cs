using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Commands;
using NuPattern.Runtime;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.References;

namespace NuPattern.Library.UnitTests.Commands
{
    [TestClass]
    public class InstantiateGuidanceWorkflowCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenACommandWithAnInstalledGuidanceExtension
        {
            private Mock<IGuidanceManager> mockManager;
            private InstantiateGuidanceWorkflowCommand command;
            private Mock<IGuidanceExtensionRegistration> mockRegistration;

            [TestInitialize]
            public void Initialize()
            {
                this.mockManager = new Mock<IGuidanceManager>();
                mockRegistration = new Mock<IGuidanceExtensionRegistration>();

                this.mockRegistration.SetupGet(reg => reg.ExtensionId).Returns("AnId");
                this.mockRegistration.SetupGet(reg => reg.DefaultName).Returns("DefaultInstanceName");

                this.mockManager.SetupGet(m => m.InstalledGuidanceExtensions).Returns(new[] { this.mockRegistration.Object });

                this.command = new InstantiateGuidanceWorkflowCommand();
                this.command.GuidanceManager = this.mockManager.Object;
                this.command.CurrentElement = new Mock<IProductElement>().Object;
                this.command.ServiceProvider = new Mock<IServiceProvider>().Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstanceOfAnotherWorkflow_ThenDoesNotCreateAWorkflow()
            {
                this.command.ExtensionId = "AnId2";
                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.ExtensionId, It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstance_ThenCreatesAndActivatesWorkflowWithUniqueDefaultWorkflowName()
            {
                this.command.ExtensionId = "AnId";
                this.command.ActivateOnInstantiation = true;

                Mock<IGuidanceExtension> mockExtension = new Mock<IGuidanceExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);
                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.ExtensionId, "DefaultInstanceName"), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveGuidanceExtension = mockExtension.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstanceWithDefaultInstanceName_ThenCreatesAndActivatesWorkflowWithDefaultInstanceName()
            {
                this.command.ExtensionId = "AnId";
                this.command.ActivateOnInstantiation = true;
                this.command.DefaultInstanceName = "ADefaultName";

                Mock<IGuidanceExtension> mockExtension = new Mock<IGuidanceExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.ExtensionId, this.command.DefaultInstanceName), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveGuidanceExtension = mockExtension.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstanceWithNoActivateOnInstantiation_ThenCreatesWorkflowWithNoActivation()
            {
                this.command.ExtensionId = "AnId";
                this.command.ActivateOnInstantiation = false;

                Mock<IGuidanceExtension> mockExtension = new Mock<IGuidanceExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.ExtensionId, "DefaultInstanceName"), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveGuidanceExtension = mockExtension.Object, Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstanceWithNoActivateOnInstantiation_ThenCreatesGuidanceReference()
            {
                Mock<IProductElement> owner = new Mock<IProductElement>();
                Mock<IReference> reference = new Mock<IReference>();
                reference.SetupAllProperties();

                owner.Setup(o => o.CreateReference(It.IsAny<Action<IReference>>()))
                    .Callback<Action<IReference>>(action => action(reference.Object))
                    .Returns(reference.Object);

                this.command.ExtensionId = "AnId";
                this.command.CurrentElement = owner.Object;
                this.command.ActivateOnInstantiation = false;

                Mock<IGuidanceExtension> mockExtension = new Mock<IGuidanceExtension>();
                mockExtension.Setup(e => e.InstanceName).Returns("DefaultInstanceName");
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.ExtensionId, "DefaultInstanceName"), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveGuidanceExtension = mockExtension.Object, Times.Once());

                Assert.Equal(ReferenceKindConstants.Guidance, reference.Object.Kind);
                Assert.Equal("DefaultInstanceName", reference.Object.Value);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstanceWithSharedInstanceOnInstantiation_ThenCreatesGuidanceReference()
            {
                Mock<IProductElement> owner = new Mock<IProductElement>();
                Mock<IReference> reference = new Mock<IReference>();
                reference.SetupAllProperties();

                owner.Setup(o => o.CreateReference(It.IsAny<Action<IReference>>()))
                    .Callback<Action<IReference>>(action => action(reference.Object))
                    .Returns(reference.Object);

                this.command.ExtensionId = "AnId";
                this.command.CurrentElement = owner.Object;
                this.command.SharedInstance = true;

                Mock<IGuidanceExtension> mockExtension = new Mock<IGuidanceExtension>();
                mockExtension.Setup(e => e.InstanceName).Returns("DefaultInstanceName");
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.ExtensionId, "DefaultInstanceName"), Times.Once());

                Assert.Equal(ReferenceKindConstants.Guidance, reference.Object.Kind);
                Assert.Equal("DefaultInstanceName", reference.Object.Value);
            }
        }

        [TestClass]
        public class GivenACommandWithAnInstalledGuidanceExtensionWithManyInstantiatedGuidanceWorkflows
        {
            private Mock<IGuidanceManager> mockManager = null;
            private InstantiateGuidanceWorkflowCommand command;

            [TestInitialize]
            public void Initialize()
            {
                this.mockManager = new Mock<IGuidanceManager>();
                var registration = new Mock<IGuidanceExtensionRegistration>();

                registration.SetupGet(reg => reg.ExtensionId).Returns("AnId");
                registration.SetupGet(reg => reg.DefaultName).Returns("DefaultInstanceName");

                this.mockManager.SetupGet(m => m.InstalledGuidanceExtensions).Returns(new[] { registration.Object });

                this.mockManager.SetupGet(m => m.InstantiatedGuidanceExtensions).Returns(new[]
                {
                    Mocks.Of<IGuidanceExtension>().First(f => f.InstanceName == "DefaultInstanceName" && f.ExtensionId == "AnId"),
                    Mocks.Of<IGuidanceExtension>().First(f => f.InstanceName == "DefaultInstanceName1" && f.ExtensionId == "AnId"),
                    Mocks.Of<IGuidanceExtension>().First(f => f.InstanceName == "DefaultInstanceName2" && f.ExtensionId == "AnId")
                });

                this.command = new InstantiateGuidanceWorkflowCommand();
                this.command.GuidanceManager = this.mockManager.Object;
                this.command.CurrentElement = new Mock<IProductElement>().Object;
                this.command.ServiceProvider = new Mock<IServiceProvider>().Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstance_ThenCreatesAndActivatesWorkflowWithUniqueDefaultWorkflowName()
            {
                this.command.ExtensionId = "AnId";

                var mockExtension = new Mock<IGuidanceExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.ExtensionId, "DefaultInstanceName3"), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveGuidanceExtension = mockExtension.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingWithSharedInstance_ThenSharesAndActivatesWorkflowWithUniqueDefaultWorkflowName()
            {
                this.command.ExtensionId = "AnId";
                this.command.SharedInstance = true;

                var mockExtension = new Mock<IGuidanceExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.ExtensionId, "DefaultInstanceName3"), Times.Never());
                var extension = Mock.Get(this.mockManager.Object.InstantiatedGuidanceExtensions.First());
                this.mockManager.VerifySet(fm => fm.ActiveGuidanceExtension = extension.Object);
            }
        }
    }
}
