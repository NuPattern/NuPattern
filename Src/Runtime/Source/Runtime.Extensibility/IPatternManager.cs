using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Defines contracts to manage patterns in an instance of Visual Studio.
    /// </summary>
    public interface IPatternManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when an element in the pattern is activated.
        /// </summary>
        event EventHandler<ValueEventArgs<IProductElement>> ElementActivated;

        /// <summary>
        /// Occurs when an element was deleted.
        /// </summary>
        event EventHandler<ValueEventArgs<IProductElement>> ElementDeleted;

        /// <summary>
        /// Occurs when an element is being deleted.
        /// </summary>
        event EventHandler<ValueEventArgs<IProductElement>> ElementDeleting;

        /// <summary>
        /// Occurs when an element is created.
        /// </summary>
        event EventHandler<ValueEventArgs<IProductElement>> ElementCreated;

        /// <summary>
        /// Occurs when an element is instantiated.
        /// </summary>
        event EventHandler<ValueEventArgs<IProductElement>> ElementInstantiated;

        /// <summary>
        /// Occurs when an product state is saved.
        /// </summary>
        event EventHandler<ValueEventArgs<IProductState>> StoreSaved;

        /// <summary>
        /// Occurs when the pattern manager opens or closes a state.
        /// </summary>
        event EventHandler IsOpenChanged;

        /// <summary>
        /// Gets the installed toolkits.
        /// </summary>
        IEnumerable<IInstalledToolkitInfo> InstalledToolkits { get; }

        /// <summary>
        /// Gets the instantiated products.
        /// </summary>
        IEnumerable<IProduct> Products { get; }

        /// <summary>
        /// Gets the opened state, if <see cref="IsOpen"/> is <see langword="true"/>.
        /// </summary>
        IProductState Store { get; }

        /// <summary>
        /// Gets the opened state file, if <see cref="IsOpen"/> is <see langword="true"/>.
        /// </summary>
        string StoreFile { get; }

        /// <summary>
        /// Gets a value indicating whether the pattern manager has opened a state.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Opens the specified pattern state.
        /// </summary>
        /// <param name="storeFile">Name of the file.</param>
        /// <param name="autoCreate">Whether to automatically create an empty state file if the 
        /// given <paramref name="storeFile"/> does not exist or is blank.</param>
        void Open(string storeFile, bool autoCreate = false);

        /// <summary>
        /// Saves all current products to the underlying state.
        /// </summary>
        void Save();

        /// <summary>
        /// Saves all current products to the underlying state with a new name
        /// </summary>
        /// <param name="fileName">The new file name for the state, without directory</param>
        void SaveAs(string fileName);

        /// <summary>
        /// Saves all pending changes to the underlying state, and closes the state if it was opened, otherwise throws <see cref="InvalidOperationException"/>.
        /// </summary>
        void Close();

        /// <summary>
        /// Activates the element.
        /// </summary>
        void ActivateElement(IProductElement element);

        /// <summary>
        /// Creates the pattern from the specified <see cref="IInstalledToolkitInfo"/>.
        /// </summary>
        /// <param name="toolkitInfo">The toolkit info.</param>
        /// <param name="name">The pattern name.</param>
        /// <param name="raiseInstantiateEvents">Whether instantiation events should be raised.</param>
        IProduct CreateProduct(IInstalledToolkitInfo toolkitInfo, string name, bool raiseInstantiateEvents = true);

        /// <summary>
        /// Deletes the specified product.
        /// </summary>
        /// <param name="product">The product.</param>
        bool DeleteProduct(IProduct product);

        /// <summary>
        /// Validates the specified elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        bool Validate(IEnumerable<IInstanceBase> elements);

        /// <summary>
        /// Validates the product state.
        /// </summary>
        void ValidateProductState();

        /// <summary>
        /// Validates the product state.
        /// </summary>
        void ValidateProductState(IProductState state);
    }
}
