using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests.Events
{
	[TestClass]
	public class AnonymousDisposableSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenConstructedWithNullAction_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				using (new AnonymousDisposable(null))
				{
				}
			});
		}

		[TestMethod]
		public void WhenDisposed_ThenInvokesAction()
		{
			var called = false;

			using (new AnonymousDisposable(() => called = true))
			{
			}

			Assert.True(called);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "On purpose")]
		[TestMethod]
		public void WhenDisposedTwice_ThenInvokesActionOnce()
		{
			var called = 0;

			using (var disposable = new AnonymousDisposable(() => called++))
			{
				disposable.Dispose();
				disposable.Dispose();
			}

			Assert.Equal(1, called);
		}
	}
}
