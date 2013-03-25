using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Modeling;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Store;
using NuPattern.Runtime.Validation;

namespace NuPattern.Runtime.UnitTests.Store
{
    public class PropertySpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        [TestClass]
        public class GivenAProperty
        {
            internal DslTestStore<ProductStateStoreDomainModel> Store { get; private set; }
            internal ProductElement Element { get; private set; }
            internal Property Property { get; private set; }
            protected Mock<IDynamicBindingContext> BindingContext { get; private set; }

            [TestInitialize]
            public virtual void Initialize()
            {
                this.Store = new DslTestStore<ProductStateStoreDomainModel>();

                using (var tx = Store.TransactionManager.BeginTransaction())
                {
                    this.Property = Store.ElementFactory.CreateElement<Property>();
                    this.Element = Store.ElementFactory.CreateElement<Element>();
                    this.Element.Properties.Add(this.Property);
                    tx.Commit();
                }

                this.BindingContext = new Mock<IDynamicBindingContext> { DefaultValue = DefaultValue.Mock };

                var factory = new Mock<IBindingFactory> { DefaultValue = DefaultValue.Mock };
                factory.Setup(x => x.CreateContext()).Returns(this.BindingContext.Object);

                Mock.Get(this.Store.ServiceProvider)
                    .Setup(x => x.GetService(typeof(IBindingFactory)))
                    .Returns(factory.Object);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.Store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingStringProperty_ThenParentElementRaisesPropertyChangedEvent()
            {
                this.Property.Info = Mock.Of<IPropertyInfo>(info => info.Name == "My" && info.Type == "System.String");
                var changedProp = "";
                ((INotifyPropertyChanged)this.Element).PropertyChanged += (sender, args) => changedProp = args.PropertyName;

                this.Property.RawValue = "Foo";

                Assert.Equal(this.Property.Info.Name, changedProp);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResettingStringProperty_ThenSetsToEmptyString()
            {
                this.Property.Info = Mock.Of<IPropertyInfo>(info => info.Name == "My" && info.Type == "System.String");

                this.Property.RawValue = "Foo";
                this.Property.Reset();

                Assert.Equal(string.Empty, this.Property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResettingBooleanProperty_ThenSetsToFalse()
            {
                this.Property.Info = Mock.Of<IPropertyInfo>(info => info.Name == "My" && info.Type == "System.Boolean");

                this.Property.RawValue = "True";
                this.Property.Reset();

                Assert.Equal("False", this.Property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResettingWithDefaultValue_ThenSetsToDefault()
            {
                this.Property.Info = Mock.Of<IPropertyInfo>(info =>
                    info.Name == "My" &&
                    info.Type == "System.String" &&
                    info.DefaultValue == Mock.Of<IPropertyBindingSettings>(prop =>
                        prop.Name == "My" &&
                        prop.Value == "Bar"));

                this.Property.RawValue = "Foo";
                this.Property.Reset();

                Assert.Equal("Bar", this.Property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResettingWithDefaultValueExpression_ThenEvaluatesAgainstOwner()
            {
                this.Element.InstanceName = "Hello";
                this.Property.Info = Mock.Of<IPropertyInfo>(info =>
                    info.Name == "My" &&
                    info.Type == "System.String" &&
                    info.DefaultValue == Mock.Of<IPropertyBindingSettings>(prop =>
                        prop.Name == "My" &&
                        prop.Value == "{InstanceName} World"));

                this.Property.RawValue = "Foo";
                this.Property.Reset();

                Assert.Equal("Hello World", this.Property.RawValue);
            }
        }

        [TestClass]
        public class GivenAPropertyWithDefaultValueProvider : GivenAProperty
        {
            private Mock<IDynamicBinding<IValueProvider>> binding = new Mock<IDynamicBinding<IValueProvider>>();
            private Mock<IDynamicBindingContext> bindingContext = new Mock<IDynamicBindingContext>();
            private Mock<IValueProvider> valueProvider = new Mock<IValueProvider>();

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.Element.InstanceName = "Hello";

                this.Property.Info = Mock.Of<IPropertyInfo>(info =>
                    info.Name == "My" &&
                    info.Type == "System.String" &&
                    info.DefaultValue == Mock.Of<IPropertyBindingSettings>(prop =>
                        prop.Name == "My" &&
                        prop.Value == "{InstanceName} World" &&
                        prop.ValueProvider == Mock.Of<IValueProviderBindingSettings>()));

                this.binding.Setup(x => x.CreateDynamicContext()).Returns(this.bindingContext.Object);
                this.binding.Setup(x => x.Value).Returns(this.valueProvider.Object);

                Mock.Get(this.Store.ServiceProvider)
                    .Setup(x => x.GetService(typeof(IBindingFactory)))
                    .Returns(Mock.Of<IBindingFactory>(factory =>
                        factory.CreateBinding<IValueProvider>(It.IsAny<IBindingSettings>()) == this.binding.Object &&
                        factory.CreateContext() == this.bindingContext.Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBindingSucceedsAndResettingProperty_ThenSetsToValueProvider()
            {
                this.Property.RawValue = "foo";

                this.binding.Setup(x => x.Evaluate(this.bindingContext.Object)).Returns(true);
                this.valueProvider.Setup(x => x.Evaluate()).Returns("Hello");

                this.Property.Reset();

                Assert.Equal("Hello", this.Property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBindingFailsAndResettingProperty_ThenSetsToEvaluatedDefaultValue()
            {
                this.Property.RawValue = "foo";

                this.binding.Setup(x => x.Evaluate(this.bindingContext.Object)).Returns(false);

                this.Property.Reset();

                Assert.Equal("Hello World", this.Property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBindingFailsAndNoDefaultValue_ThenSetsToTypeDefault()
            {
                this.Property.RawValue = "foo";

                Mock.Get(this.Property.Info)
                    .Setup(x => x.Type)
                    .Returns("System.Boolean");
                Mock.Get(this.Property.Info.DefaultValue)
                    .Setup(x => x.Value)
                    .Returns((string)null);

                this.binding.Setup(x => x.Evaluate(this.bindingContext.Object)).Returns(false);

                this.Property.Reset();

                Assert.Equal("False", this.Property.RawValue);
            }
        }

        [TestClass]
        public class GivenAPropertyWithNoInfo : GivenAProperty
        {
            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.Property.DefinitionId = Guid.Empty;
                this.Property.Info = null;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResettingValue_ThenThrowsInvalidOperationException()
            {
                Assert.Throws<InvalidOperationException>(() => this.Property.Reset());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValue_ThenThrowsInvalidOperationException()
            {
                Assert.Throws<InvalidOperationException>(() => this.Property.Value = "Foo");
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingValue_ThenGetsRawValue()
            {
                this.Property.RawValue = "zz";

                Assert.Equal(this.Property.RawValue, this.Property.Value);
            }
        }

        [TestClass]
        public class GivenAPropertyWithValueProvider : GivenAProperty
        {
            private Mock<IDynamicBinding<IValueProvider>> binding = new Mock<IDynamicBinding<IValueProvider>>();
            private Mock<IDynamicBindingContext> bindingContext = new Mock<IDynamicBindingContext>();
            private Mock<IValueProvider> valueProvider = new Mock<IValueProvider>();
            private const string ProvidedValue = "baz";

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.Element.InstanceName = "Hello";

                this.Property.Info = Mock.Of<IPropertyInfo>(info =>
                    info.Name == "My" &&
                    info.Type == "System.String" &&
                    info.ValueProvider.TypeId == "Foo");

                this.binding.Setup(x => x.CreateDynamicContext()).Returns(this.bindingContext.Object);
                this.binding.Setup(x => x.Value).Returns(this.valueProvider.Object);
                this.valueProvider.Setup(x => x.Evaluate()).Returns(ProvidedValue);

                Mock.Get(this.Store.ServiceProvider)
                    .Setup(x => x.GetService(typeof(IBindingFactory)))
                    .Returns(Mock.Of<IBindingFactory>(factory =>
                        factory.CreateBinding<IValueProvider>(It.IsAny<IBindingSettings>()) == this.binding.Object &&
                        factory.CreateContext() == this.bindingContext.Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBindingSucceedsAndRetrievingValue_ThenGetsValueProvided()
            {
                this.binding.Setup(x => x.Evaluate(this.bindingContext.Object)).Returns(true);
                this.valueProvider.Setup(x => x.Evaluate()).Returns("Hello");

                Assert.Equal("Hello", (string)this.Property.Value);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBindingSucceedsAndRetrievingValue_ThenDoesNotSaveRawValueAutomatically()
            {
                this.Property.RawValue = "Foo";

                this.binding.Setup(x => x.Evaluate(this.bindingContext.Object)).Returns(true);
                this.valueProvider.Setup(x => x.Evaluate()).Returns("Hello");

                Assert.Equal("Hello", (string)this.Property.Value);
                Assert.Equal("Foo", this.Property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBindingFails_ThenReturnsLastKnownRawValue()
            {
                this.Property.RawValue = "foo";

                this.valueProvider.Setup(x => x.Evaluate()).Returns("Hello");
                this.binding.Setup(x => x.Evaluate(this.bindingContext.Object)).Returns(false);

                Assert.Equal("foo", this.Property.RawValue);
            }
        }

        [TestClass]
        public class GivenAPropertyWithValidationBinding : GivenAProperty
        {
            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.Element.InstanceName = "Hello";

                this.Property.Info = Mock.Of<IPropertyInfo>(info =>
                    info.Name == "My" &&
                    info.Type == "System.String" &&
                    info.ValidationSettings == new IBindingSettings[] { Mock.Of<IBindingSettings>() });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValidating_ThenBindingContextHasProperty()
            {
                var controller = new ValidationController();

                controller.ValidateCustom(this.Property, ValidationConstants.RuntimeValidationCategory);

                this.BindingContext.Verify(x => x.AddExport<IProperty>(this.Property));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValidating_ThenBindingContextHasOwnerElement()
            {
                var controller = new ValidationController();

                controller.ValidateCustom(this.Property, ValidationConstants.RuntimeValidationCategory);

                this.BindingContext.Verify(x => x.AddExport<IProductElement>(this.Element));
            }
        }
    }
}
