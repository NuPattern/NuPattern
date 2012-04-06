using System;
using Microsoft.VisualStudio.Patterning.Runtime.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests
{
	[TestClass]
	public class VsMenuCommandAdapterSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenCreatingMenuCommandAndCommandIsNull_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new VsMenuCommandAdapter(null));
		}
	}
}