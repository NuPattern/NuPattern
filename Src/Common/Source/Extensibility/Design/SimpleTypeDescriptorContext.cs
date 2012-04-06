using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Simple type descriptor context
    /// </summary>
    public class SimpleTypeDescriptorContext : ITypeDescriptorContext
    {
        /// <summary>
        /// Gets the container representing this <see cref="T:System.ComponentModel.TypeDescriptor"/> request.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.ComponentModel.IContainer"/> with the set of objects for this <see cref="T:System.ComponentModel.TypeDescriptor"/>; otherwise, null if there is no container or if the <see cref="T:System.ComponentModel.TypeDescriptor"/> does not use outside objects.</returns>
        public IContainer Container { get; set; }

        /// <summary>
        /// Gets the object that is connected with this type descriptor request.
        /// </summary>
        /// <value></value>
        /// <returns>The object that invokes the method on the <see cref="T:System.ComponentModel.TypeDescriptor"/>; otherwise, null if there is no object responsible for the call.</returns>
        public object Instance { get; set; }

        /// <summary>
        /// Gets the <see cref="T:System.ComponentModel.PropertyDescriptor"/> that is associated with the given context item.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.PropertyDescriptor"/> that describes the given context item; otherwise, null if there is no <see cref="T:System.ComponentModel.PropertyDescriptor"/> responsible for the call.</returns>
        public PropertyDescriptor PropertyDescriptor { get; set; }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanged"/> event.
        /// </summary>
        public virtual void OnComponentChanged()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanging"/> event.
        /// </summary>
        /// <returns>
        /// true if this object can be changed; otherwise, false.
        /// </returns>
        public virtual bool OnComponentChanging()
        {
            return true;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public virtual object GetService(Type serviceType)
        {
            if (this.ServiceProvider != null)
            {
                return this.ServiceProvider.GetService(serviceType);
            }

            return null;
        }
    }
}