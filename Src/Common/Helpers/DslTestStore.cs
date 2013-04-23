using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Moq;
using NuPattern.Runtime.Composition;

namespace NuPattern
{
    internal class DslTestStore<TDomainModel> : IDisposable
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "DSL Store")]
        public DslTestStore()
            : this(null)
        {
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "DSL Store")]
        public DslTestStore(Type extendedDomainModel)
        {
            this.ServiceProvider = Mock.Of<IServiceProvider>(sp =>
                sp.GetService(typeof(INuPatternCompositionService)) == Mock.Of<INuPatternCompositionService>());

            if (extendedDomainModel == null)
            {
                this.Store = new Store(
                    this.ServiceProvider,
                    typeof(CoreDesignSurfaceDomainModel),
                    typeof(TDomainModel));
            }
            else
            {
                this.Store = new Store(
                    this.ServiceProvider,
                    typeof(CoreDesignSurfaceDomainModel),
                    typeof(TDomainModel),
                    extendedDomainModel);
            }
        }

        public ElementFactory ElementFactory
        {
            get { return this.Store.ElementFactory; }
        }

        public IServiceProvider ServiceProvider { get; private set; }

        public Store Store { get; private set; }

        public TransactionManager TransactionManager
        {
            get { return this.Store.TransactionManager; }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.Store != null)
            {
                this.Store.Dispose();
            }
        }
    }
}