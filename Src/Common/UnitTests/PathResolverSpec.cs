using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Properties;
using NuPattern.Extensibility.References;
using NuPattern.Runtime;

namespace NuPattern.Common.UnitTests
{
    [TestClass]
    public class PathResolverSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenNormalizingWithoutRelativeMove_ThenReturnsSameInput()
        {
            Assert.Equal("Foo\\Bar\\Baz", PathResolver.Normalize("Foo\\Bar\\Baz"));
        }


        [TestMethod, TestCategory("Unit")]
        public void WhenNormalizingWithRelativeMove_ThenPreservesNeededOnes()
        {
            Assert.Equal("..\\..\\Foo", PathResolver.Normalize("..\\..\\Foo"));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenNormalizingWithRelativeMove_ThenRemovesUnneededOnes()
        {
            // A\\B\\C\\[start]\\Foo\\ - 2
            // A\\B\\Bar\\ - 2
            // A\\Baz
            // == A\\B\\C\\[start] - 2 \\Baz
            Assert.Equal("..\\..\\Baz", PathResolver.Normalize("Foo\\..\\..\\Bar\\..\\..\\Baz"));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPathEndsWithSlash_ThenItIsRemoved()
        {
            var resolver = new PathResolver(Mock.Of<IProductElement>(), Mock.Of<IFxrUriReferenceService>(), "Folder\\SubFolder\\");

            resolver.Resolve();

            Assert.Equal("Folder\\SubFolder", resolver.Path);
        }

        [TestClass]
        public class GivenAProductElementHierarchy
        {
            public Mock<IProductElement> Element { get; private set; }
            public Mock<IFxrUriReferenceService> UriService { get; private set; }
            public PathResolver Resolver { get; private set; }
            public IProduct Root { get; private set; }

            [TestInitialize]
            public void Initialize()
            {
                this.UriService = new Mock<IFxrUriReferenceService>();
                this.Root = Mock.Of<IProduct>(p =>
                        p.ProductState.GetService(typeof(IFxrUriReferenceService)) == Mock.Of<IFxrUriReferenceService>());

                this.Element = new Mock<IProductElement>();
                this.Element
                    .Setup(x => x.Root)
                    .Returns(this.Root);

                this.Resolver = new PathResolver(this.Element.Object, this.UriService.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTargetPathStartsWithTildeButNoAssetLink_ThenThrowsInvalidOperationException()
            {
                this.Resolver.Path = "~\\Foo";

                Assert.Throws<InvalidOperationException>(
                    Resources.PathResolver_ErrorNoAncestorWithArtifactLink,
                    () => this.Resolver.Resolve());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactLinkFailsToResolve_ThenThrowsInvalidOperationException()
            {
                this.Resolver.Path = "~\\Foo";

                this.Element
                    .Setup(x => x.References)
                    .Returns(new[] 
					{
						Mock.Of<IReference>(r => r.Kind == ReferenceKindConstants.ArtifactLink && r.Value == "solution://root/Foo"),
					});

                Assert.Throws<InvalidOperationException>(
                    Resources.PathResolver_ErrorInvalidArtifactLink,
                    () => this.Resolver.Resolve());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactLinkResolves_ThenPrependsToTargetPath()
            {
                this.Resolver.Path = "~\\Bar";

                this.Element
                    .Setup(x => x.References)
                    .Returns(new[] 
					{
						Mock.Of<IReference>(r => r.Kind == ReferenceKindConstants.ArtifactLink && r.Value == "solution://root/Bar"),
					});

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(Mock.Of<IItemContainer>(item => item.Name == "Foo" &&
                        item.Parent == Mock.Of<ISolution>(parent => parent.Name == "Solution")));

                this.Resolver.Resolve();

                Assert.Equal("Foo\\Bar", this.Resolver.Path);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMultipleArtifactLinkResolve_ThenCanFilterDesiredOne()
            {
                this.Resolver.Path = "~\\Fixed";

                this.Element
                    .Setup(x => x.References)
                    .Returns(new[] 
					{
						Mock.Of<IReference>(r => r.Kind == ReferenceKindConstants.ArtifactLink && r.Value == "solution://root/Bar"),
						Mock.Of<IReference>(r => r.Kind == ReferenceKindConstants.ArtifactLink && r.Value == "solution://root/Baz"),
					});

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(new Uri("solution://root/Bar")))
                    .Returns(Mock.Of<IItemContainer>(item => item.Name == "Bar" &&
                        item.Kind == ItemKind.Item &&
                        item.Parent == Mock.Of<ISolution>(parent => parent.Name == "Solution")));
                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(new Uri("solution://root/Baz")))
                    .Returns(Mock.Of<IItemContainer>(item => item.Name == "Baz" &&
                        item.Kind == ItemKind.Folder &&
                        item.Parent == Mock.Of<ISolution>(parent => parent.Name == "Solution")));

                this.Resolver.Resolve(item => item.Kind == ItemKind.Folder);

                Assert.Equal("Baz\\Fixed", this.Resolver.Path);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathContainsProductRelativeParentThen_ThenResolvesArtifact()
            {
                this.Resolver.Path = "..\\..\\~\\Bar";

                this.Element.Setup(x => x.Parent).Returns(
                    Mock.Of<IProductElement>(parent =>
                        parent.Parent == Mock.Of<IProductElement>(ancestor =>
                            ancestor.References == new[] 
							{ 
								Mock.Of<IReference>(r => 
									r.Owner == ancestor && 
									r.Kind == ReferenceKindConstants.ArtifactLink && 
									r.Value == "solution://root/Bar"),
							} &&
                            ancestor.Root == this.Root) &&
                        parent.Root == this.Root));

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(Mock.Of<IItemContainer>(item => item.Name == "Foo" &&
                        item.Parent == Mock.Of<ISolution>(parent => parent.Name == "Solution")));

                this.Resolver.Resolve();

                Assert.Equal("Foo\\Bar", this.Resolver.Path);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathStartsWithSlash_ThenDoesNotResolveArtifactLink()
            {
                this.Resolver.Path = "\\Solution Items";

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Throws<InvalidOperationException>();

                this.Resolver.Resolve();

                Assert.Equal("Solution Items", this.Resolver.Path);
            }
        }
    }
}
