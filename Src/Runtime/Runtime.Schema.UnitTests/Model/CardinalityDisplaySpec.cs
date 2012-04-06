using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
{
    [TestClass]
    public class CardinalityDisplaySpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public abstract class GivenAPatternModel
        {
            protected PatternModelSchema PatternModel { get; set; }

            [TestInitialize]
            public virtual void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "ToolkitId"));

                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.PatternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                    var patternSchema = this.PatternModel.Create<PatternSchema>();
                    patternSchema.Name = "WebService";
                    patternSchema.PatternLink = "patternmanager://foo";
                    patternSchema.UriService = uriService;

                    var property = patternSchema.Create<PropertySchema>();
                    property.Name = "Namespace";
                });
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnAuthoredPatternWithAViewAndASingleElement : GivenAPatternModel
        {
            protected ElementSchema Element { get; set; }

            [TestInitialize]
            public override void InitializeContext()
            {
                base.InitializeContext();

                this.PatternModel.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    var view = this.PatternModel.Pattern.Create<ViewSchema>();
                    this.Element = view.Create<ElementSchema>();
                });
            }

            [TestMethod]
            public void ThenCardinalityPropertyIsBrowsableAndNotReadOnly()
            {
                var propertyName = Reflector<ViewHasElements>.GetProperty(rellie => rellie.Cardinality).Name;
                var descriptor = TypeDescriptor.GetProperties(this.Element)[propertyName];

                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenATailoredPatternWithAViewAndASingleElement : GivenAPatternModel
        {
            protected ElementSchema Element { get; set; }

            [TestInitialize]
            public override void InitializeContext()
            {
                base.InitializeContext();

                this.PatternModel.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    var view = this.PatternModel.Pattern.Create<ViewSchema>();
                    this.Element = view.Create<ElementSchema>();
                });

                this.PatternModel = PatternModelSpec.TailorPatternModel(this.PatternModel, new Version("1.0.0.0"));

                this.Element = this.PatternModel.Pattern.Views[0].Elements[0] as ElementSchema;
            }

            [TestMethod]
            public void ThenCardinalityPropertyIsBrowsableAndReadOnly()
            {
                var propertyName = Reflector<ViewHasElements>.GetProperty(rellie => rellie.Cardinality).Name;
                var descriptor = TypeDescriptor.GetProperties(this.Element)[propertyName];

                Assert.True(descriptor.IsBrowsable);
                Assert.True(descriptor.IsReadOnly);
            }

            [TestMethod]
            public void WhenAddingNewElement_ThenCardinalityPropertyIsBrowsableAndNotReadOnly()
            {
                ElementSchema element2 = null;
                this.PatternModel.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    element2 = this.Element.View.Create<ElementSchema>();
                });

                var propertyName = Reflector<ViewHasElements>.GetProperty(rellie => rellie.Cardinality).Name;
                var descriptor = TypeDescriptor.GetProperties(element2)[propertyName];

                Assert.False(element2.IsInheritedFromBase);
                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }
        }
    }
}