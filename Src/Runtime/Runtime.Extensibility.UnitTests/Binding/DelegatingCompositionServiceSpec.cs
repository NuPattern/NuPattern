using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Composition;

namespace NuPattern.Runtime.UnitTests.Binding
{
    [TestClass]
    public class DelegatingCompositionServiceSpec
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        [TestMethod, TestCategory("Unit")]
        public void WhenComposingParts_ThenDelegates()
        {
            var composition = new Mock<IFeatureCompositionService>();

            var delegating = new DelegatingCompositionService(composition.Object);

            delegating.ComposeParts(new object());

            composition.Verify(c => c.ComposeParts(It.IsAny<object[]>()));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDisposing_ThenDoesNotDisposeInnerService()
        {
            var composition = new Mock<IFeatureCompositionService>();

            var delegating = new DelegatingCompositionService(composition.Object);

            delegating.Dispose();

            composition.Verify(c => c.Dispose(), Times.Never());
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        [TestMethod, TestCategory("Unit")]
        public void WhenGettingExport_ThenDelegates()
        {
            var composition = new Mock<IFeatureCompositionService>();

            var delegating = new DelegatingCompositionService(composition.Object);

            delegating.GetExport<IFormatProvider>();

            composition.Verify(c => c.GetExport<IFormatProvider>());
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        [TestMethod, TestCategory("Unit")]
        public void WhenGettingExportedValue_ThenDelegates()
        {
            var composition = new Mock<IFeatureCompositionService>();

            var delegating = new DelegatingCompositionService(composition.Object);

            delegating.GetExportedValue<IFormatProvider>();

            composition.Verify(c => c.GetExportedValue<IFormatProvider>());
        }

        [TestMethod, TestCategory("Unit")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        public void WhenGettingExportedValueOrDefault_ThenDelegates()
        {
            var composition = new Mock<IFeatureCompositionService>();

            var delegating = new DelegatingCompositionService(composition.Object);

            delegating.GetExportedValueOrDefault<IFormatProvider>();

            composition.Verify(c => c.GetExportedValueOrDefault<IFormatProvider>());
        }

        [TestMethod, TestCategory("Unit")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        public void WhenGettingExportedValues_ThenDelegates()
        {
            var composition = new Mock<IFeatureCompositionService>();

            var delegating = new DelegatingCompositionService(composition.Object);

            delegating.GetExportedValues<IFormatProvider>();

            composition.Verify(c => c.GetExportedValues<IFormatProvider>());
        }

        [TestMethod, TestCategory("Unit")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        public void WhenGettingExports_ThenDelegates()
        {
            var composition = new Mock<IFeatureCompositionService>();

            var delegating = new DelegatingCompositionService(composition.Object);

            delegating.GetExports<IFormatProvider>();

            composition.Verify(c => c.GetExports<IFormatProvider>());
        }

        [TestMethod, TestCategory("Unit")]
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
