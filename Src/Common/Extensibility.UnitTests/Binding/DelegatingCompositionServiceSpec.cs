using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
	[TestClass]
	public class DelegatingCompositionServiceSpec
	{
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		[TestMethod]
		public void WhenComposingParts_ThenDelegates()
		{
			var composition = new Mock<IFeatureCompositionService>();

			var delegating = new DelegatingCompositionService(composition.Object);

			delegating.ComposeParts(new object());

			composition.Verify(c => c.ComposeParts(It.IsAny<object[]>()));
		}

		[TestMethod]
		public void WhenDisposing_ThenDoesNotDisposeInnerService()
		{
			var composition = new Mock<IFeatureCompositionService>();

			var delegating = new DelegatingCompositionService(composition.Object);

			delegating.Dispose();

			composition.Verify(c => c.Dispose(), Times.Never());
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		[TestMethod]
		public void WhenGettingExport_ThenDelegates()
		{
			var composition = new Mock<IFeatureCompositionService>();

			var delegating = new DelegatingCompositionService(composition.Object);

			delegating.GetExport<IFormatProvider>();

			composition.Verify(c => c.GetExport<IFormatProvider>());
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		[TestMethod]
		public void WhenGettingExportedValue_ThenDelegates()
		{
			var composition = new Mock<IFeatureCompositionService>();

			var delegating = new DelegatingCompositionService(composition.Object);

			delegating.GetExportedValue<IFormatProvider>();

			composition.Verify(c => c.GetExportedValue<IFormatProvider>());
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenGettingExportedValueOrDefault_ThenDelegates()
		{
			var composition = new Mock<IFeatureCompositionService>();

			var delegating = new DelegatingCompositionService(composition.Object);

			delegating.GetExportedValueOrDefault<IFormatProvider>();

			composition.Verify(c => c.GetExportedValueOrDefault<IFormatProvider>());
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenGettingExportedValues_ThenDelegates()
		{
			var composition = new Mock<IFeatureCompositionService>();

			var delegating = new DelegatingCompositionService(composition.Object);

			delegating.GetExportedValues<IFormatProvider>();

			composition.Verify(c => c.GetExportedValues<IFormatProvider>());
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenGettingExports_ThenDelegates()
		{
			var composition = new Mock<IFeatureCompositionService>();

			var delegating = new DelegatingCompositionService(composition.Object);

			delegating.GetExports<IFormatProvider>();

			composition.Verify(c => c.GetExports<IFormatProvider>());
		}

		[TestMethod]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
		public void WhenSatisfyingImportsOnce_ThenDelegates()
		{
			var composition = new Mock<IFeatureCompositionService>();

			var delegating = new DelegatingCompositionService(composition.Object);

			delegating.SatisfyImportsOnce(AttributedModelServices.CreatePart(new object()));

			composition.Verify(c => c.SatisfyImportsOnce(It.IsAny<ComposablePart>()));
		}
	}
}
