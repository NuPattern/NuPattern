using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.ComponentModel;
using NuPattern.Modeling;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public class NamedElementSchemaDisplaySpec
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
                var uriService = Mock.Of<IUriReferenceService>(
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
                });
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnAuthoredPattern : GivenAPatternModel
        {
            [TestMethod, TestCategory("Unit")]
            public void ThenNamePropertyIsBrowsableAndNotReadOnly()
            {
                var descriptor = TypedDescriptor.GetProperty(this.PatternModel.Pattern, pattern => pattern.Name);

                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenATailoredPattern : GivenAPatternModel
        {
            [TestInitialize]
            public override void InitializeContext()
            {
                base.InitializeContext();

                this.PatternModel = PatternModelSpec.TailorPatternModel(this.PatternModel, new Version("1.0.0.0"));
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenNamePropertyIsReadOnly()
            {
                var descriptor = TypedDescriptor.GetProperty(this.PatternModel.Pattern, pattern => pattern.Name);

                Assert.True(descriptor.IsBrowsable);
                Assert.True(descriptor.IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingNewProperty_ThenNamePropertyIsBrowsableAndNotReadOnly()
            {
                PropertySchema property = null;
                this.PatternModel.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    property = this.PatternModel.Create<PropertySchema>();
                });

                var descriptor = TypedDescriptor.GetProperty(property, prop => prop.Name);

                Assert.False(property.IsInheritedFromBase);
                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }
        }
    }
}