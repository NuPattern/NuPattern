using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility.Bindings;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Extensibility.UnitTests.Binding
{
    [TestClass]
    public class PropertyBindingSettingsExtensionsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private IBindingFactory bindingFactory;
        private ITraceSource tracer;
        private IDynamicBindingContext context;
        private IDynamicBinding<IValueProvider> binding;
        private IValueProvider valueProvider;

        [TestInitialize]
        public void Initialize()
        {
            this.tracer = Mock.Of<ITraceSource>();
            this.context = Mock.Of<IDynamicBindingContext>();
            this.valueProvider = Mock.Of<IValueProvider>(vp => (string)vp.Evaluate() == "Foo");

            this.binding = Mock.Of<IDynamicBinding<IValueProvider>>(binding =>
                binding.Evaluate(It.IsAny<IDynamicBindingContext>()) == true &&
                binding.Value == this.valueProvider &&
                binding.CreateDynamicContext() == this.context);

            this.bindingFactory = Mock.Of<IBindingFactory>(factory =>
                factory.CreateBinding<IValueProvider>(It.IsAny<IBindingSettings>()) == this.binding);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenNoValueProviderExists_ThenReturnsBindingValue()
        {
            var settings = new PropertyBindingSettings { Value = "Foo" };

            Assert.Equal("Foo", settings.Evaluate(this.bindingFactory, this.tracer));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenValueProviderExists_ThenEvaluatesItsValue()
        {
            var settings = new PropertyBindingSettings { ValueProvider = new ValueProviderBindingSettings { } };

            Assert.Equal("Foo", settings.Evaluate(this.bindingFactory, this.tracer));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenValueProviderBindingFails_ThenThrowsInvalidOperationExceptionAndTraces()
        {
            var settings = new PropertyBindingSettings { ValueProvider = new ValueProviderBindingSettings { } };

            Mock.Get(this.binding)
                .Setup(x => x.Evaluate(It.IsAny<IDynamicBindingContext>()))
                .Returns(false);

            Assert.Throws<InvalidOperationException>(() => settings.Evaluate(this.bindingFactory, this.tracer));
            Mock.Get(this.tracer)
                .Verify(x => x.TraceData(TraceEventType.Error, It.IsAny<int>(), It.IsAny<object>()));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenContextInitializerNotNull_ThenInvokesItOnEvaluation()
        {
            var settings = new PropertyBindingSettings { ValueProvider = new ValueProviderBindingSettings { } };
            bool invoked = false;

            settings.Evaluate(this.bindingFactory, this.tracer, c => invoked = true);

            Assert.True(invoked);
        }
    }
}
