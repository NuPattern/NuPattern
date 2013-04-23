using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Commands;
using NuPattern.Runtime;

namespace NuPattern.Library.UnitTests.Commands
{
    [TestClass]
    public class ValidateElementCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenACommand
        {
            private Mock<IProductElement> element;
            private Mock<IUriReferenceService> uriService;
            private ValidateElementCommand command;

            [TestInitialize]
            public void Initialize()
            {
                this.element = new Mock<IProductElement>();
                this.uriService = new Mock<IUriReferenceService>();

                this.command = new ValidateElementCommand();
                this.command.CurrentElement = this.element.Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNew_ThenValidateDescendantsIsTrue()
            {
                Assert.True(this.command.ValidateDescendants);
            }
        }
    }
}