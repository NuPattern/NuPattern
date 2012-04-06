using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
    /// <summary>
    /// Represents the product state.
    /// </summary>
    public partial interface IProductState : IServiceProvider
    {
        /// <summary>
        /// Event raised whenever a dynamic element is instantiated at runtime.
        /// </summary>
        event EventHandler<ValueEventArgs<IInstanceBase>> ElementCreated;

        /// <summary>
        /// Event raised whenever a dynamic element is deserialized at runtime.
        /// </summary>
        event EventHandler<ValueEventArgs<IInstanceBase>> ElementLoaded;

        /// <summary>
        /// Event raised whenever a dynamic element is instantiated at runtime.
        /// </summary>
        event EventHandler<ValueEventArgs<IInstanceBase>> ElementInstantiated;

        /// <summary>
        /// Event raised whenever a dynamic element was deleted at runtime.
        /// </summary>
        event EventHandler<ValueEventArgs<IInstanceBase>> ElementDeleted;

        /// <summary>
        /// Event raised whenever a dynamic element is being deleted at runtime.
        /// </summary>
        event EventHandler<ValueEventArgs<IInstanceBase>> ElementDeleting;

        /// <summary>
        /// Event raised whenever a dynamic element property is changed, where the sender is the changed element.
        /// </summary>
        event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

        /// <summary>
        /// Event raised whenever the state is saved to the file system.
        /// </summary>
        event EventHandler Saved;

        /// <summary>
        /// Event raised whenever a top-level transaction is commited to the state.
        /// </summary>
        event EventHandler TransactionCommited;

        /// <summary>
        /// Gets instances of a given type.
        /// </summary>
        IEnumerable<T> FindAll<T>() where T : IInstanceBase;

        /// <summary>
        /// Validates all elements in the state.
        /// </summary>
        void Validate();

        /// <summary>
        /// Deletes a single pattern from the state.
        /// </summary>		
        void Delete(IProduct product);

        /// <summary>
        /// Provides an arbitrary transient property bag to state miscelaneous 
        /// state for the entire state.
        /// </summary>
        Dictionary<object, object> PropertyBag { get; }
    }
}