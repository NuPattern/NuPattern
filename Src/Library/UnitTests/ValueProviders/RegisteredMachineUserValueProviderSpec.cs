using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using NuPattern.Library.ValueProviders;

namespace NuPattern.Library.UnitTests.ValueProviders
{
	[TestClass]
	public class RegisteredMachineUserValueProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenAProvider
		{
			private RegisteredMachineUserValueProvider provider;
		    private Mock<IRegistryReader> reader;

			[TestInitialize]
			public void InitializeContext()
			{
			    this.reader = new Mock<IRegistryReader>();
                this.provider = new RegisteredMachineUserValueProvider(this.reader.Object);
			}

			[TestMethod, TestCategory("Unit")]
			public void ThenReturnsRegisteredMachineUser()
			{
			    this.reader.Setup(r => r.ReadValue()).Returns("foo");

				var result = this.provider.Evaluate();

				Assert.Equal("foo", result);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenRegisteredMachineIsEmpty_ThenReturnsUnknownUser()
			{
                this.reader.Setup(r => r.ReadValue()).Returns(string.Empty);

                var result = this.provider.Evaluate();

                Assert.Equal(RegisteredMachineUserValueProvider.UnknownOrganization, result);
            }	
        }
	}
}
