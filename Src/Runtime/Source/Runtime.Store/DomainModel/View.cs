using System;
using System.Collections.Generic;
using System.Linq;

namespace NuPattern.Runtime.Store
{
    partial class View : IElementContainer
    {
        private ElementContainerImplementation<View> containerImplementation;

        private ElementContainerImplementation<View> ContainerImplementation
        {
            get
            {
                if (this.containerImplementation == null)
                    this.containerImplementation = new ElementContainerImplementation<View>(this);

                return this.containerImplementation;
            }
        }

        /// <devdoc>
        /// Made explicit to avoid collision with the specific Info property.
        /// </devdoc>
        IElementInfoContainer IElementContainer.Info
        {
            get { return this.Info; }
        }

        /// <devdoc>
        /// Made explicit to avoid collision with the Elements property from the embedding relationship.
        /// </devdoc>
        IEnumerable<IAbstractElement> IElementContainer.Elements
        {
            get { return this.Elements.Cast<IAbstractElement>(); }
        }

        /// <devdoc>
        /// Made explicit to avoid collision with the Extensions property from the embedding relationship.
        /// </devdoc>
        IEnumerable<IProduct> IElementContainer.Extensions
        {
            get { return this.ExtensionProducts.Cast<IProduct>(); }
        }

        /// <summary>
        /// Gets the contained elements and extension points.
        /// </summary>
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
        public ICollection CreateCollection(Action<ICollection> initializer = null, bool raiseInstantiateEvents = true)
        {
            return this.ContainerImplementation.CreateCollection(initializer, raiseInstantiateEvents);
        }

        /// <summary>
        /// Creates an instance of a child <see cref="IElement"/> with an optional initializer to perform
        /// object initialization within the creation transaction. The child is automatically added to the
        /// <see cref="IElementContainer.Elements"/> property.
        /// </summary>
        public IElement CreateElement(Action<IElement> initializer = null, bool raiseInstantiateEvents = true)
        {
            return this.ContainerImplementation.CreateElement(initializer, raiseInstantiateEvents);
        }

        /// <summary>
        /// Creates an extension <see cref="IProduct"/> with an optional initializer to perform
        /// object initialization within the creation transaction. The child is automatically added to the
        /// <see cref="IElementContainer.Extensions"/> property.
        /// </summary>
        public IProduct CreateExtension(Action<IProduct> initializer = null, bool raiseInstantiateEvents = true)
        {
            return this.ContainerImplementation.CreateExtension(initializer, raiseInstantiateEvents);
        }
    }
}