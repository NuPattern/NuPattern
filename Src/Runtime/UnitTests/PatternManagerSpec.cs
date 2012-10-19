using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Patterning.Runtime.Store;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests
{
    public class PatternManagerSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            [TestMethod]
            public void WhenCreatingWithNullServiceProvider_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new PatternManager(null, Mock.Of<IShellEvents>(), Mock.Of<ISolutionEvents>(), Mock.Of<IItemEvents>(), Enumerable.Empty<IInstalledToolkitInfo>(), Mock.Of<IUserMessageService>()));
            }

            [TestMethod]
            public void WhenCreatingWithNullSolutionEvents_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new PatternManager(Mock.Of<IServiceProvider>(), Mock.Of<IShellEvents>(), null, Mock.Of<IItemEvents>(), Enumerable.Empty<IInstalledToolkitInfo>(), Mock.Of<IUserMessageService>()));
            }

            [TestMethod]
            public void WhenCreatingWithNullShellEvents_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new PatternManager(Mock.Of<IServiceProvider>(), null, Mock.Of<ISolutionEvents>(), Mock.Of<IItemEvents>(), Enumerable.Empty<IInstalledToolkitInfo>(), Mock.Of<IUserMessageService>()));
            }

            [TestMethod]
            public void WhenCreatingWithNullItemEvents_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new PatternManager(Mock.Of<IServiceProvider>(), Mock.Of<IShellEvents>(), Mock.Of<ISolutionEvents>(), null, Enumerable.Empty<IInstalledToolkitInfo>(), Mock.Of<IUserMessageService>()));
            }

            [TestMethod]
            public void WhenCreatingWithNullInstalledFactories_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new PatternManager(Mock.Of<IServiceProvider>(), Mock.Of<IShellEvents>(), Mock.Of<ISolutionEvents>(), Mock.Of<IItemEvents>(), null, Mock.Of<IUserMessageService>()));
            }

            [TestMethod]
            public void WhenCreatingWithNullMessageService_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new PatternManager(Mock.Of<IServiceProvider>(), Mock.Of<IShellEvents>(), Mock.Of<ISolutionEvents>(), Mock.Of<IItemEvents>(), Enumerable.Empty<IInstalledToolkitInfo>(), null));
            }

        }

        [TestClass]
        [DeploymentItem("Blank.slnbldr")]
        [DeploymentItem("Invalid.slnbldr")]
        [DeploymentItem("InvalidVersion.slnbldr")]
        [DeploymentItem("OldVersion.slnbldr")]
        [SuppressMessage("Microsoft.Design", "CA1001", Justification = "Disposed on cleanup.")]
        public class GivenAConfiguredPatternManager
        {
            private static readonly string solutionItemsDir = Path.GetTempPath() + "\\Solution Items";
            private Solution solution;
            private Mock<IShellEvents> shellEvents;
            private Mock<ISolutionEvents> solutionEvents;
            private Mock<IItemEvents> itemEvents;
            private Mock<IUserMessageService> messageService;
            private Mock<IInstalledToolkitInfo> toolkitInfo;
            private PatternManager manager;
            private DslTestStore<ProductStateStoreDomainModel> store;

            [TestInitialize]
            public void Initialize()
            {
                this.solutionEvents = new Mock<ISolutionEvents>();
                this.itemEvents = new Mock<IItemEvents>();
                this.messageService = new Mock<IUserMessageService>();

                if (Directory.Exists(solutionItemsDir))
                {
                    Directory.Delete(solutionItemsDir, true);
                }

                Directory.CreateDirectory(solutionItemsDir);

                this.solution = new Solution
                {
                    PhysicalPath = Path.GetTempFileName(),
                    Items =
					{
						new SolutionFolder 
						{
							PhysicalPath = Path.GetTempPath() + "\\Solution Items",
							Name = "Solution Items"
						}
					}
                };

                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                this.toolkitInfo = new Mock<IInstalledToolkitInfo>
                {
                    DefaultValue = DefaultValue.Mock
                };
                this.toolkitInfo.Setup(f => f.Id).Returns("foo");
                this.toolkitInfo.Setup(f => f.Version).Returns(new Version("1.0.0.0"));
                this.toolkitInfo.Setup(f => f.Schema.Pattern.ExtensionId).Returns("foo");

                var installedFactories = new[] { this.toolkitInfo.Object };

                var componentModel = new Mock<IComponentModel>();
                componentModel.Setup(cm => cm.GetService<IEnumerable<IInstalledToolkitInfo>>()).Returns(installedFactories);
                componentModel.Setup(cm => cm.GetService<IToolkitRepository>()).Returns(Mock.Of<IToolkitRepository>());

                var serviceProvider = Mock.Get(this.store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(SComponentModel))).Returns(componentModel.Object);
                serviceProvider.Setup(s => s.GetService(typeof(ISolution))).Returns(this.solution);

                this.shellEvents = new Mock<IShellEvents>();
                this.shellEvents.Setup(x => x.IsInitialized).Returns(true);

                this.manager = new TestPatternManager(this.store.ServiceProvider, this.shellEvents.Object, this.solutionEvents.Object, this.itemEvents.Object, installedFactories, this.messageService.Object);
            }

            [TestCleanup]
            public void Cleanup()
            {
                if (this.manager.IsOpen)
                {
                    this.manager.Products.ToList().ForEach(p => this.manager.DeleteProduct(p));
                    this.manager.Save();
                }

                this.store.Dispose();
            }

            [TestMethod]
            public void WhenCreatingAProductWithNullToolkitInfo_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => this.manager.CreateProduct(null, "foo"));
            }

            [TestMethod]
            public void WhenCreatingAProductWithNullName_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => this.manager.CreateProduct(new Mock<IInstalledToolkitInfo>().Object, null));
            }

            [TestMethod]
            public void WhenCreatingAProductWithEmptyName_ThenThrowsArgumentException()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => this.manager.CreateProduct(new Mock<IInstalledToolkitInfo>().Object, string.Empty));
            }

            [TestMethod]
            public void WhenDeletingANullProduct_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => this.manager.DeleteProduct((IProduct)null));
            }

            [TestMethod]
            public void WhenDeletingANullProductName_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => this.manager.Delete((string)null));
            }

            [TestMethod]
            public void WhenDeletingAnEmptyProductName_ThenThrowsArgumentOutOfRangeException()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => this.manager.Delete(string.Empty));
            }

            [TestMethod]
            public void WhenCreatingProduct_ThenRaisesElementInstantiatedEvent()
            {
                this.manager.Open("Blank" + Constants.RuntimeStoreExtension);
                IProduct instantiatedProduct = null;
                this.manager.ElementInstantiated += (sender, args) => instantiatedProduct = (IProduct)args.Value;

                this.manager.CreateProduct(this.toolkitInfo.Object, "toolkit");

                Assert.NotNull(instantiatedProduct);
            }

            [TestMethod]
            public void WhenSavingCreatedProduct_ThenRaisesStoreSavedEvent()
            {
                this.manager.Open("Blank" + Constants.RuntimeStoreExtension);
                this.manager.CreateProduct(this.toolkitInfo.Object, "toolkit");

                var managerSavedCalled = false;
                var storeSavedCalled = false;

                this.manager.StoreSaved += (sender, args) => managerSavedCalled = true;
                this.manager.Store.Saved += (sender, args) => storeSavedCalled = true;

                this.manager.Save();

                Assert.True(managerSavedCalled);
                Assert.True(storeSavedCalled);
            }

            [TestMethod]
            public void WhenCreatingProduct_ThenDoesNotRaisePropertyChangedEvents()
            {
                this.manager.Open("Blank" + Constants.RuntimeStoreExtension);
                bool changed = false;
                this.manager.PropertyChanged += (sender, args) => changed = true;

                this.manager.CreateProduct(this.toolkitInfo.Object, "propertyChangedTest");

                Assert.False(changed);
            }

            [TestMethod]
            public void WhenOpeningStateFile_ThenRaisesIsOpenChanged()
            {
                var changed = false;

                this.manager.IsOpenChanged += (s, e) => changed = true;

                this.manager.Open(new FileInfo("Blank" + Constants.RuntimeStoreExtension).FullName);

                Assert.True(changed);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Dont"), TestMethod]
            public void WhenOpeningStateFileAndAlreadyOpened_SavesExistingStateEvenIfNoSolutionIsOpened()
            {
                var tempFile = Path.Combine(solutionItemsDir, Guid.NewGuid().ToString());
                File.Copy(new FileInfo("Blank" + Constants.RuntimeStoreExtension).FullName, tempFile, true);

                this.manager.Open(tempFile);

                Assert.Equal(tempFile, this.manager.StoreFile);

                var product = this.manager.CreateProduct(this.toolkitInfo.Object, "Foo");

                this.manager.Open(new FileInfo("Blank" + Constants.RuntimeStoreExtension).FullName);

                Assert.True(File.ReadAllText(tempFile).Contains(product.Id.ToString()));
            }

            [TestMethod]
            public void WhenOpeningInvalidStateFile_ThenIsOpenChangedIsNotRaised()
            {
                var changed = false;

                this.manager.IsOpenChanged += (s, e) => changed = true;

                this.manager.Open(new FileInfo("Invalid" + Constants.RuntimeStoreExtension).FullName);

                Assert.False(changed);
            }

            [TestMethod]
            public void WhenOpeningOldVersionWithoutUpgradeStateFile_ThenIsOpenChangedIsNotRaised()
            {
                var changed = false;

                this.messageService.Setup(m => m.PromptWarning(It.IsAny<string>())).Returns(false);

                this.manager.IsOpenChanged += (s, e) => changed = true;

                this.manager.Open(new FileInfo("OldVersion" + Constants.RuntimeStoreExtension).FullName);

                Assert.False(changed);
            }

            [TestMethod]
            public void WhenOpeningOldVersionWithUpgradeStateFile_ThenIsOpenChangedIsNotRaised()
            {
                var changed = false;

                this.messageService.Setup(m => m.PromptWarning(It.IsAny<string>())).Returns(true);

                this.manager.IsOpenChanged += (s, e) => changed = true;

                this.manager.Open(new FileInfo("OldVersion" + Constants.RuntimeStoreExtension).FullName);

                Assert.True(changed);
            }

            [TestMethod]
            public void WhenOpeningInvalidVersionStateFile_ThenIsOpenChangedIsNotRaised()
            {
                var changed = false;

                this.manager.IsOpenChanged += (s, e) => changed = true;

                this.manager.Open(new FileInfo("InvalidVersion" + Constants.RuntimeStoreExtension).FullName);

                Assert.False(changed);
            }

            [TestMethod]
            public void WhenSavingAClosedManager_ThenThrowsInvalidOperationException()
            {
                Assert.Throws<InvalidOperationException>(() => this.manager.Save());
            }

            [TestMethod]
            public void WhenClosingAClosedManager_ThenThrowsInvalidOperationException()
            {
                Assert.Throws<InvalidOperationException>(() => this.manager.Close());
            }

            [TestMethod]
            public void WhenRetrievingProductsFromClosedManager_ThenReturnsEmptyEnumeration()
            {
                Assert.NotNull(this.manager.Products);
                Assert.Equal(0, this.manager.Products.Count());
            }

            [TestMethod]
            public void WhenOpeningSolutionWithNoState_ThenDoesNotRiseIsOpenChanged()
            {
                var changed = false;
                this.manager.IsOpenChanged += (s, e) => changed = true;

                this.solutionEvents.Raise(x => x.SolutionOpened += null, new SolutionEventArgs(this.solution));

                Assert.False(changed);
            }

            [TestMethod]
            public void WhenOpeningSolutionButShellNotInitialized_ThenWaitsForShellInitialized()
            {
                var stateFile = new FileInfo("Blank" + Constants.RuntimeStoreExtension).FullName;

                // Add the product state file to cause the auto-open behavior.
                this.solution.Items.OfType<SolutionFolder>().First().Items.Add(new Item { PhysicalPath = stateFile });

                // Shell is uninitialized.
                this.shellEvents.Setup(x => x.IsInitialized).Returns(false);

                // Raise the event.
                this.solutionEvents.Raise(x => x.SolutionOpened += null, new SolutionEventArgs(this.solution));

                Assert.False(this.manager.IsOpen);

                // Initialize the shell
                this.shellEvents.Setup(x => x.IsInitialized).Returns(true);
                this.shellEvents.Raise(x => x.ShellInitialized += null, EventArgs.Empty);

                Assert.True(this.manager.IsOpen);
                Assert.Equal(stateFile, this.manager.StoreFile);
            }

            [TestMethod]
            public void WhenOpeningSolutionWithOneStateFile_ThenOpensItInManager()
            {
                var stateFile = new FileInfo("Blank" + Constants.RuntimeStoreExtension).FullName;

                this.solutionEvents.Raise(x => x.SolutionOpened += null, new SolutionEventArgs(new Solution
                {
                    PhysicalPath = "Bar.sln",
                    Items = 
					{
						new SolutionFolder
						{
							Name = "Solution Items", 
							Items = 
							{
								new Item
								{
									PhysicalPath = stateFile
								}
							}
						}
					}
                }));

                Assert.Equal(stateFile, this.manager.StoreFile);
            }

            [TestMethod]
            public void WhenOpeningSolutionWithStateFileUnderOtherFolder_ThenDoesNotOpenItInManager()
            {
                var stateFile = new FileInfo("Blank" + Constants.RuntimeStoreExtension).FullName;

                this.solutionEvents.Raise(x => x.SolutionOpened += null, new SolutionEventArgs(new Solution
                {
                    PhysicalPath = "Bar.sln",
                    Items = 
					{
						new SolutionFolder
						{
							Name = "My Folder", 
							Items = 
							{
								new Item
								{
									PhysicalPath = stateFile
								}
							}
						}
					}
                }));

                Assert.False(this.manager.IsOpen);
            }

            [TestMethod]
            public void WhenOpeningSolutionWithMultipleStateFiles_ThenOpensTheOneMatchingTheSolutionName()
            {
                var stateFile = new FileInfo("Blank" + Constants.RuntimeStoreExtension).FullName;

                this.solutionEvents.Raise(x => x.SolutionOpened += null, new SolutionEventArgs(new Solution
                {
                    Name = "Blank.sln",
                    PhysicalPath = "Blank.sln",
                    Items = 
					{
						new SolutionFolder
						{
							Name = "Solution Items", 
							Items = 
							{
								new Item { PhysicalPath = "Foo" + Constants.RuntimeStoreExtension },
								new Item { PhysicalPath = stateFile },
							}
						}
					}
                }));

                Assert.Equal(stateFile, this.manager.StoreFile);
            }

            [TestMethod]
            public void WhenOpeningSolutionWithMultipleStateFiles_ThenOpensFirstOneIfNoSolutionNameMatch()
            {
                var stateFile = new FileInfo("Blank" + Constants.RuntimeStoreExtension).FullName;

                this.solutionEvents.Raise(x => x.SolutionOpened += null, new SolutionEventArgs(new Solution
                {
                    Name = "Baz.sln",
                    PhysicalPath = "Baz.sln",
                    Items = 
					{
						new SolutionFolder
						{
							Name = "Solution Items", 
							Items = 
							{
								new Item { PhysicalPath = stateFile },
								new Item { PhysicalPath = "Foo" + Constants.RuntimeStoreExtension },
								new Item { PhysicalPath = "Bar" + Constants.RuntimeStoreExtension },
							}
						}
					}
                }));

                Assert.Equal(stateFile, this.manager.StoreFile);
            }

            [TestMethod]
            public void WhenClosingSolution_ThenNoOpIfNotOpened()
            {
                this.solutionEvents.Raise(x => x.SolutionClosing += null, new SolutionEventArgs(new Solution()));
            }

            [TestMethod]
            public void WhenClosingSolution_ThenClosesManagerIfOpened()
            {
                this.manager.Open(new FileInfo("Blank" + Constants.RuntimeStoreExtension).FullName);

                Assert.True(this.manager.IsOpen);

                this.solutionEvents.Raise(x => x.SolutionClosing += null, new SolutionEventArgs(new Solution()));

                Assert.False(this.manager.IsOpen);
            }

            private class TestPatternManager : PatternManager
            {
                public TestPatternManager(IServiceProvider serviceProvider, IShellEvents shellEvents, ISolutionEvents solutionEvents, IItemEvents itemEvents, IEnumerable<IInstalledToolkitInfo> installedFactories, IUserMessageService messageService)
                    : base(serviceProvider, shellEvents, solutionEvents, itemEvents, installedFactories, messageService)
                {
                }

                protected override IItemContainer AddStateToSolution(IItemContainer targetFolder, string targetName, string sourceFile)
                {
                    File.WriteAllText(Path.Combine(targetFolder.PhysicalPath, targetName), File.ReadAllText(sourceFile));

                    return Mocks.Of<IItemContainer>().First(x => x.Name == targetName && x.Parent == targetFolder && x.PhysicalPath == sourceFile);
                }
            }
        }
    }
}