using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.ComponentModel;
using NuPattern.Library.Automation;
using NuPattern.Modeling;
using NuPattern.Reflection;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.UnitTests.Automation.Extensions
{
    public class ArtifactExtensionDisplaySpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAProduct
        {
            private PatternSchema product;
            private ArtifactExtension artifactExtension;
            private DslTestStore<PatternModelDomainModel> store =
                new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.product = patternModel.Create<PatternSchema>();
                    this.product.Name = "WebService";
                });

                this.artifactExtension = ExtensionElement.GetExtension<ArtifactExtension>(this.product);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenAssociatedArtifactIsBrowsable()
            {
                var descriptor = TypedDescriptor.GetProperty(this.artifactExtension, extension => extension.AssociatedArtifacts);

                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenOnArtifactActivationNameDescriptorNotExist()
            {
                var descriptor = TypedDescriptor.GetProperty(this.artifactExtension, extension => extension.OnArtifactActivation);

                Assert.Null(descriptor);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenAssociatedArtifactDescriptorHasCorrectProperties()
            {
                Mock<ITypeDescriptorContext> mockContext = new Mock<ITypeDescriptorContext>();
                mockContext.SetupGet(context => context.Instance).Returns(this.product);

                var descriptor = TypedDescriptor.GetProperty(this.artifactExtension, extension => extension.AssociatedArtifacts);
                var descriptors = descriptor.Converter.GetProperties(mockContext.Object, string.Empty);

                Assert.Equal(2, descriptors.Count);
                Assert.NotNull(descriptors[Reflector<ArtifactExtension>.GetProperty(extension => extension.OnArtifactActivation).Name]);
                Assert.NotNull(descriptors[Reflector<ArtifactExtension>.GetProperty(extension => extension.OnArtifactDeletion).Name]);
            }
        }
    }
}
