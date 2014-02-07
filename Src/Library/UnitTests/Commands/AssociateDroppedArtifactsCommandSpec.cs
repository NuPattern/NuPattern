using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Commands;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.UnitTests.Commands
{
    public class AssociateDroppedArtifactsCommandSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenSolutionItems
        {
            private AssociateDroppedArtifactsCommand command;
            private Mock<ISolution> solution;
            private Mock<IUriReferenceService> uriService;
            private Mock<IProductElement> currentElement;
            private Mock<IReference> createdReference;

            [TestInitialize]
            public void Initialize()
            {
                this.solution = new Mock<ISolution>();
                this.uriService = new Mock<IUriReferenceService>();
                this.createdReference = new Mock<IReference>();
                this.currentElement = new Mock<IProductElement>();
                this.currentElement.Setup(ce => ce.CreateReference(It.IsAny<Action<IReference>>()))
                    .Returns(this.createdReference.Object);

                this.command = new AssociateDroppedArtifactsCommand();
                this.command.Solution = this.solution.Object;
                this.command.UriService = this.uriService.Object;
                this.command.CurrentElement = this.currentElement.Object;
                this.command.Tag = "tag1";

                this.solution.Setup(s => s.Items)
                    .Returns(new[]
                    {
                        Mock.Of<IItemContainer>(i => i.Kind == ItemKind.Item && i.PhysicalPath == @"C:\Foo"),
                        Mock.Of<IItemContainer>(i => i.Kind == ItemKind.Item && i.PhysicalPath == @"C:\Bar"),
                    });

                this.uriService.Setup(us => us.CreateUri(It.Is<IItemContainer>(i => i.PhysicalPath == @"C:\Foo"), null))
                    .Returns(new Uri("solution://Foo"));
                this.uriService.Setup(us => us.CreateUri(It.Is<IItemContainer>(i => i.PhysicalPath == @"C:\Bar"), null))
                    .Returns(new Uri("solution://Bar"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithNoDroppedItems_ThenReturns()
            {
                this.command.AssociateDroppedSolutionItems(new HashSet<string>());

                this.solution.Verify(s => s.Items, Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithNewDroppedItems_ThenAddsNewReferencesToItems()
            {
                this.currentElement.Setup(ce => ce.References)
                    .Returns(Enumerable.Empty<IReference>());

                this.command.AssociateDroppedSolutionItems(new HashSet<string> { @"c:\foo", @"c:\bar" });
                
                this.currentElement.Verify(ce => ce.CreateReference(It.IsAny<Action<IReference>>()), Times.Exactly(2));
                this.createdReference.VerifySet(r => r.Tag = "tag1", Times.Exactly(2));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithNewWithExistingDroppedItems_ThenUpdatesReferencesToItems()
            {
                var reference = Mock.Of<IReference>(r => r.Kind == typeof (SolutionArtifactLinkReference).FullName
                                                         && r.Value == "solution://Foo");
                this.currentElement.Setup(ce => ce.References)
                    .Returns(new[]
                    {
                        reference
                    });

                this.command.AssociateDroppedSolutionItems(new HashSet<string> { @"c:\foo", @"c:\bar" });

                this.currentElement.Verify(ce => ce.CreateReference(It.IsAny<Action<IReference>>()), Times.Exactly(1));
                this.createdReference.VerifySet(r => r.Tag = "tag1", Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithExistingDroppedItems_ThenUpdatesReferencesAndTagsToItems()
            {
                var reference = Mock.Of<IReference>(r => r.Kind == typeof (SolutionArtifactLinkReference).FullName
                                                         && r.Value == "solution://Foo"
                                                         && r.Tag == "foo,bar");
                this.currentElement.Setup(ce => ce.References)
                    .Returns(new[]
                    {
                        reference
                    });

                this.command.AssociateDroppedSolutionItems(new HashSet<string> { @"c:\foo", @"c:\bar" });

                this.currentElement.Verify(ce => ce.CreateReference(It.IsAny<Action<IReference>>()), Times.Exactly(1));
                Assert.Equal("foo,bar,tag1", reference.Tag);
            }
        }
    }
}