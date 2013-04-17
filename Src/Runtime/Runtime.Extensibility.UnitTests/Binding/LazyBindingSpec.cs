using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.ComponentModel.Composition;

namespace NuPattern.Runtime.UnitTests.Binding
{
    [TestClass]
    public class LazyBindingSpec
    {
        [TestMethod, TestCategory("Unit")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        public void WhenExportProvidedUpFront_ThenEvaluatesTrue()
        {
            var catalog = new TypeCatalog(typeof(Foo), typeof(Bar));
            var container = new CompositionContainer(catalog);
            var compositionService = new CompositionContainerService(container);

            var binding = new Binding<IFoo>(compositionService, "Foo");
            var result = binding.Evaluate();

            Assert.IsTrue(result);
        }

        //// TODO: waiting on bug fix BlueTab-PLATU11
        [Ignore]
        [TestMethod, TestCategory("Unit")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code.")]
        public void WhenExportProvidedLater_ThenEvaluatesTrue()
        {
            // Note catalog now doesn't contain the IBar dependency.
            var catalog = new TypeCatalog(typeof(Foo));
            var container = new CompositionContainer(catalog);
            var compositionService = new CompositionContainerService(container);

            //// CompositionInfoTextFormatter.Write(new CompositionInfo(catalog, container), Console.Out);

            var binding = new Binding<IFoo>(compositionService, "Foo");
            var result = binding.Evaluate();

            // As appropriate, this now evaluates to false.
            Assert.IsFalse(result);

            // To re-assure this is the proper behavior, the unresolved import causes the 
            // component to be unavailable via a plain query:
            var lazyValue = container.GetExports<IFoo, IFeatureComponentMetadata>()
                .FirstOrDefault(component => component.Metadata.Id == "Foo");
            Assert.IsNull(lazyValue);

            // Here we provide the missing export.
            container.ComposeExportedValue<IBar>(new Bar());

            // CompositionInfoTextFormatter.Write(new CompositionInfo(catalog, container), Console.Out);

            // Note how now the component becomes available via query, as 
            // the dependency is now satisfied.
            lazyValue = container.GetExports<IFoo, IFeatureComponentMetadata>()
                .FirstOrDefault(component => component.Metadata.Id == "Foo");
            Assert.IsNotNull(lazyValue);

            // And re-evaluate, which should now locate the component as 
            // its imports are satisfied.
            result = binding.Evaluate();

            // But this fails :(
            Assert.IsTrue(result);
        }

        public interface IFoo
        {
        }

        [FeatureComponent(typeof(IFoo), Id = "Foo", Category = "Category", Description = "Description", DisplayName = "DisplayName")]
        [FeatureComponentCatalog(typeof(Foo))]
        public class Foo : IFoo
        {
            [Import]
            public IBar Bar { get; set; }
        }

        [Export(typeof(IBar))]
        public class Bar : IBar
        {
        }

        public interface IBar
        {
        }

        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class)]
        public sealed class FeatureComponentCatalogAttribute : Attribute
        {
            public FeatureComponentCatalogAttribute(Type exportingType)
            {
                this.ExportingType = exportingType;
            }

            public Type ExportingType { get; private set; }
            public static string CatalogName
            {
                get
                {
                    return Catalog.CatalogName;
                }
            }
        }

        private class CompositionContainerService : IFeatureCompositionService
        {
            private CompositionContainer container;

            public CompositionContainerService(CompositionContainer container)
            {
                this.container = container;
            }

            public void ComposeParts(params object[] attributedParts)
            {
                this.container.ComposeParts(attributedParts);
            }

            public Lazy<T, TMetadataView> GetExport<T, TMetadataView>()
            {
                return this.container.GetExport<T, TMetadataView>();
            }

            public Lazy<T> GetExport<T>()
            {
                return this.container.GetExport<T>();
            }

            public T GetExportedValue<T>()
            {
                return this.container.GetExportedValue<T>();
            }

            public T GetExportedValueOrDefault<T>()
            {
                return this.container.GetExportedValueOrDefault<T>();
            }

            public IEnumerable<T> GetExportedValues<T>()
            {
                return this.container.GetExportedValues<T>();
            }

            public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>()
            {
                return this.container.GetExports<T, TMetadataView>();
            }

            public IEnumerable<Lazy<T>> GetExports<T>()
            {
                return this.container.GetExports<T>();
            }

            public void SatisfyImportsOnce(ComposablePart part)
            {
                this.container.SatisfyImportsOnce(part);
            }

            public void Dispose()
            {
            }


            public Lazy<T, TMetadataView> GetExport<T, TMetadataView>(string contractName)
            {
                return this.container.GetExport<T, TMetadataView>(contractName);
            }

            public Lazy<T> GetExport<T>(string contractName)
            {
                return this.container.GetExport<T>(contractName);
            }

            public T GetExportedValue<T>(string contractName)
            {
                return this.container.GetExportedValue<T>(contractName);
            }

            public T GetExportedValueOrDefault<T>(string contractName)
            {
                return this.container.GetExportedValueOrDefault<T>(contractName);
            }

            public IEnumerable<T> GetExportedValues<T>(string contractName)
            {
                return this.container.GetExportedValues<T>(contractName);
            }

            public IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>(string contractName)
            {
                return this.container.GetExports<T, TMetadataView>(contractName);
            }

            public IEnumerable<Lazy<T>> GetExports<T>(string contractName)
            {
                return this.container.GetExports<T>(contractName);
            }
        }
    }
}
