using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UnitTests
{
    [TestClass]
    public class PathResolverSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenNormalizingWithoutRelativeMove_ThenReturnsSameInput()
        {
            Assert.Equal(@"Foo\Bar\Baz", PathResolver.Normalize(@"Foo\Bar\Baz"));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenNormalizingWithRelativeMove_ThenPreservesNeededOnes()
        {
            Assert.Equal(@"..\..\Foo", PathResolver.Normalize(@"..\..\Foo"));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenNormalizingWithRelativeMove_ThenRemovesUnneededOnes()
        {
            // A\\B\\C\\[start]\\Foo\\ - 2
            // A\\B\\Bar\\ - 2
            // A\\Baz
            // == A\\B\\C\\[start] - 2 \\Baz
            Assert.Equal(@"..\..\Baz", PathResolver.Normalize(@"Foo\..\..\Bar\..\..\Baz"));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPathEndsWithSlash_ThenItIsRemoved()
        {
            var resolver = new PathResolver(Mock.Of<IProductElement>(), Mock.Of<IUriReferenceService>(), @"Folder\SubFolder\");

            resolver.Resolve();

            Assert.Equal(@"Folder\SubFolder", resolver.Path);
        }

        [TestClass]
        public class GivenAProductElementHierarchy
        {
            public Mock<IProductElement> Element { get; private set; }
            public Mock<IUriReferenceService> UriService { get; private set; }
            public PathResolver Resolver { get; private set; }
            public IProduct Root { get; private set; }

            [TestInitialize]
            public void Initialize()
            {
                this.UriService = new Mock<IUriReferenceService>();
                this.Root = Mock.Of<IProduct>(p =>
                        p.ProductState.GetService(typeof(IUriReferenceService)) == Mock.Of<IUriReferenceService>());

                this.Element = new Mock<IProductElement>();
                this.Element
                    .Setup(x => x.Root)
                    .Returns(this.Root);

                this.Resolver = new PathResolver(this.Element.Object, this.UriService.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTargetPathStartsWithTildeButNoAssetLink_ThenThrowsInvalidOperationException()
            {
                this.Resolver.Path = @"~\Foo";

                Assert.Throws<InvalidOperationException>(
                    Resources.PathResolver_ErrorNoAncestorWithArtifactLink,
                    () => this.Resolver.Resolve());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactLinkFailsToResolve_ThenThrowsInvalidOperationException()
            {
                this.Resolver.Path = @"~\Foo";

                this.Element
                    .Setup(x => x.References)
                    .Returns(new[] 
                    {
                        Mock.Of<IReference>(r => r.Kind == ReferenceKindConstants.SolutionItem && r.Value == "solution://root/Foo"),
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
                        Mock.Of<IReference>(r => r.Kind == ReferenceKindConstants.SolutionItem && r.Value == "solution://root/Bar"),
                    });

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(Mock.Of<IItemContainer>(item => item.Name == "Foo" &&
                        item.Parent == Mock.Of<ISolution>(parent => parent.Name == "Solution")));

                this.Resolver.Resolve();

                Assert.Equal(@"Foo\Bar", this.Resolver.Path);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMultipleArtifactLinkResolve_ThenCanFilterDesiredOne()
            {
                this.Resolver.Path = @"~\Fixed";

                this.Element
                    .Setup(x => x.References)
                    .Returns(new[] 
                    {
                        Mock.Of<IReference>(r => r.Kind == ReferenceKindConstants.SolutionItem && r.Value == "solution://root/Bar"),
                        Mock.Of<IReference>(r => r.Kind == ReferenceKindConstants.SolutionItem && r.Value == "solution://root/Baz"),
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

                Assert.Equal(@"Baz\Fixed", this.Resolver.Path);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathContainsProductRelativeParentThen_ThenResolvesArtifact()
            {
                this.Resolver.Path = @"..\..\~\Bar";

                this.Element.Setup(x => x.Parent).Returns(
                    Mock.Of<IProductElement>(parent =>
                        parent.Parent == Mock.Of<IProductElement>(ancestor =>
                            ancestor.References == new[] 
                            { 
                                Mock.Of<IReference>(r => 
                                    r.Owner == ancestor && 
                                    r.Kind == ReferenceKindConstants.SolutionItem && 
                                    r.Value == "solution://root/Bar"),
                            } &&
                            ancestor.Root == this.Root) &&
                        parent.Root == this.Root));

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(Mock.Of<IItemContainer>(item => item.Name == "Foo" &&
                        item.Parent == Mock.Of<ISolution>(parent => parent.Name == "Solution")));

                this.Resolver.Resolve();

                Assert.Equal(@"Foo\Bar", this.Resolver.Path);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathStartsWithSlash_ThenDoesNotResolveArtifactLink()
            {
                this.Resolver.Path = @"\Solution Items";

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Throws<InvalidOperationException>();

                this.Resolver.Resolve();

                Assert.Equal("Solution Items", this.Resolver.Path);
            }
        }

        [TestClass]
        public class GivenAProductElementWithRichHierarchy
        {
            private Mock<IProductElement> element;
            private Mock<IUriReferenceService> uriService;
            private PathResolver resolver;
            private IProduct root;
            private Mock<IAbstractElement> child;
            private Mock<IAbstractElement> descendant;
            private Mock<IProductElement> ancestor;
            private Mock<IProductElement> parent;

            [TestInitialize]
            public void Initialize()
            {
                this.uriService = new Mock<IUriReferenceService>();
                this.root = Mock.Of<IProduct>(p =>
                        p.ProductState.GetService(typeof(IUriReferenceService)) == Mock.Of<IUriReferenceService>());

                this.parent = new Mock<IProductElement>();
                parent.Setup(ae => ae.DefinitionName).Returns("parent");
                this.ancestor = new Mock<IProductElement>();
                ancestor.Setup(ae => ae.DefinitionName).Returns("ancestor");
                this.child = new Mock<IAbstractElement>();
                child.Setup(ae => ae.DefinitionName).Returns("child");
                child.Setup(c => c.Info.Cardinality).Returns(Cardinality.OneToMany);
                this.descendant = new Mock<IAbstractElement>();
                descendant.Setup(ae => ae.DefinitionName).Returns("descendant");
                descendant.Setup(d => d.Info.Cardinality).Returns(Cardinality.OneToMany);
                descendant.Setup(ae => ae.References).Returns(new[] 
                                        { 
                                            Mock.Of<IReference>(r => 
                                                r.Kind == ReferenceKindConstants.SolutionItem && 
                                                r.Value == "solution://root/Bar"),
                                        });

                child.As<IElementContainer>().Setup(ec => ec.Elements).Returns(new [] { descendant.Object });
                ancestor.As<IElementContainer>().Setup(ec => ec.Elements).Returns(new []{child.Object});
                parent.Setup(p => p.Parent).Returns(ancestor.Object);
                parent.Setup(p => p.Root).Returns(this.root);

                this.element = new Mock<IProductElement>();
                this.element.Setup(ae => ae.DefinitionName).Returns("element");
                this.element.Setup(x => x.Root).Returns(this.root);
                this.element.Setup(x => x.Parent).Returns(parent.Object);

                this.uriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(Mock.Of<IItemContainer>(item => item.Name == "Foo" &&
                        item.Parent == Mock.Of<ISolution>(p => p.Name == "Solution")));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathNavigatesDownAncestorHierarchyWithWrongCardinalityDescendant_ThenThrows()
            {
                this.resolver = new PathResolver(this.element.Object, this.uriService.Object);
                this.resolver.Path = @"..\..\child\descendant\~\Bar";

                Assert.Throws<InvalidOperationException>(()=> this.resolver.Resolve());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathNavigatesDownAncestorHierarchyWithSingleCardinalityDescendant_ThenResolvesArtifactLink()
            {
                this.resolver = new PathResolver(this.element.Object, this.uriService.Object);
                this.resolver.Path = @"..\..\child\descendant\~\Bar";

                this.child.Setup(c => c.Info.Cardinality).Returns(Cardinality.OneToOne);
                this.descendant.Setup(d => d.Info.Cardinality).Returns(Cardinality.OneToOne);

                this.resolver.Resolve();

                Assert.Equal(@"Foo\Bar", this.resolver.Path);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathNavigatesDownHierarchyWithSingleCardinalityDescendant_ThenResolvesArtifactLink()
            {
                this.resolver = new PathResolver(this.ancestor.Object, this.uriService.Object);
                this.resolver.Path = @"child\descendant\~\Bar";

                this.child.Setup(c => c.Info.Cardinality).Returns(Cardinality.OneToOne);
                this.descendant.Setup(d => d.Info.Cardinality).Returns(Cardinality.OneToOne);

                this.resolver.Resolve();

                Assert.Equal(@"Foo\Bar", this.resolver.Path);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathNavigatesDownHierarchyWithSingleCardinalityDescendant2_ThenResolvesArtifactLink()
            {
                this.resolver = new PathResolver(this.ancestor.Object, this.uriService.Object);
                this.resolver.Path = @".\child\descendant\~\Bar";

                this.child.Setup(c => c.Info.Cardinality).Returns(Cardinality.OneToOne);
                this.descendant.Setup(d => d.Info.Cardinality).Returns(Cardinality.OneToOne);

                this.resolver.Resolve();

                Assert.Equal(@"Foo\Bar", this.resolver.Path);
            }
        }

        [TestClass]
        public class GivenAProductElementWithMultipleReferences
        {
            private Mock<IProductElement> element;
            private Mock<IUriReferenceService> uriService;
            private PathResolver resolver;
            private IProduct root;
            private Mock<IProductElement> ancestor;
            private Mock<IProductElement> parent;

            [TestInitialize]
            public void Initialize()
            {
                this.uriService = new Mock<IUriReferenceService>();
                this.root = Mock.Of<IProduct>(p =>
                        p.ProductState.GetService(typeof(IUriReferenceService)) == Mock.Of<IUriReferenceService>());

                this.parent = new Mock<IProductElement>();
                parent.Setup(ae => ae.DefinitionName).Returns("parent");
                this.ancestor = new Mock<IProductElement>();
                ancestor.Setup(ae => ae.DefinitionName).Returns("ancestor");
                ancestor.Setup(ae => ae.References).Returns(new[] 
                                        { 
                                            Mock.Of<IReference>(r => 
                                                r.Kind == ReferenceKindConstants.SolutionItem && 
                                                r.Value == "solution://root/Foo" &&
                                                r.Tag == "foo"),
                                            Mock.Of<IReference>(r => 
                                                r.Kind == ReferenceKindConstants.SolutionItem && 
                                                r.Value == "solution://root/Bar" &&
                                                r.Tag == "bar,bar5,bar8"),
                                        });
                parent.Setup(p => p.Parent).Returns(ancestor.Object);
                parent.Setup(p => p.Root).Returns(this.root);

                this.element = new Mock<IProductElement>();
                this.element.Setup(ae => ae.DefinitionName).Returns("element");
                this.element.Setup(x => x.Root).Returns(this.root);
                this.element.Setup(x => x.Parent).Returns(parent.Object);

                this.uriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.Is<Uri>(u => u.ToString() == "solution://root/Foo")))
                    .Returns(Mock.Of<IItemContainer>(item => item.Name == "Foo" &&
                        item.Parent == Mock.Of<ISolution>(p => p.Name == "Solution")));
                this.uriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.Is<Uri>(u => u.ToString() == "solution://root/Bar")))
                    .Returns(Mock.Of<IItemContainer>(item => item.Name == "Bar" &&
                        item.Parent == Mock.Of<ISolution>(p => p.Name == "Solution")));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathReferencesNoTag_ThenResolvesFirstArtifactLink()
            {
                this.resolver = new PathResolver(this.element.Object, this.uriService.Object);
                this.resolver.Path = @"..\..\~\Bar2";

                this.resolver.Resolve();

                Assert.Equal(@"Foo\Bar2", this.resolver.Path);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathReferencesTag_ThenResolvesToSpecificArtifactLink()
            {
                this.resolver = new PathResolver(this.element.Object, this.uriService.Object);
                this.resolver.Path = @"..\..\~[bar5]\Bar2";

                this.resolver.Resolve();

                Assert.Equal(@"Bar\Bar2", this.resolver.Path);
            }
        }

        [TestClass]
        public class GivenVariousPaths
        {
            [TestInitialize]
            public void Initialize()
            {
                
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathBlank_ThenValid()
            {
                Assert.True(IsValidExpression(@""));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathStartsWithTilda_Then()
            {
                Assert.True(IsValidExpression(@"~"));
                Assert.True(IsValidExpression(@"~\"));
                Assert.True(IsValidExpression(@"~/"));
                Assert.True(IsValidExpression(@"~Project Folder"));
                Assert.True(IsValidExpression(@"~ProjectFolder"));
                Assert.True(IsValidExpression(@"~ProjectFolder1\ProjectFolder2"));
                Assert.True(IsValidExpression(@"~\ProjectFolder"));
                Assert.True(IsValidExpression(@"~/ProjectFolder"));
                Assert.True(IsValidExpression(@"~\ProjectFolder1\ProjectFolder2\ProjectFolder3"));
                Assert.True(IsValidExpression(@"~\ProjectFolder1\{Substitution.Substitution}\{Substitution}Substitution"));
                Assert.True(IsValidExpression(@"~\..\..\ProjectFolder"));
                Assert.True(IsValidExpression(@"~\.\..\ProjectFolder\{Substitution}"));
                Assert.False(IsValidExpression(@"~[]"));
                Assert.False(IsValidExpression(@"~[{Substitution}]"));
                Assert.True(IsValidExpression(@"~[tag1]"));
                Assert.True(IsValidExpression(@"~[tag1;tag2,tag3-4_9]"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathHasNoTilda_Then()
            {
                Assert.True(IsValidExpression(@"\"));
                Assert.True(IsValidExpression(@"/"));
                Assert.True(IsValidExpression(@"SolutionFolder"));
                Assert.True(IsValidExpression(@"SolutionFolder\SolutionFolder\SolutionFolder"));
                Assert.True(IsValidExpression(@"SolutionFolder\..\SolutionFolder"));
                Assert.True(IsValidExpression(@"\SolutionFolder"));
                Assert.True(IsValidExpression(@"\Solution Folder"));
                Assert.True(IsValidExpression(@"/SolutionFolder"));
                Assert.True(IsValidExpression(@"\SolutionFolder\SolutionFolder\SolutionFolder"));
                Assert.True(IsValidExpression(@".\"));
                Assert.True(IsValidExpression(@"./"));
                Assert.True(IsValidExpression(@"..\"));
                Assert.True(IsValidExpression(@"../"));
                Assert.True(IsValidExpression(@"..\..\"));
                Assert.True(IsValidExpression(@"..\..\Element1\Element2"));
                Assert.True(IsValidExpression(@"../../Element1/Element2"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPathHasTilda_Then()
            {
                Assert.True(IsValidExpression(@"..\~"));
                Assert.True(IsValidExpression(@"../~"));
                Assert.True(IsValidExpression(@"../~..\ProjectFolder"));
                Assert.True(IsValidExpression(@"../~.\"));
                Assert.True(IsValidExpression(@"..\~ProjectFolder"));
                Assert.True(IsValidExpression(@"..\~\ProjectFolder"));
                Assert.True(IsValidExpression(@"..\~\ProjectFolder1\ProjectFolder2"));
                Assert.True(IsValidExpression(@"..\Element1\~\ProjectFolder1"));
                Assert.True(IsValidExpression(@".\~{Substitution}{Substitution}\ParentProjectFolder1"));
                Assert.True(IsValidExpression(@".\~\{Substitution}{Substitution}\..\ParentProjectFolder1"));
            }

            private bool IsValidExpression(string expression)
            {
                return Regex.IsMatch(expression, PathResolver.PathRegExpression);
            }
        }
    }
}
