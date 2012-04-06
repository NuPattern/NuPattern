using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests
{
	[TestClass]
	public class SchemaSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenInitializingAndExttensionPathIsNull_ThenThrowsArgumentNullException()
		{
			var content = new Mock<IExtensionContent>();
			content.Setup(c => c.RelativePath).Returns("foo.patterndefinition");
			content.Setup(c => c.Attributes).Returns(new Dictionary<string, string>() { { "AssemblyFile", "bar.dll" } });

			Assert.Throws<ArgumentNullException>(() => new SchemaResource(null, content.Object));
		}

		[TestMethod]
		public void WhenInitializingAndExttensionPathIsEmpty_ThenArgumentOutOfRangeExceptionIsThrown()
		{
			var content = new Mock<IExtensionContent>();
			content.Setup(c => c.RelativePath).Returns("foo.patterndefinition");
			content.Setup(c => c.Attributes).Returns(new Dictionary<string, string>() { { "AssemblyFile", "bar.dll" } });

			Assert.Throws<ArgumentOutOfRangeException>(() => new SchemaResource(string.Empty, content.Object));
		}

		[TestMethod]
		public void WhenInitilizingAndContentIsNull_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new SchemaResource(@"X:\", null));
		}

		[TestMethod]
		public void WhenExtensionContentAttributesIsNull_ThenArgumentExceptionIsThrown()
		{
			var content = new Mock<IExtensionContent>();
			content.Setup(c => c.RelativePath).Returns("foo.patterndefinition");

			Assert.Throws<ArgumentException>(() => new SchemaResource(@"X:\", content.Object));
		}

		[TestMethod]
		public void WhenExtensionContentAssemblyFileAttributeIsMissing_ThenArgumentExceptionIsThrown()
		{
			var content = new Mock<IExtensionContent>();
			content.Setup(c => c.RelativePath).Returns("foo.patterndefinition");
			content.Setup(c => c.Attributes).Returns(new Dictionary<string, string>() { { "Foo", "bar.dll" } });

			Assert.Throws<ArgumentException>(() => new SchemaResource(@"X:\", content.Object));
		}
	}
}