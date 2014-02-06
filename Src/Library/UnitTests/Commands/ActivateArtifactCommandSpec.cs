using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Commands;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.UnitTests.Commands
{
    [TestClass]
    public class ActivateArtifactCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenACommand
        {
            protected Mock<IProductElement> OwnerElement { get; set; }
            protected Mock<IUriReferenceService> UriService { get; set; }
            protected Mock<ISolution> Solution { get; set; }
            protected ActivateArtifactCommand Command { get; set; }

            [TestInitialize]
            public virtual void Initialize()
            {
                this.UriService = new Mock<IUriReferenceService>();
                this.OwnerElement = new Mock<IProductElement>();
                this.OwnerElement
                    .Setup(x => x.Root)
                    .Returns(Mock.Of<IProduct>(p =>
                        p.ProductState.GetService(typeof(IUriReferenceService)) == this.UriService.Object));

                this.Solution = new Mock<ISolution>();

                this.Command = new ActivateArtifactCommand();

                this.Command.CurrentElement = this.OwnerElement.Object;
                this.Command.UriReferenceService = this.UriService.Object;
                this.Command.CurrentElement = this.OwnerElement.Object;
                this.Command.Solution = this.Solution.Object;
            }
        }

        [TestClass]
        public class GivenACommandWithNoReferences : GivenACommand
        {
            [TestMethod, TestCategory("Unit")]
            public void ThenOpenIsFalse()
            {
                Assert.False(this.Command.Open);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactLinkIsNull_ThenNotActivated()
            {
                this.Command.Execute();

                this.UriService.Verify(service => service.ResolveUri<IItemContainer>(It.IsAny<Uri>()), Times.Never());
            }
        }

        [TestClass]
        public class GivenACommandWithSingleReference : GivenACommand
        {
            private Mock<IItemContainer> item = new Mock<IItemContainer>();

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.item.Setup(container => container.Select());

                Mock<IReference> reference = new Mock<IReference>();
                reference.SetupGet(r => r.Kind).Returns(ReferenceKindConstants.SolutionItem);
                reference.SetupGet(r => r.Value).Returns("foo://");
                this.OwnerElement.Setup(owner => owner.References).Returns(new[] { reference.Object });

                this.UriService.Setup(service => service.ResolveUri<IItemContainer>(new Uri("foo://"))).Returns(item.Object);
                this.UriService.Setup(service => service.Open(It.IsAny<IItemContainer>(), null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactLinkNotResolved_ThenNotActivated()
            {
                this.UriService.Setup(service => service.ResolveUri<IItemContainer>(new Uri("foo://"))).Returns((IItemContainer)null);

                this.Command.Execute();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactLinkResolvedAndNotOpen_ThenItemSelectedAndNotOpened()
            {
                this.Command.Execute();

                this.item.Verify(container => container.Select(), Times.AtMostOnce());
                this.UriService.Verify(service => service.Open(It.IsAny<IItemContainer>(), null), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactLinkResolvedAndOpen_ThenItemSelectedAndOpened()
            {
                this.Command.Open = true;
                this.Command.Execute();

                this.item.Verify(container => container.Select(), Times.AtMostOnce());
                this.UriService.Verify(service => service.Open(It.IsAny<IItemContainer>(), null), Times.Once());
            }
        }

        [TestClass]
        public class GivenACommandWithMultipleReferences : GivenACommand
        {
            private Mock<IItemContainer> unfoldedItem = new Mock<IItemContainer>();
            private Mock<IItemContainer> generatedItem = new Mock<IItemContainer>();

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                this.unfoldedItem.Setup(container => container.Select());
                this.generatedItem.Setup(container => container.Select());

                Mock<IReference> unfoldedReference = new Mock<IReference>();
                unfoldedReference.SetupGet(r => r.Kind).Returns(ReferenceKindConstants.SolutionItem);
                unfoldedReference.SetupGet(r => r.Value).Returns("unfolded://foo/item");
                Mock<IReference> generatedReference = new Mock<IReference>();
                generatedReference.SetupGet(r => r.Kind).Returns(ReferenceKindConstants.SolutionItem);
                generatedReference.SetupGet(r => r.Value).Returns("generated://bar/item");
                this.OwnerElement.Setup(owner => owner.References).Returns(new[] { unfoldedReference.Object, generatedReference.Object });

                this.UriService.Setup(service => service.ResolveUri<IItemContainer>(new Uri("unfolded://foo/item"))).Returns(unfoldedItem.Object);
                this.UriService.Setup(service => service.ResolveUri<IItemContainer>(new Uri("generated://bar/item"))).Returns(generatedItem.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNothingResolves_ThenNoItemsSelected()
            {
                this.UriService.Setup(service => service.ResolveUri<IItemContainer>(new Uri("unfolded://foo/item"))).Returns((IItemContainer)null);
                this.UriService.Setup(service => service.ResolveUri<IItemContainer>(new Uri("generated://bar/item"))).Returns((IItemContainer)null);

                this.Command.Execute();

                this.unfoldedItem.Verify(container => container.Select(), Times.Never());
                this.generatedItem.Verify(container => container.Select(), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllResolves_ThenAllItemsSelected()
            {
                this.Command.Execute();

                this.unfoldedItem.Verify(container => container.Select(), Times.AtMostOnce());
                this.generatedItem.Verify(container => container.Select(), Times.AtMostOnce());
            }
        }
    }
}