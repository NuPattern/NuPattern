using System;
using System.ComponentModel;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Defines a type descriptor context for delegated descriptors.
    /// </summary>
    public class DelegatingTypeDescriptorContext : ITypeDescriptorContext
    {
        private ITypeDescriptorContext innerTypeDescriptor;
        private PropertyDescriptor innerPropertyDescriptor;

        /// <summary>
        /// Creates a new instance of the <see cref="DelegatingTypeDescriptorContext"/> class.
        /// </summary>
        public DelegatingTypeDescriptorContext(ITypeDescriptorContext innerTypeDescriptor)
        {
            this.innerTypeDescriptor = innerTypeDescriptor;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DelegatingTypeDescriptorContext"/> class.
        /// </summary>
        public DelegatingTypeDescriptorContext(ITypeDescriptorContext innerTypeDescriptor, PropertyDescriptor innerPropertyDescriptor)
        {
            this.innerTypeDescriptor = innerTypeDescriptor;
            this.innerPropertyDescriptor = innerPropertyDescriptor;
        }

        /// <summary>
        /// Gets the container property of the descriptor.
        /// </summary>
        public IContainer Container
        {
            get
            {
                return innerTypeDescriptor.Container;
            }
        }

        /// <summary>
        /// Gets the instance property of the descriptor.
        /// </summary>
        public object Instance
        {
            get
            {
                return innerTypeDescriptor.Instance;
            }
        }

        /// <summary>
        /// Called when the component property changes.
        /// </summary>
        public void OnComponentChanged()
        {
            innerTypeDescriptor.OnComponentChanged();
        }

        /// <summary>
        /// Called when the component property is changing.
        /// </summary>
        /// <returns></returns>
        public bool OnComponentChanging()
        {
            return innerTypeDescriptor.OnComponentChanging();
        }

        /// <summary>
        /// Gets the wrapped delegating descriptor.
        /// </summary>
        public PropertyDescriptor PropertyDescriptor
        {
            get
            {
                if (innerPropertyDescriptor != null)
                {
                    return innerPropertyDescriptor;
                }
                return innerTypeDescriptor.PropertyDescriptor;
            }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return innerTypeDescriptor.GetService(serviceType);
        }
    }
}
