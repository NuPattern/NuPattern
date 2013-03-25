using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Runtime.UnitTests.Binding
{
    public class BindingPropertyDescriptorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAProperty
        {
            private BindingPropertyDescriptor<IValueProvider> descriptor;
            private string component;
            private BindingSettings settings;

            [TestInitialize]
            public void InitializeContext()
            {
                this.settings = new BindingSettings { TypeId = string.Empty };
                this.component = "SomeComponent";

                var innerDescriptor = new Mock<PropertyDescriptor>("foo", new Attribute[0]);
                innerDescriptor.Setup(x => x.Name).Returns("Property");
                innerDescriptor.Setup(x => x.Attributes).Returns(new AttributeCollection());
                innerDescriptor.Setup(x => x.GetValue(this.component)).Returns(this.settings);
                this.descriptor = new BindingPropertyDescriptor<IValueProvider>(innerDescriptor.Object, new Attribute[0]);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenCanResetValueIsFalse()
            {
                Assert.False(this.descriptor.CanResetValue(this.component));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueProviderTypeIdIsSet_ThenCanResetValueIsTrue()
            {
                this.descriptor.SetValue(this.component, "SomeTypeId");

                Assert.True(this.descriptor.CanResetValue(this.component));
            }
        }
    }
}
