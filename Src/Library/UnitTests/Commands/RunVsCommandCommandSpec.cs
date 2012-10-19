using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Commands
{
    [TestClass]
    public class RunVsCommandCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenACommand
        {
            private Mock<ISolution> solution;
            private RunVsCommandCommand command;

            [TestInitialize]
            public void Initialize()
            {
                this.solution = new Mock<ISolution>();
                this.command = new RunVsCommandCommand();
                this.command.Solution = this.solution.Object;
            }

            [TestMethod]
            public void WhenCommandIsEmpty_ThenThrows()
            {
                Assert.Throws<ValidationException>(() =>
                    this.command.Execute());
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Test Code")]
            [TestMethod]
            public void WhenCommandIsInvalid_ThenThrows()
            {
                var mockSolution = new Mock<EnvDTE.Solution>();
                mockSolution.Setup(s => s.DTE.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>())).Throws(new COMException());
                this.solution.Setup(s => s.As<EnvDTE.Solution>()).Returns(mockSolution.Object);

                this.command.CommandName = "foo";

                Assert.Throws<COMException>(() =>
                    this.command.Execute());
            }

            [TestMethod]
            public void WhenCommandIsValid_ThenCommandIsExecuted()
            {
                var mockSolution = new Mock<EnvDTE.Solution>();
                mockSolution.Setup(s => s.DTE.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>()));
                this.solution.Setup(s => s.As<EnvDTE.Solution>()).Returns(mockSolution.Object);

                this.command.CommandName = "foo";
            }
        }
    }
}