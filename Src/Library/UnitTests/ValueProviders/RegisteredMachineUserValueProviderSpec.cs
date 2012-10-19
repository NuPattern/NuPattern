using Microsoft.VisualStudio.Patterning.Library.ValueProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.ValueProviders
{
	[TestClass]
	public class RegisteredMachineUserValueProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenAProvider
		{
			private RegisteredMachineUserValueProvider provider;

			[TestInitialize]
			public void InitializeContext()
			{
				this.provider = new RegisteredMachineUserValueProvider();
			}

			[TestMethod]
			public void ThenReturnsNotNullOrEmpty()
			{
				var result = this.provider.Evaluate();

				Assert.NotNull(result);
			}
		}
	}
}
