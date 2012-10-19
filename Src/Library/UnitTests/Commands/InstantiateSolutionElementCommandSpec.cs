using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Commands
{
    [TestClass]
    public class InstantiateSolutionElementCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        protected const string ExpectedToolkitIdentifier = "ToolkitId";
        protected const string ExpectedInstanceName = "ElementName";

        public abstract class GivenAPatternManager
        {
            protected InstantiateSolutionElementCommand Command { get; set; }
            protected Mock<IPatternManager> MockPatternManager { get; set; }

            public virtual void Initialize()
            {
                this.Command = new InstantiateSolutionElementCommand();
                this.Command.InstanceName = ExpectedInstanceName;
                this.MockPatternManager = CreateMockPatternManager();
                this.Command.PatternManager = this.MockPatternManager.Object;
            }
        }

        [TestClass]
        public class GivenAPatternManagerWithAnInstalledToolkit : GivenAPatternManager
        {
            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();
                this.Command.ToolkitIdentifier = ExpectedToolkitIdentifier;
            }

            [TestMethod]
            public void WhenExecuting_CreatesANewProductElement()
            {
                this.Command.Execute();

                this.MockPatternManager.Verify(p => p.CreateProduct(
                    It.Is<IInstalledToolkitInfo>(toolkitInfo => toolkitInfo.Id == ExpectedToolkitIdentifier),
                    It.Is<string>(name => name == ExpectedInstanceName),
                    It.Is<bool>(raiseInstantiatedEvents => raiseInstantiatedEvents == true)),
                    Times.Once());
            }
        }

        [TestClass]
        public class GivenAPatternManagerWithoutExpectedToolkitInstalled : GivenAPatternManager
        {
            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();
                this.Command.ToolkitIdentifier = "ElementIdThatIsNotInstalled";
            }

            [TestMethod]
            public void WhenExecuting_ThrowsAnInvalidOperationException()
            {
                Assert.Throws<InvalidOperationException>(() => this.Command.Execute());
            }
        }

        [TestClass]
        public class GivenInvalidCommandSetup : GivenAPatternManager
        {
            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();
                this.Command.ToolkitIdentifier = ExpectedToolkitIdentifier;
            }

            /// <summary>
            /// Ensures that the object validator throws if the pattern manager property is null.
            /// </summary>
            /// <remarks>
            /// Test ignored until ObjectValidator.ThrowIfInvalid() bug is fixed.
            /// Currently required properties with Internal visibility are not validated.
            /// </remarks>
            [TestMethod, Ignore]
            public void WhenPatternManagerNull_ValidatorThrowsInvalidOperationException()
            {
                this.Command.PatternManager = null;

                Assert.Throws<InvalidOperationException>(() => this.Command.Execute());
            }

            [TestMethod]
            public void WhenToolkitIdentifierNull_ValidatorThrowsInvalidOperationException()
            {
                this.Command.ToolkitIdentifier = null;

                Assert.Throws<ValidationException>(() => this.Command.Execute());
            }

            [TestMethod]
            public void WhenInstanceNameNull_ValidatorThrowsInvalidOperationException()
            {
                this.Command.InstanceName = null;

                Assert.Throws<ValidationException>(() => this.Command.Execute());
            }
        }

        private static Mock<IPatternManager> CreateMockPatternManager()
        {
            var installedToolkits = new List<IInstalledToolkitInfo>();
            installedToolkits.Add(Mock.Of<IInstalledToolkitInfo>(i => i.Id == ExpectedToolkitIdentifier));
            var mockPatternManager = new Mock<IPatternManager>();
            mockPatternManager.SetupGet(p => p.InstalledToolkits).Returns(installedToolkits);
            return mockPatternManager;
        }
    }
}