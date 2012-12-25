using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Runtime.Store.Properties;

namespace NuPattern.Runtime.Store
{
    public partial class Property
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<Property>();

        private IDynamicBinding<IValidationRule>[] validationBindings;
        private IDynamicBindingContext bindingContext;
        private PropertyDescriptor descriptor;
        private IDisposable propertyChangedSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="store">Store where new element is to be created.</param>
        /// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
        public Property(Microsoft.VisualStudio.Modeling.Store store, params PropertyAssignment[] propertyAssignments)
            : this(store != null ? store.DefaultPartitionForClass(DomainClassId) : null, propertyAssignments)
        {
            this.propertyChangedSubscription = this.PropertyChanges.AddHandler(OnPropertyChanged);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="partition">Partition where new element is to be created.</param>
        /// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
        public Property(Partition partition, params PropertyAssignment[] propertyAssignments)
            : base(partition, propertyAssignments)
        {
            this.propertyChangedSubscription = this.PropertyChanges.AddHandler(OnPropertyChanged);
        }

        /// <summary>
        /// Gets or sets the typed property value.
        /// </summary>
        public object Value
        {
            get { return this.Descriptor.GetValue(this); }
            set { this.Descriptor.SetValue(this, value); }
        }

        /// <summary>
        /// Resets the property value to its initial value. If a default value 
        /// was specified in the schema, it will be used, as well as a 
        /// value provider, if any.
        /// </summary>
        public void Reset()
        {
            this.Descriptor.ResetValue(this);
        }

        /// <summary>
        /// Lazy retrieves the descriptor for the current property. If the definition id is changed later, it re-creates 
        /// the descriptor.
        /// </summary>
        internal PropertyDescriptor Descriptor
        {
            get
            {
                if (this.descriptor == null)
                {
                    if (this.Info == null)
                    {
                        this.descriptor = new PropertyUnavailableDescriptor(this);
                    }
                    else
                    {
                        var type = this.Info.TryLoad();
                        if (type == null)
                            this.descriptor = new PropertyUnavailableDescriptor(this);
                        else
                            this.descriptor = new PropertyPropertyDescriptor(this, type);
                    }
                }

                return this.descriptor;
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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == this.GetPropertyName(x => x.DefinitionId) ||
                args.PropertyName == this.GetPropertyName(x => x.Info))
            {
                this.descriptor = null;
            }
            else if (args.PropertyName == this.GetPropertyName(x => x.RawValue))
            {
                this.Owner.RaisePropertyChanged(this.DefinitionName);
            }
        }

        private IDynamicBindingContext BindingContext
        {
            get
            {
                if (this.bindingContext == null)
                {
                    var bindingFactory = EnsureGetService<IBindingFactory>();

                    this.bindingContext = bindingFactory.CreateContext();
                    this.bindingContext.AddInterfaceLayer(this.Owner);
                    this.bindingContext.AddExportsFromInterfaces(this.Owner);
                    this.bindingContext.AddExport<IProperty>(this);
                }

                return this.bindingContext;
            }
        }

        private IEnumerable<IDynamicBinding<IValidationRule>> ValidationBindings
        {
            get
            {
                var bindingFactory = EnsureGetService<IBindingFactory>();

                return this.validationBindings ??
                    (this.validationBindings = this.Info.ValidationSettings
                        .Select(s => bindingFactory.CreateBinding<IValidationRule>(s))
                        .Where(rule => rule != null)
                        .ToArray());
            }
        }
    }
}
