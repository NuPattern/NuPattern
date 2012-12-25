using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Provides a <see cref="IServiceProvider"/> that wraps another and allow add and remove local services.
	/// </summary>
	internal class ProxyServiceProvider : IServiceProvider, IServiceContainer
	{
		private IServiceProvider serviceProvider;
		private Dictionary<Type, ServiceCreatorCallback> services = new Dictionary<Type, ServiceCreatorCallback>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ProxyServiceProvider"/> class.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		public ProxyServiceProvider(IServiceProvider serviceProvider)
		{
			Guard.NotNull(() => serviceProvider, serviceProvider);

			this.serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <param name="serviceType">An object that specifies the type of service object to get.</param>
		/// <returns>
		/// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
		/// </returns>
		public object GetService(Type serviceType)
		{
			Guard.NotNull(() => serviceType, serviceType);

			ServiceCreatorCallback callback;
			if (services.TryGetValue(serviceType, out callback))
			{
				return callback(this, serviceType);
			}

			return this.serviceProvider.GetService(serviceType);
		}

		/// <summary>
		/// Adds the specified service to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested.</param>
		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			Guard.NotNull(() => serviceType, serviceType);
			Guard.NotNull(() => callback, callback);

			this.services.Add(serviceType, callback);
		}

		/// <summary>
		/// Adds the specified service to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the <paramref name="serviceType"/> parameter.</param>
		public void AddService(Type serviceType, object serviceInstance)
		{
			Guard.NotNull(() => serviceType, serviceType);
			Guard.NotNull(() => serviceInstance, serviceInstance);

			this.services.Add(serviceType, new ServiceCreatorCallback((s, t) => serviceInstance));
		}

		/// <summary>
		/// Removes the specified service type from the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		public void RemoveService(Type serviceType)
		{
			Guard.NotNull(() => serviceType, serviceType);

			this.services.Remove(serviceType);
		}

		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			throw new NotImplementedException();
		}

		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
		{
			throw new NotImplementedException();
		}

		void IServiceContainer.RemoveService(Type serviceType, bool promote)
		{
			throw new NotImplementedException();
		}
	}
}