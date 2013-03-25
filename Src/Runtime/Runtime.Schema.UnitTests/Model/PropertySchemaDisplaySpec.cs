using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.ComponentModel;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class PropertySchemaDisplaySpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public abstract class GivenAPatternModel
        {
            protected PatternModelSchema PatternModel { get; set; }

            internal PropertySchema Property { get; set; }

            [TestInitialize]
            public virtual void InitializeContext()
            {
                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.PatternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = this.PatternModel.Create<PatternSchema>();
                    pattern.PatternLink = "patternmanager://foo";

                    var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                        u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                            p.ToolkitInfo.Identifier == "ToolkitId"));

                    pattern.UriService = uriService;

                    this.Property = pattern.Create<PropertySchema>();
                });
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnAuthoredPattern : GivenAPatternModel
        {
            [TestMethod, TestCategory("Unit")]
            public void ThenTypePropertyIsBrowsableAndNotReadOnly()
            {
                var descriptor = TypedDescriptor.GetProperty(this.Property, property => property.Type);

                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void TheValidationRulesPropertyIsBrowsableAndNotReadOnly()
            {
                var descriptor = TypedDescriptor.GetProperty(this.Property, property => property.RawValidationRules);

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

                // Reassign tailored element
                this.Property = this.PatternModel.Pattern.Properties[0];
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenTypePropertyIsReadOnly()
            {
                var descriptor = TypedDescriptor.GetProperty(this.Property, property => property.Type);

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