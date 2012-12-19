using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Store.UnitTests
{
    public class ProductElementTypeDescriptorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        [TestClass]
        public class GivenAnElementWithAProperty
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private const string PropertyName = "MyProperty";
            private ProductElement element;
            private Property property;
            private ProductElementTypeDescriptor descriptor;

            [TestInitialize]
            public void Initialize()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                using (var tx = store.TransactionManager.BeginTransaction())
                {
                    this.property = store.ElementFactory.CreateElement<Property>();
                    this.property.Info = Mocks.Of<IPropertyInfo>().First(i => i.Name == PropertyName && i.IsVisible == true && i.IsReadOnly == false);

                    this.element = store.ElementFactory.CreateElement<Element>();
                    this.element.Properties.Add(this.property);
                    tx.Commit();
                }

                this.descriptor = new ProductElementTypeDescriptor(
                    TypeDescriptor.GetProvider(this.element).GetTypeDescriptor(this.element), this.element);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyTypeIsString_ThenTypeIsResolvedForPropertyInfo()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.String");

                var info = this.descriptor.GetProperties().Cast<PropertyDescriptor>().First(p => p.Name == PropertyName);

                Assert.Equal(typeof(string), info.PropertyType);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingDescriptorValue_ThenSetsPropertyValue()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.String");
                var info = this.descriptor.GetProperties().Cast<PropertyDescriptor>().First(p => p.Name == PropertyName);

                info.SetValue(this.element, "Bar");

                Assert.Equal("Bar", this.property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyTypeIsBoolean_ThenConvertsValue()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.Boolean");
                var info = this.descriptor.GetProperties().Cast<PropertyDescriptor>().First(p => p.Name == PropertyName);

                info.SetValue(this.element, true);

                Assert.True(bool.Parse(this.property.RawValue));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueDoesNotEqualDefaultValue_ThenCanResetProperty()
            {
                Mock.Get(this.property.Info)
                    .Setup(x => x.Type)
                    .Returns("System.String");

                Mock.Get(this.property.Info)
                    .Setup(x => x.DefaultValue)
                    .Returns(Mock.Of<IPropertyBindingSettings>(x => x.Value == "Foo"));

                this.property.RawValue = "Bar";

                var info = this.descriptor.GetProperties().Cast<PropertyDescriptor>().First(p => p.Name == PropertyName);

                Assert.True(info.CanResetValue(this.element));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResettingValue_ThenValueIsRevertedToDefaultValue()
            {
                Mock.Get(this.property.Info)
                    .Setup(x => x.Type)
                    .Returns("System.String");

                Mock.Get(this.property.Info)
                    .Setup(x => x.DefaultValue)
                    .Returns(Mock.Of<IPropertyBindingSettings>(x => x.Value == "Foo"));

                this.property.RawValue = "Bar";
                var info = this.descriptor.GetProperties().Cast<PropertyDescriptor>().First(p => p.Name == PropertyName);

                info.ResetValue(this.element);

                Assert.Equal(this.property.Info.DefaultValue.Value, this.property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyIsNotBrowsable_ThenTypeDescriptorReturnsItAnyways()
            {
                Assert.NotNull(TypeDescriptor.GetProperties(typeof(Foo))["Value"]);
                Assert.Null(TypeDescriptor.GetProperties(typeof(Foo), new Attribute[] { new BrowsableAttribute(true) })["Value"]);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyIsNotVisible_ThenIsBrowsableIsFalse()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.String");
                Mock.Get(this.property.Info).Setup(x => x.IsVisible).Returns(false);

                Assert.False(this.descriptor.GetProperties()[PropertyName].IsBrowsable);
                Assert.Null(TypeDescriptor.GetProperties(this.descriptor, new Attribute[] { new BrowsableAttribute(true) })[PropertyName]);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyIsReadOnly_ThenIsReadOnlyIsTrue()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.String");
                Mock.Get(this.property.Info).Setup(x => x.IsReadOnly).Returns(true);

                Assert.True(this.descriptor.GetProperties()[PropertyName].IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyHasDescription_ThenDescriptorHasDescription()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.String");
                Mock.Get(this.property.Info).Setup(x => x.Description).Returns("Hello World");

                Assert.Equal("Hello World", this.descriptor.GetProperties()[PropertyName].Description);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyHasCategory_ThenDescriptorHasCategory()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.String");
                Mock.Get(this.property.Info).Setup(x => x.Category).Returns("Data");

                Assert.Equal("Data", this.descriptor.GetProperties()[PropertyName].Category);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyDoesNotHaveInfo_ThenDescriptorHidesIt()
            {
                var count = this.descriptor.GetProperties().Count;

                this.element.CreateProperty(p => p.DefinitionName = "NoInfo");

                Assert.Equal(count, this.descriptor.GetProperties().Count);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueIsString_ThenSetsPropertyValue()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.Int32");

                var info = this.descriptor.GetProperties().Cast<PropertyDescriptor>().First(p => p.Name == PropertyName);

                info.SetValue(this.element, "foo");

                Assert.Equal("foo", this.property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingTypedValue_ThenConvertsUsingBuiltInConverter()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.Int32");
                this.property.RawValue = "25";

                var info = this.descriptor.GetProperties().Cast<PropertyDescriptor>().First(p => p.Name == PropertyName);

                var value = info.GetValue(this.element);

                Assert.Equal(25, (int)value);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingInvalidValue_ThenSetsPropertyValue()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.Int32");

                var info = this.descriptor.GetProperties().Cast<PropertyDescriptor>().First(p => p.Name == PropertyName);

                info.SetValue(this.element, long.MaxValue);

                Assert.Equal(long.MaxValue.ToString(), this.property.RawValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPropertyHasCustomConverter_ThenUsesItAutomatically()
            {
                Mock.Get(this.property.Info).Setup(x => x.Type).Returns("System.String");
                Mock.Get(this.property.Info).Setup(x => x.TypeConverterTypeName).Returns(typeof(PointConverter).AssemblyQualifiedName);

                var info = this.descriptor.GetProperties().Cast<PropertyDescriptor>().First(p => p.Name == PropertyName);

                var value = info.GetValue(this.element);

                Assert.True(value is Point);

                var point = (Point)value;

                point.X = 25;
                point.Y = 10;

                info.SetValue(this.element, point);

                Assert.Equal("25:10", this.property.RawValue);

                var saved = (Point)info.GetValue(this.element);

                Assert.Equal(25, saved.X);
                Assert.Equal(10, saved.Y);
            }

            public class Foo
            {
                [Browsable(false)]
                public int Value { get; set; }

                public int Name { get; set; }
            }

            public class Point
            {
                public int X { get; set; }
                public int Y { get; set; }
            }

            public class PointConverter : StringConverter
            {
                public override bool IsValid(ITypeDescriptorContext context, object value)
                {
                    return value is Point || (value is string && ((string)value).Length > 0);
                }

                public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
                {
                    if (value is Point)
                    {
                        var point = (Point)value;
                        return point.X + ":" + point.Y;
                    }

                    return base.ConvertTo(context, culture, value, destinationType);
                }

                public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
                {
                    if (value is string)
                    {
                        var stringValue = (string)value;

                        if (string.IsNullOrEmpty(stringValue))
                            return new Point();

                        var parts = ((string)value).Split(':');
                        return new Point { X = int.Parse(parts[0]), Y = int.Parse(parts[1]) };
                    }

                    return base.ConvertFrom(context, culture, value);
                }
            }
        }
    }
}