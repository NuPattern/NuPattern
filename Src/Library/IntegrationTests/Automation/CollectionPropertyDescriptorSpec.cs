using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Moq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Patterning.Library.IntegrationTests
{
	public class CollectionPropertyDescriptorSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenTwoModels
		{
			private Collection<Model> models;

			public GivenTwoModels()
			{
				this.models = new Collection<Model>
				{
					new Model
					{
						Properties = 
						{
							new Property { Value = "Value" }, 
							new Property 
							{
								ValueProvider = new ValueProvider
								{
									Properties = { new Property { Value = "C:\\" } }
								}
							}
						}
					},
					new Model 
					{
						Properties = 
						{
							new Property { Value = "Value" }, 
							new Property 
							{
								ValueProvider = new ValueProvider
								{
									Properties = { new Property { Value = "C:\\" } }
								}
							}
						}
					},
				};
			}

			[TestMethod]
			public void WhenSettingValue_ThenCanRoundtrip()
			{
				var property = new Mock<PropertyDescriptor>("foo", new Attribute[0]);
				property.Setup(x => x.Name).Returns("Property");
				property.Setup(x => x.Attributes).Returns(new AttributeCollection());

				string serializedValue = null;
				property
					.Setup(x => x.SetValue(It.IsAny<object>(), It.IsAny<object>()))
					.Callback<object, object>((component, value) => serializedValue = (string)value);
				property
					.Setup(x => x.GetValue(It.IsAny<object>()))
					.Returns(() => serializedValue);

				var descriptor = new CollectionPropertyDescriptor<Model>(property.Object);

				descriptor.SetValue(new object(), this.models);

				var actual = (Collection<Model>)descriptor.GetValue(new object());

				Assert.Equal(this.models.Count, actual.Count);
			}

			[TestMethod]
			public void WhenSettingValue_ThenSerializesToJson()
			{
				var property = new Mock<PropertyDescriptor>("foo", new Attribute[0]);
				property.Setup(x => x.Name).Returns("Property");
				property.Setup(x => x.Attributes).Returns(new AttributeCollection());

				string serializedValue = null;
				property
					.Setup(x => x.SetValue(It.IsAny<object>(), It.IsAny<object>()))
					.Callback<object, object>((component, value) => serializedValue = (string)value);

				var descriptor = new CollectionPropertyDescriptor<Model>(property.Object);

				descriptor.SetValue(new object(), this.models);

				var conditions = BindingSerializer.Deserialize<Collection<Model>>(serializedValue);

				Assert.Equal(2, conditions.Count);
			}

			[TestMethod]
			public void WhenValueIsNull_ThenCannotReset()
			{
				var property = new Mock<PropertyDescriptor>("foo", new Attribute[0]);
				property.Setup(x => x.Name).Returns("Property");
				property.Setup(x => x.Attributes).Returns(new AttributeCollection());
				property.Setup(x => x.GetValue(It.IsAny<object>())).Returns(null);

				var descriptor = new CollectionPropertyDescriptor<Model>(property.Object);

				Assert.False(descriptor.CanResetValue(new object()));
			}

			[TestMethod]
			public void WhenValueIsNotNull_ThenCanReset()
			{
				var property = new Mock<PropertyDescriptor>("foo", new Attribute[0]);
				property.Setup(x => x.Name).Returns("Property");
				property.Setup(x => x.Attributes).Returns(new AttributeCollection());
				property.Setup(x => x.GetValue(It.IsAny<object>())).Returns("{ }");

				var descriptor = new CollectionPropertyDescriptor<Model>(property.Object);

				Assert.True(descriptor.CanResetValue(new object()));
			}

			[TestMethod]
			public void WhenResettingValue_ThenSetsItToNull()
			{
				var property = new Mock<PropertyDescriptor>("foo", new Attribute[0]);
				property.Setup(x => x.Name).Returns("Property");
				property.Setup(x => x.Attributes).Returns(new AttributeCollection());
				property.Setup(x => x.GetValue(It.IsAny<object>())).Returns("{ }");

				var descriptor = new CollectionPropertyDescriptor<Model>(property.Object);

				descriptor.ResetValue(new object());

				property.Verify(x => x.SetValue(It.IsAny<object>(), null));
			}
		}

		public class Model
		{
			public Model()
			{
				this.Properties = new Collection<Property>();
			}

			public Collection<Property> Properties { get; private set; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property")]
		public class Property
		{
			public string Value { get; set; }
			public ValueProvider ValueProvider { get; set; }
		}

		public class ValueProvider
		{
			public ValueProvider()
			{
				this.Properties = new List<Property>();
			}

			public List<Property> Properties { get; private set; }
		}
	}
}