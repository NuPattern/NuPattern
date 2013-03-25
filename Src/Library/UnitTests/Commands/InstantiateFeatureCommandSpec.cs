using System;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Commands;
using NuPattern.Runtime;
using NuPattern.Runtime.References;

namespace NuPattern.Library.UnitTests.Commands
{
    [TestClass]
    public class InstantiateFeatureCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenACommandWithAnInstalledFeature
        {
            private Mock<IFeatureManager> mockManager;
            private InstantiateFeatureCommand command;
            private Mock<IFeatureRegistration> mockRegistration;

            [TestInitialize]
            public void Initialize()
            {
                this.mockManager = new Mock<IFeatureManager>();
                mockRegistration = new Mock<IFeatureRegistration>();

                this.mockRegistration.SetupGet(reg => reg.FeatureId).Returns("FeatureId");
                this.mockRegistration.SetupGet(reg => reg.DefaultName).Returns("DefaultInstanceName");

                this.mockManager.SetupGet(m => m.InstalledFeatures).Returns(new IFeatureRegistration[] { this.mockRegistration.Object });

                this.command = new InstantiateFeatureCommand();
                this.command.FeatureManager = this.mockManager.Object;
                this.command.CurrentElement = new Mock<IProductElement>().Object;
                this.command.ServiceProvider = new Mock<IServiceProvider>().Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstanceOfAnotherFeature_ThenDoesNotCreateAFeature()
            {
                this.command.FeatureId = "FeatureId2";
                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.FeatureId, It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstance_ThenCreatesAndActivatesFeatureWithUniqueDefaultFeatureName()
            {
                this.command.FeatureId = "FeatureId";
                this.command.ActivateOnInstantiation = true;

                Mock<IFeatureExtension> mockExtension = new Mock<IFeatureExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);
                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.FeatureId, "DefaultInstanceName"), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveFeature = mockExtension.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstanceWithDefaultInstanceName_ThenCreatesAndActivatesFeatureWithDefaultInstanceName()
            {
                this.command.FeatureId = "FeatureId";
                this.command.ActivateOnInstantiation = true;
                this.command.DefaultInstanceName = "ADefaultName";

                Mock<IFeatureExtension> mockExtension = new Mock<IFeatureExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.FeatureId, this.command.DefaultInstanceName), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveFeature = mockExtension.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstanceWithNoActivateOnInstantiation_ThenCreatesFeatureWithNoActivation()
            {
                this.command.FeatureId = "FeatureId";
                this.command.ActivateOnInstantiation = false;

                Mock<IFeatureExtension> mockExtension = new Mock<IFeatureExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.FeatureId, "DefaultInstanceName"), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveFeature = mockExtension.Object, Times.Never());
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

                this.command.FeatureId = "FeatureId";
                this.command.CurrentElement = owner.Object;
                this.command.ActivateOnInstantiation = false;

                Mock<IFeatureExtension> mockExtension = new Mock<IFeatureExtension>();
                mockExtension.Setup(e => e.InstanceName).Returns("DefaultInstanceName");
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.FeatureId, "DefaultInstanceName"), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveFeature = mockExtension.Object, Times.Never());

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

                this.command.FeatureId = "FeatureId";
                this.command.CurrentElement = owner.Object;
                this.command.SharedInstance = true;

                Mock<IFeatureExtension> mockExtension = new Mock<IFeatureExtension>();
                mockExtension.Setup(e => e.InstanceName).Returns("DefaultInstanceName");
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.FeatureId, "DefaultInstanceName"), Times.Once());

                Assert.Equal(ReferenceKindConstants.Guidance, reference.Object.Kind);
                Assert.Equal("DefaultInstanceName", reference.Object.Value);
            }
        }

        [TestClass]
        public class GivenACommandWithAnInstalledFeatureWithManyInstantiatedFeatures
        {
            private Mock<IFeatureManager> mockManager = null;
            private InstantiateFeatureCommand command;

            [TestInitialize]
            public void Initialize()
            {
                this.mockManager = new Mock<IFeatureManager>();
                var registration = new Mock<IFeatureRegistration>();

                registration.SetupGet(reg => reg.FeatureId).Returns("FeatureId");
                registration.SetupGet(reg => reg.DefaultName).Returns("DefaultInstanceName");

                this.mockManager.SetupGet(m => m.InstalledFeatures).Returns(new IFeatureRegistration[] { registration.Object });

                this.mockManager.SetupGet(m => m.InstantiatedFeatures).Returns(new IFeatureExtension[]
                {
                    Mocks.Of<IFeatureExtension>().First(f => f.InstanceName == "DefaultInstanceName" && f.FeatureId == "FeatureId"),
                    Mocks.Of<IFeatureExtension>().First(f => f.InstanceName == "DefaultInstanceName1" && f.FeatureId == "FeatureId"),
                    Mocks.Of<IFeatureExtension>().First(f => f.InstanceName == "DefaultInstanceName2" && f.FeatureId == "FeatureId")
                });

                this.command = new InstantiateFeatureCommand();
                this.command.FeatureManager = this.mockManager.Object;
                this.command.CurrentElement = new Mock<IProductElement>().Object;
                this.command.ServiceProvider = new Mock<IServiceProvider>().Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAnInstance_ThenCreatesAndActivatesFeatureWithUniqueDefaultFeatureName()
            {
                this.command.FeatureId = "FeatureId";

                Mock<IFeatureExtension> mockExtension = new Mock<IFeatureExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.FeatureId, "DefaultInstanceName3"), Times.Once());
                this.mockManager.VerifySet(fm => fm.ActiveFeature = mockExtension.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingWithSharedInstance_ThenSharesAndActivatesFeatureWithUniqueDefaultFeatureName()
            {
                this.command.FeatureId = "FeatureId";
                this.command.SharedInstance = true;

                Mock<IFeatureExtension> mockExtension = new Mock<IFeatureExtension>();
                this.mockManager.Setup(fm => fm.Instantiate(It.IsAny<string>(), It.IsAny<string>())).Returns(mockExtension.Object);

                this.command.Execute();

                this.mockManager.Verify(fm => fm.Instantiate(this.command.FeatureId, "DefaultInstanceName3"), Times.Never());
                var extension = Mock.Get(this.mockManager.Object.InstantiatedFeatures.First());
                this.mockManager.VerifySet(fm => fm.ActiveFeature = extension.Object);
            }
        }
    }
}
