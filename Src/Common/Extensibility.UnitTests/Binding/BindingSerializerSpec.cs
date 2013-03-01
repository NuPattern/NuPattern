using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility.Binding;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.UnitTests
{
    [TestClass]
    public class BindingSerializerSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenSerialingAnObjectWithAStringPropertyWithBrackets_ThenItDeserializesItProperly()
        {
            var foos = new List<Foo> { new Foo { Bar = 1, Baz = "{Baz1}" }, new Foo { Bar = 2, Baz = "{Baz2}" } };

            var ser = BindingSerializer.Serialize(foos);

            var dserFoos = BindingSerializer.Deserialize<List<Foo>>(ser);

            Assert.Equal(dserFoos.Count, foos.Count);
            Assert.Equal(dserFoos.ElementAt(0).Bar, foos.ElementAt(0).Bar);
            Assert.Equal(dserFoos.ElementAt(0).Baz, foos.ElementAt(0).Baz);
            Assert.Equal(dserFoos.ElementAt(1).Bar, foos.ElementAt(1).Bar);
            Assert.Equal(dserFoos.ElementAt(1).Baz, foos.ElementAt(1).Baz);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenSerialingAnObjectWithAStringProperty_ThenItDeserializesItProperly()
        {
            var foos = new List<Foo> { new Foo { Bar = 1, Baz = "Baz1" }, new Foo { Bar = 2, Baz = "Baz2" } };

            var ser = BindingSerializer.Serialize(foos);

            var dserFoos = BindingSerializer.Deserialize<List<Foo>>(ser);

            Assert.Equal(dserFoos.Count, foos.Count);
            Assert.Equal(dserFoos.ElementAt(0).Bar, foos.ElementAt(0).Bar);
            Assert.Equal(dserFoos.ElementAt(0).Baz, foos.ElementAt(0).Baz);
            Assert.Equal(dserFoos.ElementAt(1).Bar, foos.ElementAt(1).Bar);
            Assert.Equal(dserFoos.ElementAt(1).Baz, foos.ElementAt(1).Baz);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenRoundTrippingFullBinding_ThenSucceeds()
        {
            IBindingSettings binding = new BindingSettings
                {
                    TypeId = "foo",
                };
            var propBinding = binding.AddProperty("Name", typeof(string));
            propBinding.Value = "Value";
            propBinding.ValueProvider = new ValueProviderBindingSettings
            {
                TypeId = "ValueProvider",
            };
            var vpPropBinding = propBinding.ValueProvider.AddProperty("Id", typeof(string));
            vpPropBinding.Value = "1";

            var serialized = BindingSerializer.Serialize(binding);
            var deserialized = BindingSerializer.Deserialize<IBindingSettings>(serialized);

            Assert.Equal(binding.TypeId, deserialized.TypeId);
            Assert.Equal(binding.Properties.Count(), deserialized.Properties.Count());
            Assert.Equal(binding.Properties.First().Name, deserialized.Properties.First().Name);
            Assert.Equal(binding.Properties.First().Value, deserialized.Properties.First().Value);
            Assert.Equal(binding.Properties.First().ValueProvider.TypeId, deserialized.Properties.First().ValueProvider.TypeId);
            Assert.Equal(binding.Properties.First().ValueProvider.Properties.First().Name, deserialized.Properties.First().ValueProvider.Properties.First().Name);
            Assert.Equal(binding.Properties.First().ValueProvider.Properties.First().Value, deserialized.Properties.First().ValueProvider.Properties.First().Value);
        }

        public class Foo
        {
            public int Bar { get; set; }

            public string Baz { get; set; }
        }
    }
}