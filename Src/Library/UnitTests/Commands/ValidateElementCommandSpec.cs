using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Commands
{
	[TestClass]
	public class ValidateElementCommandSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenACommand
		{
			private Mock<IProductElement> element;
			private Mock<IFxrUriReferenceService> uriService;
			private ValidateElementCommand command;

			[TestInitialize]
			public void Initialize()
			{
				this.element = new Mock<IProductElement>();
				this.uriService = new Mock<IFxrUriReferenceService>();

				this.command = new ValidateElementCommand();
				this.command.CurrentElement = this.element.Object;
			}

			[TestMethod]
			public void WhenCreatingNew_ThenValidateDescendantsIsTrue()
			{
				Assert.True(this.command.ValidateDescendants);
			}
		}
	}
}