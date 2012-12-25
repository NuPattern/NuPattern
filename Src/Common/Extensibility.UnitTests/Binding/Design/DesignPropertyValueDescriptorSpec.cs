using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility.Binding;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.UnitTests.Binding.Design
{
    public class DesignPropertyValueDescriptorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAStringDesignProperty
        {
            private DesignPropertyValueDescriptor descriptor;
            private DesignProperty component;
            private Mock<IPropertyBindingSettings> settings;

            [TestInitialize]
            public void InitializeContext()
            {
                this.settings = new Mock<IPropertyBindingSettings>();
                this.component = new DesignProperty(this.settings.Object);
                this.component.Attributes = new Attribute[0];
                this.component.Type = typeof(string);
                this.descriptor = new DesignPropertyValueDescriptor("AProperty", typeof(string), new System.ComponentModel.StringConverter(), new Attribute[0]);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueIsNull_ThenCanResetValueIsFalse()
            {
                this.settings.Setup(s => s.Value).Returns((string)null);

                Assert.False(this.descriptor.CanResetValue(this.component));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueIsEmpty_ThenCanResetValueIsFalse()
            {
                this.settings.Setup(s => s.Value).Returns(string.Empty);

                Assert.False(this.descriptor.CanResetValue(this.component));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenHasAValue_ThenCanResetValueIsTrue()
            {
                this.settings.Setup(s => s.Value).Returns("Foo");

                Assert.True(this.descriptor.CanResetValue(this.component));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenReset_ThenValueIsEmpty()
            {
                this.settings.Setup(s => s.Value).Returns("0");
                this.descriptor.ResetValue(this.component);

                this.settings.VerifySet(s => s.Value = string.Empty);
            }
        }

        [TestClass]
        public class GivenAnIntegerDesignProperty
        {
            private DesignPropertyValueDescriptor descriptor;
            private DesignProperty component;
            private Mock<IPropertyBindingSettings> settings;

            [TestInitialize]
            public void InitializeContext()
            {
                this.settings = new Mock<IPropertyBindingSettings>();
                this.component = new DesignProperty(this.settings.Object);
                this.component.Attributes = new Attribute[0];
                this.component.Type = typeof(int);
                this.descriptor = new DesignPropertyValueDescriptor("AProperty", typeof(int), new System.ComponentModel.Int32Converter(), new Attribute[0]);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueIsNull_ThenCanResetValueIsFalse()
            {
                this.settings.Setup(s => s.Value).Returns((string)null);

                Assert.False(this.descriptor.CanResetValue(this.component));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueIsEmpty_ThenCanResetValueIsFalse()
            {
                this.settings.Setup(s => s.Value).Returns(string.Empty);

                Assert.False(this.descriptor.CanResetValue(this.component));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenHasAValue_ThenCanResetValueIsTrue()
            {
                this.settings.Setup(s => s.Value).Returns("0");

                Assert.True(this.descriptor.CanResetValue(this.component));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenReset_ThenValueIsEmpty()
            {
                this.settings.Setup(s => s.Value).Returns("0");
                this.descriptor.ResetValue(this.component);

                this.settings.VerifySet(s => s.Value = string.Empty);
            }
        }
    }
}
