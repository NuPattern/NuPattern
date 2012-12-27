using System;
using System.ComponentModel.Composition.Primitives;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NuPattern.Library.IntegrationTests
{
	public class BindingFactorySpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenNoContext
		{
			[TestMethod]
			public void WhenCreatingNewWithNullCompositionService_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => new BindingFactory(null));
			}
		}

		[TestClass]
		public class GivenACommandBinding
		{
			private BindingFactory target;

			[TestInitialize]
			public void Initialize()
			{
				var commandMetadata = new Mock<IFeatureComponentMetadata>();
				commandMetadata.Setup(m => m.CatalogName).Returns(Constants.CatalogName);
				commandMetadata.Setup(m => m.Id).Returns("Foo");

				var compositionService = new Mock<IFeatureCompositionService>();
				compositionService.Setup(cs => cs.SatisfyImportsOnce(It.IsAny<ComposablePart>()));
				compositionService.Setup(cs => cs.GetExports<IFeatureCommand, IFeatureComponentMetadata>())
					.Returns(new[]
					{
						new Lazy<IFeatureCommand, IFeatureComponentMetadata>(() => new Mock<IFeatureCommand>().Object, commandMetadata.Object)
					});

				this.target = new BindingFactory(compositionService.Object);
			}

			[TestMethod]
			public void WhenCreatingABinding_ThenReturnsBinding()
			{
				var binding = this.target.Create<IFeatureCommand>(Mocks.First<IBindingSettings>(s => s.Type == "Foo"));

				Assert.NotNull(binding);
			}
		}
	}
}