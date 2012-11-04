using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
	[TestClass]
	public class BindingSerializerSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
		public void WhenRoundTrippingFullBinding_ThenSucceeds()
		{
			IBindingSettings binding = new BindingSettings
			{
				TypeId = "foo",
				Properties = 
				{
					new PropertyBindingSettings
					{
						Name = "Name", 
						Value = "Value", 
						ValueProvider = new ValueProviderBindingSettings
						{
							TypeId = "ValueProvider",
							Properties = 
							{
								new PropertyBindingSettings
								{
									Name = "Id", 
									Value = "1"
								}
							}
						}
					}
				}
			};

			var serialized = BindingSerializer.Serialize(binding);
			var deserialized = BindingSerializer.Deserialize<IBindingSettings>(serialized);

			Assert.Equal(binding.TypeId, deserialized.TypeId);
			Assert.Equal(binding.Properties.Count, deserialized.Properties.Count);
			Assert.Equal(binding.Properties[0].Name, deserialized.Properties[0].Name);
			Assert.Equal(binding.Properties[0].Value, deserialized.Properties[0].Value);
			Assert.Equal(binding.Properties[0].ValueProvider.TypeId, deserialized.Properties[0].ValueProvider.TypeId);
			Assert.Equal(binding.Properties[0].ValueProvider.Properties[0].Name, deserialized.Properties[0].ValueProvider.Properties[0].Name);
			Assert.Equal(binding.Properties[0].ValueProvider.Properties[0].Value, deserialized.Properties[0].ValueProvider.Properties[0].Value);
		}

		public class Foo
		{
			public int Bar { get; set; }

			public string Baz { get; set; }
		}
	}
}