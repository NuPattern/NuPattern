using System;
using Microsoft.VisualStudio.Patterning.Library.ValueProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.ValueProviders
{
	[TestClass]
	public class RandomGuidValueProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenAProvider
		{
			private RandomGuidValueProvider provider;

			[TestInitialize]
			public void InitializeContext()
			{
				this.provider = new RandomGuidValueProvider();
			}

			[TestMethod, TestCategory("Unit")]
			public void ThenReturnsGuidInDefaultFormat()
			{
				var result = this.provider.Evaluate();

				Assert.NotNull(result);
				Assert.NotNull(Guid.ParseExact((string)result, "D"));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenFormatIsDigitsHyphensCurlyBraces_ThenReturnsFormattedGuid()
			{
				this.provider.Format = GuidFormat.DigitsHyphensCurlyBraces;
				var result = this.provider.Evaluate();

				Assert.NotNull(result);
				Assert.NotNull(Guid.ParseExact((string)result, "B"));
			}
		}
	}
}
