using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Commands;
using NuPattern.Modeling;
using NuPattern.Runtime;
using NuPattern.Runtime.Store;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.UnitTests.Commands
{
    public class GenerateModelingCodeCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        [TestClass]
        public class GivenASolution
        {
            private Mock<IUriReferenceService> uriService;
            private Mock<ITemplate> template;
            private Mock<IServiceProvider> serviceProvider;
            private Mock<IModelBus> modelBus;
            private DslTestStore<ProductStateStoreDomainModel> store;
            private ISolution solution;
            private IProduct product;
            private GenerateModelingCodeCommand command;

            [TestInitialize]
            public void Initialize()
            {
                this.template = new Mock<ITemplate>();
                this.serviceProvider = new Mock<IServiceProvider>();
                this.modelBus = new Mock<IModelBus>();

                this.serviceProvider.Setup(x => x.GetService(typeof(SModelBus))).Returns(this.modelBus.Object);

                this.solution = CreateSolution();
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var productStore = this.store.ElementFactory.CreateElement<ProductState>();
                    this.product = productStore.CreateProduct(prod => prod.InstanceName = "Foo");
                    var property = (Property)this.product.CreateProperty();
                    property.RawValue = "GuidanceValue";
                    property.Info = Mocks.Of<IPropertyInfo>().First(i => i.Name == "GuidanceName" && i.Type == "System.String" && i.IsVisible == true && i.IsReadOnly == false);
                });

                this.uriService = new Mock<IUriReferenceService>();
                this.uriService.Setup(x => x.ResolveUri<ITemplate>(It.IsAny<Uri>())).Returns(this.template.Object);

                this.command = new GenerateModelingCodeCommand
                {
                    TargetFileName = "Foo",
                    TemplateUri = new Uri("t4://foo.tt"),
                    UriService = this.uriService.Object,
                    ModelElement = (ModelElement)this.product,
                    ModelFile = "C:\\Temp\\foo" + NuPattern.Runtime.StoreConstants.RuntimeStoreExtension,
                    ServiceProvider = this.serviceProvider.Object,
                    Solution = this.solution,
                };

                // Set modelbus adapter
                var adapter = new Mock<ModelBusAdapterManager>();
                this.modelBus.Setup(x => x.FindAdapterManagers(It.IsAny<object[]>())).Returns(new[] { adapter.Object });
                adapter.Setup(x => x.GetSupportedLogicalAdapterIds()).Returns(new[] { "FooAdapter" });
                adapter.Setup(x => x.GetExposedElementTypes("FooAdapter")).Returns(new[] { new SupportedType(typeof(IProduct)) });
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTemplateCannotResolve_ThenThrowsFileNotFoundException()
            {
                this.uriService.Setup(x => x.ResolveUri<ITemplate>(It.IsAny<Uri>())).Returns((ITemplate)null);

                Assert.Throws<FileNotFoundException>(() => this.command.Execute());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTargetNameUsesNonExistingProperty_ThenThrowsInvalidOperationException()
            {
                this.command.TargetFileName = "{NonExistentProperty}";

                Assert.Throws<InvalidOperationException>(() => this.command.Execute());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSimpleTargetPathExists_ThenUnfoldsToIt()
            {
                this.command.TargetFileName = "Foo";
                this.command.TargetPath = "Project";
                var projectParent = this.solution.Find<IProject>().FirstOrDefault();

                this.command.Execute();

                this.template.Verify(x => x.Unfold(It.IsAny<string>(), projectParent));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTargetFileNameContainsInvalidFileChars_ThenRemovesThemandUnfoldsToIt()
            {
                this.command.TargetFileName = @"Foo\Bar";
                this.command.TargetPath = "Project";
                var projectParent = this.solution.Find<IProject>().FirstOrDefault();

                this.command.Execute();

                this.template.Verify(x => x.Unfold("FooBar", projectParent));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTargetPathContainsDots_ThenCanResolveThem()
            {
                this.command.TargetFileName = "Foo";
                this.command.TargetPath = "Project\\{InstanceName}\\..\\..\\Project";
                var projectParent = this.solution.Find<IProject>().FirstOrDefault();

                this.command.Execute();

                this.template.Verify(x => x.Unfold(It.IsAny<string>(), projectParent));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTargetPathExpressionExists_ThenUnfoldsToIt()
            {
                this.command.TargetFileName = "Foo";
                this.command.TargetPath = "Project\\{InstanceName}";
                var projectParent = this.solution.Find<IFolder>(x => x.Name == this.product.InstanceName).FirstOrDefault();

                this.command.Execute();

                this.template.Verify(x => x.Unfold(It.IsAny<string>(), projectParent));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecutingWithTargetNameExpression_ThenEvaluatesItForUnfold()
            {
                this.command.TargetPath = "Project";
                this.command.TargetFileName = "I{InstanceName}";
                var projectParent = this.solution.Find<IProject>().FirstOrDefault();

                this.command.Execute();

                this.template.Verify(x => x.Unfold("IFoo", projectParent));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecutingWithTargetNameExpression_ThenEvaluatesVariablePropertyForUnfold()
            {
                this.command.TargetPath = "Project";
                this.command.TargetFileName = "I{GuidanceName}";
                var projectParent = this.solution.Find<IProject>().FirstOrDefault();

                this.command.Execute();

                this.template.Verify(x => x.Unfold("IGuidanceValue", projectParent));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSimpleTargetPathDoesNotExist_ThenAddsItAndUnfoldsToIt()
            {
                this.command.TargetFileName = "Foo";
                this.command.TargetPath = "Project\\GeneratedCode";

                this.command.Execute();

                var projectParent = this.solution.Find("Project\\GeneratedCode").FirstOrDefault();
                Assert.NotNull(projectParent);

                this.template.Verify(x => x.Unfold(It.IsAny<string>(), projectParent));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTargetPathExpressionDoesNotExist_ThenAddsItAndUnfoldsToIt()
            {
                this.command.TargetFileName = "Foo";
                this.command.TargetPath = "Project\\GeneratedCode\\{InstanceName}";

                this.command.Execute();

                var projectParent = this.solution.Find("Project\\GeneratedCode\\Foo").FirstOrDefault();
                Assert.NotNull(projectParent);

                this.template.Verify(x => x.Unfold(It.IsAny<string>(), projectParent));
            }

            private static ISolution CreateSolution()
            {
                return new Solution
                {
                    Name = "Solution.sln",
                    PhysicalPath = "C:\\Temp",
                    Items = 
                    {
                        new SolutionFolder
                        {
                            Name = "Solution Items", 
                        },
                        new Project
                        {
                            Name = "Project",
                            Items = 
                            {
                                new Folder { Name = "Folder" },
                                new Folder { Name = "Foo" },
                            }
                        },
                    }
                };
            }
        }
    }
}
