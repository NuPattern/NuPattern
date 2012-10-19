using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility.References;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Patterning.Runtime.Store;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Commands
{
    public class GenerateProductCodeCommandSpec
    {
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        [TestClass]
        public class GivenAProductStore
        {
            internal static readonly IAssertion Assert = new Assertion();

            internal DslTestStore<ProductStateStoreDomainModel> Store { get; private set; }
            protected IProduct Product { get; private set; }
            protected IView View { get; private set; }
            protected ICollection Collection { get; private set; }
            protected IElement Element { get; private set; }
            protected ISolution Solution { get; private set; }
            protected Mock<IPatternManager> PatternManager { get; private set; }
            protected Mock<IFxrUriReferenceService> UriService { get; private set; }
            protected Mock<ITemplate> Template { get; private set; }
            protected Mock<TraceListener> Listener { get; private set; }
            protected GenerateProductCodeCommand Command { get; private set; }

            [TestInitialize]
            public virtual void Initialize()
            {
                this.Store = new DslTestStore<ProductStateStoreDomainModel>();
                this.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    var productStore = this.Store.ElementFactory.CreateElement<ProductState>();
                    this.Product = productStore.CreateProduct(prod => prod.InstanceName = "Pattern");
                    this.View = this.Product.CreateView();
                    this.Collection = this.View.CreateCollection(x => x.InstanceName = "Collection");
                    this.Element = this.Collection.CreateElement(x => x.InstanceName = "CurrentElement");
                });

                this.Solution = new Solution
                {
                    Name = "Solution",
                    Items = 
                    {
                        new SolutionFolder
                        {
                            Name = "Solution Items", 
                            Items = 
                            {
                                new Item
                                {
                                    Name = "Foo.cs",
                                },
                                new Item
                                {
                                    Name = "Bar.cs",
                                }
                            }
                        }
                    }
                };

                this.PatternManager = new Mock<IPatternManager>();
                this.UriService = new Mock<IFxrUriReferenceService>();
                this.Listener = new Mock<TraceListener>();

                this.Command = new Mock<GenerateProductCodeCommand> { CallBase = true }.Object;
                this.Command.PatternManager = this.PatternManager.Object;
                this.Command.CurrentElement = this.Element;
                this.Command.ModelElement = this.Element as ModelElement;
                this.Command.ModelFile = "TestModelFile";
                this.Command.ServiceProvider = new Mock<IServiceProvider>().Object;
                this.Command.Settings = Mock.Of<ICommandSettings>(x => x.Id == Guid.NewGuid());
                this.Command.Solution = this.Solution;
                this.Command.TargetFileName = "Bar.cs";
                this.Command.TargetPath = "Solution Items";
                this.Command.TemplateUri = new Uri("template://foo/bar");
                this.Command.UriService = this.UriService.Object;

                this.Template = new Mock<ITemplate>();
                this.Template
                    .Setup(x => x.Unfold(It.IsAny<string>(), It.IsAny<IItemContainer>()))
                    .Returns(this.Solution.Traverse().OfType<IItem>().First(i => i.Name == this.Command.TargetFileName));

                this.UriService.Setup(x => x.ResolveUri<ITemplate>(It.Is<Uri>(u => u.Scheme == "template")))
                    .Returns(this.Template.Object);
                this.UriService.Setup(x => x.CreateUri(It.IsAny<IItem>(), null)).Returns(new Uri("solution://folder/solution items/foo.cs"));

                Mock.Get(this.Command).Protected().Setup<string>("SerializeReference").Returns("foo://bar");

                Tracer.AddListener(Tracer.GetSourceNameFor<GenerateProductCodeCommand>(), this.Listener.Object);
                Tracer.GetOrCreateUnderlyingSource(Tracer.GetSourceNameFor<GenerateProductCodeCommand>()).Switch.Level = SourceLevels.All;
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.Store.Dispose();
                Tracer.RemoveListener(Tracer.GetSourceNameFor<SynchArtifactNameCommand>(), this.Listener.Object);
            }

            [TestMethod]
            public void WhenPatternManagerIsNotOpen_ThenThrowsInvalidOperationException()
            {
                Mock.Get(this.Command.PatternManager).Setup(x => x.IsOpen).Returns(false);

                Assert.Throws<InvalidOperationException>(Resources.GenerateProductCodeCommand_PatternManagerNotOpen, () => Command.Execute());
            }
        }

        [TestClass]
        public class GivenAnOpenedPatternManager : GivenAProductStore
        {
            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.PatternManager.Setup(x => x.IsOpen).Returns(true);
                this.PatternManager.Setup(x => x.StoreFile).Returns("C:\\Foo" + Microsoft.VisualStudio.Patterning.Runtime.Constants.RuntimeStoreExtension);
            }

            [TestMethod]
            public void WhenExecuting_ThenSetsModelElementToImportedModelInstance()
            {
                Mock.Get(this.Command.PatternManager).Setup(x => x.IsOpen).Returns(true);

                this.Command.CurrentElement = this.Product;

                Command.Execute();
                Assert.Same(this.Product, Command.ModelElement);
            }

            [TestMethod]
            public void WhenExecuting_ThenSetsModelFileToProductStoreFile()
            {
                Command.Execute();

                Assert.Equal("C:\\Foo" + Microsoft.VisualStudio.Patterning.Runtime.Constants.RuntimeStoreExtension, Command.ModelFile);
            }

            [TestMethod]
            public void WhenExecuting_ThenAddsArtifactReference()
            {
                this.UriService
                    .Setup(x => x.ResolveUri<IItem>(It.IsAny<Uri>()))
                    .Returns((IItem)null);

                Command.Execute();

                Assert.Equal(1, this.Element.References.Count());
            }

            [TestMethod]
            public void WhenExecuting_ThenSavesValueProvidedProperties()
            {
                var prop = this.Product.CreateProperty(p =>
                    ((Property)p).Info = Mock.Of<IPropertyInfo>(x => x.Name == "Foo" && x.Type == "System.String" && x.ValueProvider.TypeId == "Foo"));

                Mock.Get(this.Store.ServiceProvider)
                    .Setup(x => x.GetService(typeof(IBindingFactory)))
                    .Returns(Mock.Of<IBindingFactory>(factory =>
                        factory.CreateContext() == Mock.Of<IDynamicBindingContext>() &&
                        factory.CreateBinding<IValueProvider>(It.IsAny<IBindingSettings>()) ==
                            Mock.Of<IDynamicBinding<IValueProvider>>(binding =>
                                binding.Evaluate(It.IsAny<IDynamicBindingContext>()) == true &&
                                binding.Value == Mock.Of<IValueProvider>(provider =>
                                    provider.Evaluate() == (object)"Hello"))));

                Command.Execute();

                Assert.Equal("Hello", prop.RawValue);
            }
        }

        [TestClass]
        public class GivenAnElementWithAnArtifactLinkReference : GivenAnOpenedPatternManager
        {
            private IReference reference;

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.reference = SolutionArtifactLinkReference.AddReference(this.Element, new Uri("solution://foo/bar"));
                this.reference.Tag = this.Command.Settings.Id.ToString();
            }

            [TestMethod]
            public void WhenExistingFileResolvesAndSyncNameFalse_ThenCurrentFilenameAndLinkReused()
            {
                var item = this.Solution.Traverse().OfType<IItem>().First(i => i.Name == "Bar.cs");
                Assert.Equal("Bar.cs", item.Name);

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(item);

                Command.Execute();

                Assert.Equal("Bar.cs", Command.GeneratedItem.Name);
                Assert.Equal(1, this.Element.References.Count());
                Assert.Equal("solution://foo/bar", this.Element.References.First().Value);
            }

            [TestMethod]
            public void WhenExistingFileNotResolvedAndSyncNameFalse_ThenNewFilenameAndReferenceCreated()
            {
                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns((IItem)null);

                Command.Execute();

                Assert.Equal("Bar.cs", Command.GeneratedItem.Name);
                Assert.Equal(1, this.Element.References.Count());
                Assert.Equal("solution://folder/solution items/foo.cs", this.Element.References.First().Value);
            }

            [TestMethod]
            public void WhenExistingFileResolvesAndSyncNameTrueAndFileNameDifferent_ThenFileRenamed()
            {
                var parent = this.Solution.Traverse().OfType<IItem>().First().Parent;
                Assert.Equal("Solution Items", parent.Name);

                var vsItem = new Mock<EnvDTE.ProjectItem>();
                var item = new Mock<IItem>();
                item.Setup(i => i.Name).Returns("Bar.cs");
                item.Setup(i => i.Kind).Returns(ItemKind.Item);
                item.Setup(i => i.Parent).Returns(parent);
                item.Setup(i => i.As<EnvDTE.ProjectItem>()).Returns(vsItem.Object);

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(item.Object);

                this.Template
                    .Setup(x => x.Unfold(It.IsAny<string>(), It.IsAny<IItemContainer>()))
                    .Returns(item.Object);

                this.Element.InstanceName = "Banana";
                this.Command.SyncName = true;
                this.Command.TargetFileName = "{InstanceName}";

                Command.Execute();

                //Assert.Equal("Foo.cs", Command.GeneratedItem.Name);
                vsItem.VerifySet(v => v.Name = "Banana.cs");
                Assert.Equal(1, this.Element.References.Count());
                Assert.Equal("solution://foo/bar", this.Element.References.First().Value);
            }

            [TestMethod]
            public void WhenExistingFileResolvesAndSyncNameTrueAndFileNameSame_ThenFileNotRenamed()
            {
                var parent = this.Solution.Traverse().OfType<IItem>().First().Parent;
                Assert.Equal("Solution Items", parent.Name);

                var vsItem = new Mock<EnvDTE.ProjectItem>();
                var item = new Mock<IItem>();
                item.Setup(i => i.Name).Returns("Banana.cs");
                item.Setup(i => i.Kind).Returns(ItemKind.Item);
                item.Setup(i => i.Parent).Returns(parent);
                item.Setup(i => i.As<EnvDTE.ProjectItem>()).Returns(vsItem.Object);

                this.UriService
                    .Setup(x => x.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(item.Object);

                this.Template
                    .Setup(x => x.Unfold(It.IsAny<string>(), It.IsAny<IItemContainer>()))
                    .Returns(item.Object);

                this.Element.InstanceName = "Banana";
                this.Command.SyncName = true;
                this.Command.TargetFileName = "{InstanceName}.cs";

                Command.Execute();

                //Assert.Equal("Foo.cs", Command.GeneratedItem.Name);
                vsItem.VerifySet(v => v.Name = It.IsAny<string>(), Times.Never());
                Assert.Equal(1, this.Element.References.Count());
                Assert.Equal("solution://foo/bar", this.Element.References.First().Value);

                //Do it again without file extension specified
                this.Command.TargetFileName = "{InstanceName}";

                Command.Execute();

                vsItem.VerifySet(v => v.Name = It.IsAny<string>(), Times.Never());
            }
        }
    }
}
