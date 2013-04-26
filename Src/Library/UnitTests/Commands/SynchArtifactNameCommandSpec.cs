using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Diagnostics;
using NuPattern.Library.Commands;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.UnitTests.Commands
{
    [TestClass]
    public class SynchArtifactNameCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenACommand
        {
            protected Mock<IProductElement> OwnerElement { get; private set; }
            protected Mock<IUriReferenceService> UriService { get; private set; }
            protected SynchArtifactNameCommand Command { get; private set; }
            protected Mock<IServiceProvider> ServiceProvider { get; private set; }
            protected Mock<TraceListener> Listener { get; private set; }

            [TestInitialize]
            public virtual void Initialize()
            {
                this.UriService = new Mock<IUriReferenceService>();
                this.OwnerElement = new Mock<IProductElement>();
                this.OwnerElement
                    .Setup(x => x.Root)
                    .Returns(Mock.Of<IProduct>(p =>
                        p.ProductState.GetService(typeof(IUriReferenceService)) == this.UriService.Object));

                this.ServiceProvider = new Mock<IServiceProvider>();
                this.Listener = new Mock<TraceListener>();

                Tracer.Manager.AddListener(typeof(SynchArtifactNameCommand).FullName, this.Listener.Object);
                Tracer.Manager.GetSource(typeof(SynchArtifactNameCommand).FullName).Switch.Level = SourceLevels.All;

                this.Command = new SynchArtifactNameCommand();
                this.Command.CurrentElement = this.OwnerElement.Object;
                this.Command.UriReferenceService = this.UriService.Object;
                this.Command.ServiceProvider = this.ServiceProvider.Object;
            }

            [TestCleanup]
            public virtual void Cleanup()
            {
                Tracer.Manager.RemoveListener(typeof(SynchArtifactNameCommand).FullName, this.Listener.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenReferenceEmpty_ThenDoNothing()
            {
                this.OwnerElement.Setup(e => e.References).Returns(Enumerable.Empty<IReference>());

                this.Command.Execute();
            }
        }

        [TestClass]
        public class GivenACommandWithNonResolvableReference : GivenACommand
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenReferenceNotResolved_ThenLogsWarning()
            {
                Mock<IReference> reference = new Mock<IReference>();
                reference.Setup(r => r.Kind).Returns(ReferenceKindConstants.ArtifactLink);
                reference.Setup(r => r.Value).Returns("solution://testuri/item");
                reference.Setup(r => r.Owner).Returns(this.OwnerElement.Object);

                this.OwnerElement.Setup(e => e.References).Returns(new[] { reference.Object });

                this.Command.Execute();

                this.Listener.Verify(x => x.TraceEvent(It.IsAny<TraceEventCache>(),
                    typeof(SynchArtifactNameCommand).FullName,
                    TraceEventType.Warning,
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>()));
            }
        }

        [TestClass]
        public class GivenACommandWithReferenceOnAnIItem : GivenACommand
        {
            private Mock<IItem> solutionItem;

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                Mock<IReference> reference = new Mock<IReference>();
                reference.Setup(r => r.Kind).Returns(ReferenceKindConstants.ArtifactLink);
                reference.Setup(r => r.Value).Returns("solution://testuri/item");
                this.OwnerElement.Setup(e => e.References).Returns(new[] { reference.Object });
                this.OwnerElement.Setup(ce => ce.InstanceName).Returns("TestElementName");
                reference.Setup(r => r.Owner).Returns(this.OwnerElement.Object);

                this.solutionItem = new Mock<IItem>();
                this.solutionItem.Setup(si => si.Name).Returns("TestSolutionItemName.cs");
                this.solutionItem.Setup(si => si.As<EnvDTE.ProjectItem>()).Returns(new Mock<EnvDTE.ProjectItem>().Object);
                Mock<IProject> parent = new Mock<IProject>();
                parent.Setup(p => p.Items).Returns(new[] { solutionItem.Object });
                this.solutionItem.Setup(si => si.Parent).Returns(parent.Object);

                this.UriService.Setup(s => s.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(solutionItem.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenReferenceIsSameName_ThenDoNothing()
            {
                this.solutionItem.Setup(c => c.Name).Returns("TestElementName.cs");

                this.Command.Execute();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenVsItemRenamed()
            {
                var projectItem = Mock.Get<EnvDTE.ProjectItem>(this.solutionItem.Object.As<EnvDTE.ProjectItem>());

                Command.Execute();

                projectItem.VerifySet(pi => pi.Name = "TestElementName.cs", Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameContainsInvalidFileChars_ThenVsItemRenamedWithRemovedChars()
            {
                this.OwnerElement.Setup(ce => ce.InstanceName).Returns(@"Test:ElementName");

                var projectItem = Mock.Get<EnvDTE.ProjectItem>(this.solutionItem.Object.As<EnvDTE.ProjectItem>());

                Command.Execute();

                projectItem.VerifySet(pi => pi.Name = "TestElementName.cs", Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenItemAlreadyExists_ThenVsItemRenamedWithVerifiedName()
            {
                var projectItem = Mock.Get<EnvDTE.ProjectItem>(this.solutionItem.Object.As<EnvDTE.ProjectItem>());

                var parent = Mock.Get<IProject>(this.solutionItem.Object.Parent as IProject);
                Mock<IItem> anotherItem = new Mock<IItem>();
                anotherItem.Setup(i => i.Name).Returns(this.OwnerElement.Object.InstanceName + ".cs");
                parent.Setup(p => p.Items).Returns(new[] { solutionItem.Object, anotherItem.Object });

                Command.Execute();

                projectItem.VerifySet(pi => pi.Name = "TestElementName1.cs", Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenItemAlreadyExistsAndTargetFileNameHasNoExtension_ThenVsItemRenamedWithVerifiedName()
            {
                var projectItem = Mock.Get<EnvDTE.ProjectItem>(this.solutionItem.Object.As<EnvDTE.ProjectItem>());

                var parent = Mock.Get<IProject>(this.solutionItem.Object.Parent as IProject);
                Mock<IItem> anotherItem = new Mock<IItem>();
                anotherItem.Setup(i => i.Name).Returns(this.OwnerElement.Object.InstanceName + ".cs");
                parent.Setup(p => p.Items).Returns(new[] { solutionItem.Object, anotherItem.Object });


                var reference = Mock.Get<IReference>(this.OwnerElement.Object.References.First());
                reference.Setup(r => r.Tag).Returns(BindingSerializer.Serialize<ReferenceTag>(new ReferenceTag { TargetFileName = "TestElementName1" }));

                Command.Execute();

                projectItem.VerifySet(pi => pi.Name = "TestElementName1.cs", Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenItemAlreadyExistsAndTargetFileNameHasExtension_ThenVsItemRenamedWithVerifiedName()
            {
                var projectItem = Mock.Get<EnvDTE.ProjectItem>(this.solutionItem.Object.As<EnvDTE.ProjectItem>());

                var parent = Mock.Get<IProject>(this.solutionItem.Object.Parent as IProject);
                Mock<IItem> anotherItem = new Mock<IItem>();
                anotherItem.Setup(i => i.Name).Returns(this.OwnerElement.Object.InstanceName + ".cs");
                parent.Setup(p => p.Items).Returns(new[] { solutionItem.Object, anotherItem.Object });


                var reference = Mock.Get<IReference>(this.OwnerElement.Object.References.First());
                reference.Setup(r => r.Tag).Returns(BindingSerializer.Serialize<ReferenceTag>(new ReferenceTag { TargetFileName = "TestElementName1.cs" }));

                Command.Execute();

                projectItem.VerifySet(pi => pi.Name = "TestElementName1.cs", Times.Once());
            }
        }

        [TestClass]
        public class GivenACommandWithReferenceOnAProject : GivenACommand
        {
            private Mock<IProject> solutionItem;

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                Mock<IReference> reference = new Mock<IReference>();
                reference.Setup(r => r.Kind).Returns(ReferenceKindConstants.ArtifactLink);
                reference.Setup(r => r.Value).Returns("solution://testuri/item");
                this.OwnerElement.Setup(e => e.References).Returns(new[] { reference.Object });
                this.OwnerElement.Setup(ce => ce.InstanceName).Returns("TestProductName");
                reference.Setup(r => r.Owner).Returns(this.OwnerElement.Object);

                this.solutionItem = new Mock<IProject>();
                this.solutionItem.Setup(si => si.Name).Returns("TestSolutionItemName");
                this.solutionItem.Setup(si => si.As<EnvDTE.Project>()).Returns(new Mock<EnvDTE.Project>().Object);
                Mock<ISolution> parent = new Mock<ISolution>();
                parent.Setup(p => p.Items).Returns(new[] { solutionItem.Object });
                this.solutionItem.Setup(si => si.Parent).Returns(parent.Object);

                this.UriService.Setup(s => s.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(solutionItem.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenReferenceIsSameName_ThenDoNothing()
            {
                this.solutionItem.Setup(c => c.Name).Returns("TestProductName");

                this.Command.Execute();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenVsProjectRenamed()
            {
                var project = Mock.Get<EnvDTE.Project>(this.solutionItem.Object.As<EnvDTE.Project>());

                Command.Execute();

                project.VerifySet(pi => pi.Name = "TestProductName", Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameContainsInvalidFileChars_ThenVsItemRenamedWithRemovedChars()
            {
                this.OwnerElement.Setup(ce => ce.InstanceName).Returns(@"Test:ProductName");

                var project = Mock.Get<EnvDTE.Project>(this.solutionItem.Object.As<EnvDTE.Project>());

                Command.Execute();

                project.VerifySet(pi => pi.Name = "TestProductName", Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenItemAlreadyExists_ThenVsItemRenamedWithVerifiedName()
            {
                var project = Mock.Get<EnvDTE.Project>(this.solutionItem.Object.As<EnvDTE.Project>());

                var parent = Mock.Get<ISolution>(this.solutionItem.Object.Parent as ISolution);
                Mock<IProject> anotherProject = new Mock<IProject>();
                anotherProject.Setup(i => i.Name).Returns("TestProductName");
                parent.Setup(p => p.Items).Returns(new[] { solutionItem.Object, anotherProject.Object });

                Command.Execute();

                project.VerifySet(pi => pi.Name = "TestProductName1", Times.Once());
            }
        }
    }
}