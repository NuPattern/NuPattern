using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.ComponentModel;
using NuPattern.Library.Automation;
using NuPattern.Modeling;
using NuPattern.Reflection;
using NuPattern.Runtime.Extensibility;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.UnitTests.Automation.Guidance
{
    [TestClass]
    public class GuidanceExtensionDisplaySpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAProduct
        {
            private PatternSchema product;
            private GuidanceExtension guidanceExtension;
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

                this.guidanceExtension = ExtensionElement.GetExtension<GuidanceExtension>(this.product);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenAssociatedGuidanceIsBrowsable()
            {
                var descriptor = TypedDescriptor.GetProperty(this.guidanceExtension, extension => extension.AssociatedGuidance);

                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenFeatureIdIsNotExist()
            {
                var descriptor = TypedDescriptor.GetProperty(this.guidanceExtension, extension => extension.GuidanceFeatureId);

                Assert.Null(descriptor);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDefaultInstanceNameDescriptorNotExist()
            {
                var descriptor = TypedDescriptor.GetProperty(this.guidanceExtension, extension => extension.GuidanceInstanceName);

                Assert.Null(descriptor);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenActivateOnCreationIsNotExist()
            {
                var descriptor = TypedDescriptor.GetProperty(this.guidanceExtension, extension => extension.GuidanceActivateOnCreation);

                Assert.Null(descriptor);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSharedInstanceIsNotExist()
            {
                var descriptor = TypedDescriptor.GetProperty(this.guidanceExtension, extension => extension.GuidanceSharedInstance);

                Assert.Null(descriptor);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenAssociatedGuidanceDescriptorHasCorrectProperties()
            {
                Mock<ITypeDescriptorContext> mockContext = new Mock<ITypeDescriptorContext>();
                mockContext.SetupGet(context => context.Instance).Returns(this.product);

                var descriptor = TypedDescriptor.GetProperty(this.guidanceExtension, extension => extension.AssociatedGuidance);
                var descriptors = descriptor.Converter.GetProperties(mockContext.Object, string.Empty);

                Assert.Equal(4, descriptors.Count);
                Assert.NotNull(descriptors[Reflector<GuidanceExtension>.GetProperty(extension => extension.GuidanceFeatureId).Name]);
                Assert.NotNull(descriptors[Reflector<GuidanceExtension>.GetProperty(extension => extension.GuidanceActivateOnCreation).Name]);
                Assert.NotNull(descriptors[Reflector<GuidanceExtension>.GetProperty(extension => extension.GuidanceInstanceName).Name]);
                Assert.NotNull(descriptors[Reflector<GuidanceExtension>.GetProperty(extension => extension.GuidanceSharedInstance).Name]);
            }
        }
    }
}
