using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;
using DslModeling = global::Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// The persistence state for all current products in the solution.
    /// </summary>
    partial class ProductState
    {
        /// <summary>
        /// Key in the <see cref="PropertyBag"/> that flags 
        /// whether the state is being deserialized.
        /// </summary>
        public const string IsSerializingKey = "IsSerializing";

        /// <summary>
        /// Key in the <see cref="PropertyBag"/> that flags 
        /// whether an element is being created.
        /// </summary>
        public const string IsCreatingElementKey = "IsCreatingElement";

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductState"/> class.
        /// </summary>
        /// <param name="store">Store where new element is to be created.</param>
        /// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
        public ProductState(DslModeling::Store store, params DslModeling::PropertyAssignment[] propertyAssignments)
            : this(store != null ? store.DefaultPartitionForClass(DomainClassId) : null, propertyAssignments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductState"/> class.
        /// </summary>
        /// <param name="partition">Partition where new element is to be created.</param>
        /// <param name="propertyAssignments">List of domain property id/value pairs to set once the element is created.</param>
        public ProductState(DslModeling::Partition partition, params DslModeling::PropertyAssignment[] propertyAssignments)
            : base(partition, propertyAssignments)
        {
            this.Store.EventManagerDirectory.TransactionCommitted.Add(new EventHandler<DslModeling.TransactionCommitEventArgs>(this.OnTransactionCommited));

            this.Store.EventManagerDirectory.ElementEventsBegun.Add(new EventHandler<DslModeling.ElementEventsBegunEventArgs>(this.OnElementEventsBegun));
            this.Store.EventManagerDirectory.ElementEventsEnded.Add(new EventHandler<DslModeling.ElementEventsEndedEventArgs>(this.OnElementEventsEnded));

            this.Store.EventManagerDirectory.ElementDeleted.Add(new EventHandler<DslModeling.ElementDeletedEventArgs>(this.OnElementDeleted));
            this.Store.EventManagerDirectory.ElementAdded.Add(new EventHandler<DslModeling.ElementAddedEventArgs>(this.OnElementAdded));
            this.Store.EventManagerDirectory.ElementPropertyChanged.Add(new EventHandler<DslModeling.ElementPropertyChangedEventArgs>(this.OnElementPropertyChanged));
        }

        /// <summary>
        /// Event raised whenever a dynamic element was deleted at runtime.
        /// </summary>
        public event EventHandler<ValueEventArgs<IInstanceBase>> ElementDeleted = (s, e) => { };

        /// <summary>
        /// Event raised whenever a dynamic element is being deleted at runtime.
        /// </summary>
        public event EventHandler<ValueEventArgs<IInstanceBase>> ElementDeleting = (s, e) => { };

        /// <summary>
        /// Event raised whenever a dynamic element is instantiated at runtime, except on serialization.
        /// </summary>
        public event EventHandler<ValueEventArgs<IInstanceBase>> ElementInstantiated = (s, e) => { };

        /// <summary>
        /// Event raised whenever a dynamic element is instantiated at runtime.
        /// </summary>
        public event EventHandler<ValueEventArgs<IInstanceBase>> ElementCreated = (s, e) => { };

        /// <summary>
        /// Event raised whenever a dynamic element is serialized at runtime.
        /// </summary>
        public event EventHandler<ValueEventArgs<IInstanceBase>> ElementLoaded = (s, e) => { };

        /// <summary>
        /// Event raised whenever a dynamic element property is changed, where the sender is the changed element.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged = (s, e) => { };

        /// <summary>
        /// Event raised whenever the state is saved to the file system.
        /// </summary>
        public event EventHandler Saved = (s, e) => { };

        /// <summary>
        /// Event raised whenever a top-level transaction is commited to the state.
        /// </summary>
        public event EventHandler TransactionCommited = (s, e) => { };

        // A transaction that we initiate on element events begin so that 
        // all events always run within a single parent transaction.
        private Stack<ITransaction> eventsTransactions = new Stack<ITransaction>();

        /// <summary>
        /// Gets instances of a given type.
        /// </summary>
        /// <typeparam name="T">The Given Type.</typeparam>
        public IEnumerable<T> FindAll<T>() where T : IInstanceBase
        {
            return this.Store.ElementDirectory.AllElements.OfType<T>();
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public void Validate()
        {
            var patternManager = this.Store.GetService<IPatternManager>();

            patternManager.ValidateProductState(this);
        }

        /// <summary>
        /// Returns whether the state containing this instance is currently 
        /// in a transaction or not.
        /// </summary>
        public bool InTransaction
        {
            get { return this.Store.TransactionActive; }
        }

        /// <summary>
        /// Returns whether the state is currently being serialized or not.
        /// </summary>
        public bool IsSerializing
        {
            get
            {
                object isSerializing;

                return this.Store.PropertyBag.TryGetValue(IsSerializingKey, out isSerializing) && (bool)isSerializing;
            }
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public ITransaction BeginTransaction()
        {
            return new ModelingTransaction(this.Store.TransactionManager.BeginTransaction());
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public ITransaction BeginTransaction(string name)
        {
            return new ModelingTransaction(this.Store.TransactionManager.BeginTransaction(name));
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        public object GetService(Type serviceType)
        {
            Guard.NotNull(() => serviceType, serviceType);

            return this.Store.GetService(serviceType);
        }

        /// <summary>
        /// Deletes a single pattern from the state.
        /// </summary>
        public void Delete(IProduct product)
        {
            this.WithTransaction(s => s.Products.Remove((Product)product));
        }

        /// <summary>
        /// Gets the property bag.
        /// </summary>
        public Dictionary<object, object> PropertyBag
        {
            get { return this.Store.PropertyBag; }
        }

        internal void RaiseSaved()
        {
            this.Saved(this, EventArgs.Empty);
        }

        private void OnElementEventsBegun(object sender, DslModeling.ElementEventsBegunEventArgs e)
        {
            this.eventsTransactions.Push(this.BeginTransaction());
        }

        private void OnElementEventsEnded(object sender, DslModeling.ElementEventsEndedEventArgs e)
        {
            if (this.eventsTransactions.Count > 0)
            {
                // A second round of events from this commit 
                // should still be wrapped in another transaction
                // until we are completely done. 
                var transaction = this.eventsTransactions.Pop();
                transaction.Commit();
                transaction.Dispose();
            }
        }

        internal void OnElementDeleting(object sender, DslModeling.ElementDeletingEventArgs e)
        {
            var element = e.ModelElement as IInstanceBase;
            if (element != null && !this.IsSerializing)
            {
                StoreEventBufferingScope.RaiseOrBufferEvent(() =>
                {
                    var eventArgs = ValueEventArgs.Create(element);
                    this.ElementDeleting(this, eventArgs);
                });
            }
        }

        private void OnTransactionCommited(object sender, DslModeling.TransactionCommitEventArgs e)
        {
            if (!this.IsSerializing && !e.Transaction.IsNested && !e.Transaction.IsSerializing)
            {
                StoreEventBufferingScope.RaiseOrBufferEvent(() =>
                {
                    this.TransactionCommited(this, e);
                });
            }
        }

        private void OnElementDeleted(object sender, DslModeling.ElementDeletedEventArgs e)
        {
            var element = e.ModelElement as IInstanceBase;
            if (element != null && !this.IsSerializing)
            {
                StoreEventBufferingScope.RaiseOrBufferEvent(() =>
                {
                    var eventArgs = ValueEventArgs.Create(element);
                    this.ElementDeleted(this, eventArgs);
                });
            }
        }

        private void OnElementAdded(object sender, DslModeling.ElementAddedEventArgs e)
        {
            var element = e.ModelElement as IInstanceBase;
            if (element != null)
            {
                // ElementCreated is raised always, regardless of how the element was created.
                StoreEventBufferingScope.RaiseOrBufferEvent(() =>
                {
                    var eventArgs = ValueEventArgs.Create(element);
                    this.ElementCreated(this, eventArgs);
                });

                if (!this.IsSerializing)
                {
                    // We only invoke this event if the element has not been marked in the state.
                    // it wants instantiation events (the default is true, see StoreEventBufferingScope ctor).
                    if (ShouldRaiseInstantiate(e.ModelElement))
                    {
                        StoreEventBufferingScope.RaiseOrBufferEvent(() =>
                        {
                            var eventArgs = ValueEventArgs.Create(element);
                            this.ElementInstantiated(this, eventArgs);
                        });
                    }
                }
                else
                {
                    // We're deserializing the element, so we raise the Loaded event.
                    StoreEventBufferingScope.RaiseOrBufferEvent(() =>
                    {
                        var eventArgs = ValueEventArgs.Create(element);
                        this.ElementLoaded(this, eventArgs);
                    });
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ElementPropertyChanged"/> event as long as the state has not been flagged as 
        /// being deserialized.
        /// </summary>
        private void OnElementPropertyChanged(object sender, DslModeling.ElementPropertyChangedEventArgs args)
        {
            // For element-specific automation, this is not used. This is only for automation 
            // that wants to listen to changes on everything.

            // Note that we don't raise the property changed events if the element is either being deserialized 
            // of if it's marked as a creation of the element by the calling code (i.e. ElementContainerImplementation on all its Create 
            // overloads).
            if (!this.IsSerializing && !this.IsCreatingElement())
            {
                StoreEventBufferingScope.RaiseOrDropEvent(() =>
                {
                    this.ElementPropertyChanged(args.ModelElement, new PropertyChangedEventArgs(args.DomainProperty.Name));

                    var property = args.ModelElement as Property;
                    if (property != null && property.Info != null)
                    {
                        // For our variable properties, we also raise the "logical" property 
                        // changed event on the element for its variable property name.
                        this.ElementPropertyChanged(property.Owner, new PropertyChangedEventArgs(property.Info.Name));
                    }
                });
            }
        }

        private bool IsCreatingElement()
        {
            object isCreating;

            return this.Store.PropertyBag.TryGetValue(IsCreatingElementKey, out isCreating) && (bool)isCreating;
        }

        private bool ShouldRaiseInstantiate(DslModeling.ModelElement element)
        {
            return !this.Store.PropertyBag.ContainsKey(element);
        }
    }
}