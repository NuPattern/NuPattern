using System;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
	public class BindingFactorySpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenNoContext
		{
			[TestMethod, TestCategory("Unit")]
			public void WhenCreatingNewWithNullCompositionService_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => new BindingFactory(null));
			}
		}

		[TestClass]
		public class GivenABinding
		{
			private const string FixedValue = "Fixed";
			private const string ValueProviderValue = "Provided";
			private BindingFactory target;

			[TestInitialize]
			public void Initialize()
			{
				var value = new Mock<IFoo>();
				value.SetupProperty(x => x.Message);
				var valueProvider = new Mock<IValueProvider>();
				valueProvider.Setup(x => x.Evaluate()).Returns(ValueProviderValue);

				var compositionService = new Mock<IBindingCompositionService>();
				compositionService.Setup(cs => cs.GetExports<IFoo, IFeatureComponentMetadata>())
					.Returns(new[]
					{
						new Lazy<IFoo, IFeatureComponentMetadata>(() => value.Object, 
							Mocks.Of<IFeatureComponentMetadata>().First(m => m.CatalogName ==  Microsoft.VisualStudio.TeamArchitect.PowerTools.Constants.CatalogName && m.Id == "Foo"))
					});

				compositionService.Setup(cs => cs.GetExports<IValueProvider, IFeatureComponentMetadata>())
					.Returns(new[]
					{
						new Lazy<IValueProvider, IFeatureComponentMetadata>(() => valueProvider.Object, 
							Mocks.Of<IFeatureComponentMetadata>().First(m => m.CatalogName == Microsoft.VisualStudio.TeamArchitect.PowerTools.Constants.CatalogName && m.Id == "ValueProvider"))
					});


				this.target = new BindingFactory(compositionService.Object);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenCreatingABinding_ThenReturnsBinding()
			{
				var binding = this.target.CreateBinding<IFeatureCommand>(Mocks.Of<IBindingSettings>().First(s => s.TypeId == "Foo"));

				Assert.NotNull(binding);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenPropertyBindingHasValue_ThenBindingResolvesToFixedValue()
			{
				var settings = new BindingSettings
				{
					TypeId = "Foo",
					Properties =  
					{
						new PropertyBindingSettings
						{
							Name = Reflector<IFoo>.GetProperty(x => x.Message).Name, 
							Value = FixedValue, 
							ValueProvider = new ValueProviderBindingSettings
							{
								TypeId = "ValueProvider"
							}
						}
					}
				};

				var binding = this.target.CreateBinding<IFoo>(settings);

				Assert.True(binding.Evaluate());
				Assert.Equal(FixedValue, binding.Value.Message);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenPropertyBindingHasEmpty_ThenBindingResolvesToValueProvider()
			{
				var settings = new BindingSettings
				{
					TypeId = "Foo",
					Properties =  
					{
						new PropertyBindingSettings
						{
							Name = Reflector<IFoo>.GetProperty(x => x.Message).Name, 
							Value = string.Empty, 
							ValueProvider = new ValueProviderBindingSettings
							{
								TypeId = "ValueProvider"
							}
						}
					}
				};

				var binding = this.target.CreateBinding<IFoo>(settings);

				Assert.True(binding.Evaluate());
				Assert.Equal(ValueProviderValue, binding.Value.Message);
			}

			public class Foo
			{
				string Message { get; set; }
			}

			public interface IFoo
			{
				string Message { get; set; }
			}
		}
	}
}