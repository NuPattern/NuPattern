using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Store.Properties;
using NuPattern.Runtime.ToolkitInterface;
using NuPattern.Runtime.Validation;

namespace NuPattern.Runtime.Store
{
    [ValidationState(ValidationState.Enabled)]
    partial class ProductElement : IProductElement
    {
        private IDynamicBinding<IValidationRule>[] validationBindings;
        private IDynamicBindingContext bindingContext;
        private static ITraceSource tracer = Tracer.GetSourceFor<ProductElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductElement"/> class.
        /// </summary>
        /// <param name="partition">Partition where new element is to be created.</param>
        /// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
        protected ProductElement(Partition partition, PropertyAssignment[] propertyAssignments)
            : base(partition, propertyAssignments)
        {
            this.AutomationExtensions = new List<IAutomationExtension>();
            partition.PartitionDisposing += this.OnPartitionDisposing;
        }

        /// <summary>
        /// Gets the owning pattern for this element instance.
        /// For an extension pattern this is the owning pattern.
        /// For a non-parented pattern, or elements in a non-parent pattern, this is the root pattern (<see cref="InstanceBase.Root"/>/>.
        /// </summary>
        public IProduct Product
        {
            get
            {
                IInstanceBase current = this;

                // Check for an extension pattern
                var extensionProduct = current as IProduct;
                if (extensionProduct != null && extensionProduct.Parent != null)
                {
                    // Move into parent pattern (view or abstractelement)
                    current = extensionProduct.Parent;
                }

                // Get the parent until we reach the next pattern
                while (current != null && !(current is IProduct))
                {
                    current = current.Parent;
                }

                return current as IProduct;
            }
        }

        /// <summary>
        /// Gets the list of automation extensions.
        /// </summary>
        public IList<IAutomationExtension> AutomationExtensions
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds an automation extension to the pattern
        /// </summary>
        /// <param name="extension">The extension to add</param>
        public void AddAutomationExtension(IAutomationExtension extension)
        {
            var initializable = extension as ISupportInitialize;
            if (initializable != null)
                initializable.BeginInit();

            this.BindingContext.CompositionService.SatisfyImportsOnce(extension);

            if (initializable != null)
                initializable.EndInit();

            AutomationExtensions.Add(extension);
        }

        /// <summary>
        /// Raises the property changed event for the given property.
        /// This method is invoked by the Property whenever its value
        /// changes.
        /// </summary>
        internal void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanges.NotifyChanged(propertyName);
        }

        /// <summary>
        /// Gets the automation extensions.
        /// </summary>
        IEnumerable<IAutomationExtension> IProductElement.AutomationExtensions
        {
            get
            {
                return this.AutomationExtensions;
            }
        }

        /// <summary>
        /// Called by the model before the element is deleted.
        /// </summary>
        protected override void OnDeleting()
        {
            base.OnDeleting();

            if (this.Root != null)
            {
                var store = this.Root.ProductState as ProductState;
                if (store != null)
                {
                    var trans = this.Store.TransactionManager.CurrentTransaction;

                    store.OnElementDeleting(
                        this,
                        new ElementDeletingEventArgs(
                            this.Store.ElementDirectory,
                            this.GetDomainClass(),
                            this.Id,
                            ChangeSource.Normal,
                            (trans != null) ? trans.Context : null));
                }
            }

            this.CleanupAutomationExtensions();
        }

        /// <summary>
        /// Called when the owning partition is being disposed.
        /// </summary>
        private void OnPartitionDisposing(object sender, System.EventArgs e)
        {
            this.CleanupAutomationExtensions();
        }

        private void CleanupAutomationExtensions()
        {
            foreach (var disposable in this.AutomationExtensions.OfType<IDisposable>())
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    tracer.TraceError(ex, Resources.ProductElement_FailedToDisposeExtension);
                }
            }
        }

        private IEnumerable<IDynamicBinding<IValidationRule>> ValidationBindings
        {
            get
            {
                var bindingFactory = EnsureGetService<IBindingFactory>();

                if (this.validationBindings == null)
                {
                    if (this.Info != null)
                    {
                        this.validationBindings = this.Info.ValidationSettings
                                .Select(s => bindingFactory.CreateBinding<IValidationRule>(s)).ToArray();
                    }
                }

                return validationBindings;
            }
        }

        internal TService EnsureGetService<TService>()
        {
            var service = this.Store.GetService<TService>();
            if (service == null)
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Validation_RequiredServiceMissing,
                    typeof(TService)));

            return service;
        }

        private IDynamicBindingContext BindingContext
        {
            get
            {
                if (this.bindingContext == null)
                {
                    var bindingFactory = EnsureGetService<IBindingFactory>();
                    this.bindingContext = bindingFactory.CreateContext();
                    this.bindingContext.AddInterfaceLayer(this);
                    this.bindingContext.AddExportsFromInterfaces(this);
                }

                return this.bindingContext;
            }
        }
    }
}
