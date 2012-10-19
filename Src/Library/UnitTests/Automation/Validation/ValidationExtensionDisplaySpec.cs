using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Runtime.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Automation.Validation
{
    [TestClass]
    public class ValidationExtensionDisplaySpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAProduct
        {
            private PatternSchema product;
            private ValidationExtension validationExtension;
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

                this.validationExtension = ExtensionElement.GetExtension<ValidationExtension>(this.product);
            }

            [TestMethod]
            public void ThenValidationExecutionIsBrowsable()
            {
                var descriptor = TypedDescriptor.GetProperty(this.validationExtension, extension => extension.ValidationExecution);

                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }

            [TestMethod]
            public void ThenValidationOnBuildDescriptorNotExist()
            {
                var descriptor = TypedDescriptor.GetProperty(this.validationExtension, extension => extension.ValidationOnBuild);

                Assert.Null(descriptor);
            }

            [TestMethod]
            public void ThenValidationOnCustomMenuDescriptorNotExist()
            {
                var descriptor = TypedDescriptor.GetProperty(this.validationExtension, extension => extension.ValidationOnCustomEvent);

                Assert.Null(descriptor);
            }

            [TestMethod]
            public void ThenValidationOnMenuDescriptorNotExist()
            {
                var descriptor = TypedDescriptor.GetProperty(this.validationExtension, extension => extension.ValidationOnMenu);

                Assert.Null(descriptor);
            }

            [TestMethod]
            public void ThenValidationOnSaveDescriptorNotExist()
            {
                var descriptor = TypedDescriptor.GetProperty(this.validationExtension, extension => extension.ValidationOnSave);

                Assert.Null(descriptor);
            }

            [TestMethod]
            public void ThenValidationExecutionDescriptorHasCorrectProperties()
            {
                Mock<ITypeDescriptorContext> mockContext = new Mock<ITypeDescriptorContext>();
                mockContext.SetupGet(context => context.Instance).Returns(this.product);

                var descriptor = TypedDescriptor.GetProperty(this.validationExtension, extension => extension.ValidationExecution);
                var descriptors = descriptor.Converter.GetProperties(mockContext.Object, string.Empty);

                Assert.Equal(4, descriptors.Count);
                Assert.NotNull(descriptors[Reflector<ValidationExtension>.GetProperty(extension => extension.ValidationOnBuild).Name]);
                Assert.NotNull(descriptors[Reflector<ValidationExtension>.GetProperty(extension => extension.ValidationOnCustomEvent).Name]);
                Assert.NotNull(descriptors[Reflector<ValidationExtension>.GetProperty(extension => extension.ValidationOnMenu).Name]);
                Assert.NotNull(descriptors[Reflector<ValidationExtension>.GetProperty(extension => extension.ValidationOnSave).Name]);
            }
        }
    }
}