using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.ValueProviders;
using NuPattern.Runtime;

namespace NuPattern.Library.UnitTests.ValueProviders
{
    [TestClass]
    public class ElementPropertyValueProviderSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAProvider
        {
            private ElementPropertyValueProvider provider;

            [TestInitialize]
            public void InitializeContext()
            {
                this.provider = new ElementPropertyValueProvider();
                this.provider.PropertyName = "TestPropertyName";
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyNameIsNullOrEmpty_ThenEvaluateReturnsEmpty()
            {
                var owner = Mock.Of<IProductElement>(x => x.Root.ProductState == Mock.Of<IProductState>());
                this.provider.CurrentElement = owner;

                var result = this.provider.Evaluate();

                Assert.Equal(string.Empty, result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyNotFoundOnOwner_ThenEvaluateReturnsEmpty()
            {
                var owner = Mock.Of<IProductElement>(x =>
                    x.Properties == new IProperty[0] &&
                    x.Root.ProductState == Mock.Of<IProductState>());
                this.provider.CurrentElement = owner;
                this.provider.PropertyName = "Foo";

                var result = this.provider.Evaluate();

                Assert.Equal(string.Empty, result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyFoundOnOwner_ThenEvaluateReturnsPropertyValue()
            {
                string propertyName = "TestPropertyName";
                string propertyValue = "Foo";

                var property = Mock.Of<IProperty>(x =>
                    x.Info.Name == propertyName &&
                    x.RawValue == propertyValue);

                var owner = Mock.Of<IProductElement>(x =>
                    x.Properties == new[] { property } &&
                    x.Root.ProductState == Mock.Of<IProductState>());

                this.provider.CurrentElement = owner;
                this.provider.PropertyName = propertyName;

                TypeDescriptor.AddProviderTransparent(new VariablePropertyProvider(), owner);

                var result = this.provider.Evaluate();

                Mock.Get(property).VerifyGet(prop => prop.RawValue, Times.Once());
                Assert.Equal(propertyValue, result);
            }

            private class VariablePropertyProvider : TypeDescriptionProvider
            {
                public override ICustomTypeDescriptor GetTypeDescriptor(System.Type objectType, object instance)
                {
                    return new VariablePropertiesDescriptor(instance as IProductElement);
                }

                private class VariablePropertiesDescriptor : CustomTypeDescriptor
                {
                    private IProductElement element;

                    public VariablePropertiesDescriptor(IProductElement element)
                    {
                        this.element = element;
                    }

                    public override PropertyDescriptorCollection GetProperties()
                    {
                        return this.GetProperties(new System.Attribute[0]);
                    }

                    public override PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
                    {
                        var properties = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

                        foreach (var prop in this.element.Properties)
                        {
                            properties.Add(new VariablePropertyDescriptor(prop));
                        }

                        return properties;
                    }
                }

                private class VariablePropertyDescriptor : PropertyDescriptor
                {
                    private IProperty prop;

                    public VariablePropertyDescriptor(IProperty prop)
                        : base(prop.Info.Name, new System.Attribute[0])
                    {
                        this.prop = prop;
                    }

                    public override bool CanResetValue(object component)
                    {
                        throw new System.NotImplementedException();
                    }

                    public override System.Type ComponentType
                    {
                        get { throw new System.NotImplementedException(); }
                    }

                    public override object GetValue(object component)
                    {
                        return this.prop.RawValue;
                    }

                    public override bool IsReadOnly
                    {
                        get { throw new System.NotImplementedException(); }
                    }

                    public override System.Type PropertyType
                    {
                        get { throw new System.NotImplementedException(); }
                    }

                    public override void ResetValue(object component)
                    {
                        throw new System.NotImplementedException();
                    }

                    public override void SetValue(object component, object value)
                    {
                        throw new System.NotImplementedException();
                    }

                    public override bool ShouldSerializeValue(object component)
                    {
                        throw new System.NotImplementedException();
                    }
                }
            }
        }
    }
}
