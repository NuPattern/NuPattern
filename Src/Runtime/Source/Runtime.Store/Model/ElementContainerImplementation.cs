using System;
using System.Globalization;
using NuPattern.Modeling;
using NuPattern.Runtime.Store.Properties;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// Provides a consistent implementation of <see cref="IElementContainer"/> which is the 
    /// common interface for abstract elements and views.
    /// </summary>
    /// <typeparam name="TOwner">The type of the owner.</typeparam>
    internal class ElementContainerImplementation<TOwner>
        where TOwner : InstanceBase, IElementContainer
    {
        private TOwner owner;
        private IElementContainer container;

        public ElementContainerImplementation(TOwner owner)
        {
            Guard.NotNull(() => owner, owner);

            this.owner = owner;
            this.container = owner;
        }

        /// <summary>
        /// Creates an instance of a child <see cref="ICollection"/> with an optional initializer to perform 
        /// object initialization within the creation transaction.
        /// </summary>
        public ICollection CreateCollection(Action<ICollection> initializer = null, bool raiseInstantiateEvents = true)
        {
            using (var tx = this.owner.Store.TransactionManager.BeginTransaction("CreateCollection", this.owner.IsSerializing))
            {
                using (var bag = new StorePropertyBag(this.owner.Store, ProductState.IsCreatingElementKey, true))
                {
                    var instance = this.owner.Create<Collection>(raiseInstantiateEvents);
                    SetOwner(instance);

                    if (initializer != null)
                    {
                        initializer(instance);
                    }

                    tx.Commit();

                    return instance;
                }
            }
        }

        /// <summary>
        /// Creates an instance of a child <see cref="IElement"/> with an optional initializer to perform 
        /// object initialization within the creation transaction.
        /// </summary>
        public IElement CreateElement(Action<IElement> initializer = null, bool raiseInstantiateEvents = true)
        {
            using (var tx = this.owner.Store.TransactionManager.BeginTransaction("CreateElement", this.owner.IsSerializing))
            {
                using (var bag = new StorePropertyBag(this.owner.Store, ProductState.IsCreatingElementKey, true))
                {
                    var instance = this.owner.Create<Element>(raiseInstantiateEvents);
                    SetOwner(instance);

                    if (initializer != null)
                    {
                        initializer(instance);
                    }

                    tx.Commit();

                    return instance;
                }
            }
        }

        /// <summary>
        /// Creates an instance of a child <see cref="IProduct"/> with an optional initializer to perform 
        /// object initialization within the creation transaction.
        /// </summary>
        public IProduct CreateExtension(Action<IProduct> initializer = null, bool raiseInstantiateEvents = false)
        {
            if (this.container.Info == null)
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ElementContainerImplementation_MisingElementContainerInfo,
                    this.container.DefinitionName));

            using (var tx = this.owner.Store.TransactionManager.BeginTransaction("CreateExtension", this.owner.IsSerializing))
            using (var inner = this.owner.Store.TransactionManager.BeginTransaction("CreateProduct", this.owner.IsSerializing))
            {
                using (var bag = new StorePropertyBag(this.owner.Store, ProductState.IsCreatingElementKey, true))
                {
                    var instance = (IProduct)this.owner.Create<Product>(raiseInstantiateEvents);
                    var view = this.owner as IView;
                    var abstractElement = this.owner as IAbstractElement;

                    if (view != null)
                        instance.View = view;
                    else if (abstractElement != null)
                        instance.Owner = abstractElement;
                    else
                        // Should never happen.
                        throw new NotSupportedException();

                    if (initializer != null)
                    {
                        initializer(instance);
                    }

                    // Need to commit the inner transaction so that the info is filled by 
                    // the add rule, before checking for a valid extension point info.
                    inner.Commit();

                    // Verify that the info contains a supported extension point identifier.
                    instance.ThrowIfInvalidExtension(this.container.Info.ExtensionPoints);

                    //TODO: Merge ExtensionPoint Info

                    tx.Commit();
                    return instance;
                }
            }
        }

        private void SetOwner(IAbstractElement element)
        {
            var view = this.owner as IView;
            var abstractElement = this.owner as IAbstractElement;

            if (view != null)
                element.View = view;
            else if (abstractElement != null)
                element.Owner = abstractElement;
            else
                // Should never happen.
                throw new NotSupportedException();
        }
    }
}
