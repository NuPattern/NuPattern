using System;
using System.Collections.Generic;
using System.Linq;
using NuPattern.Runtime.Extensibility;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// Base class for collection and element.
    /// </summary>
    partial class AbstractElement : IElementContainer
    {
        private ElementContainerImplementation<AbstractElement> containerImplementation;

        private ElementContainerImplementation<AbstractElement> ContainerImplementation
        {
            get
            {
                if (this.containerImplementation == null)
                    this.containerImplementation = new ElementContainerImplementation<AbstractElement>(this);

                return this.containerImplementation;
            }
        }

        IElementInfoContainer IElementContainer.Info
        {
            get { return this.Info; }
        }

        IEnumerable<IAbstractElement> IElementContainer.Elements
        {
            get { return this.Elements.Cast<IAbstractElement>(); }
        }

        IEnumerable<IProduct> IElementContainer.Extensions
        {
            get { return this.ExtensionProducts.Cast<IProduct>(); }
        }

        IEnumerable<IProductElement> IElementContainer.AllElements
        {
            get
            {
                return ((IElementContainer)this).Elements.Cast<IProductElement>()
                    .Concat(((IElementContainer)this).Extensions.Cast<IProductElement>())
                    .Order();
            }
        }

        /// <summary>
        /// Creates an instance of a child <see cref="ICollection"/> with an optional initializer to perform
        /// object initialization within the creation transaction. The child is automatically added to the
        /// <see cref="IElementContainer.Elements"/> property.
        /// </summary>
        public ICollection CreateCollection(Action<ICollection> initializer, bool raiseInstantiateEvents = true)
        {
            return this.ContainerImplementation.CreateCollection(initializer, raiseInstantiateEvents);
        }

        /// <summary>
        /// Creates an instance of a child <see cref="IElement"/> with an optional initializer to perform
        /// object initialization within the creation transaction. The child is automatically added to the
        /// <see cref="IElementContainer.Elements"/> property.
        /// </summary>
        public IElement CreateElement(Action<IElement> initializer, bool raiseInstantiateEvents = true)
        {
            return this.ContainerImplementation.CreateElement(initializer, raiseInstantiateEvents);
        }

        /// <summary>
        /// Creates an extension <see cref="IProduct"/> with an optional initializer to perform
        /// object initialization within the creation transaction. The child is automatically added to the
        /// <see cref="IElementContainer.Extensions"/> property.
        /// </summary>
        public IProduct CreateExtension(Action<IProduct> initializer, bool raiseInstantiateEvents = true)
        {
            return this.ContainerImplementation.CreateExtension(initializer, raiseInstantiateEvents);
        }
    }
}