using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Represents a WCF channel.
	/// </summary>
	/// <typeparam name="TChannel">The type of the channel.</typeparam>
	public class ServiceClient<TChannel> : ClientBase<TChannel>
		where TChannel : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceClient&lt;TChannel&gt;"/> class.
		/// </summary>
		public ServiceClient()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceClient&lt;TChannel&gt;"/> class.
		/// </summary>
		/// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
		public ServiceClient(string endpointConfigurationName) :
			base(endpointConfigurationName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceClient&lt;TChannel&gt;"/> class.
		/// </summary>
		/// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
		/// <param name="remoteAddress">The remote address.</param>
		public ServiceClient(string endpointConfigurationName, string remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceClient&lt;TChannel&gt;"/> class.
		/// </summary>
		/// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
		/// <param name="remoteAddress">The remote address.</param>
		public ServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceClient&lt;TChannel&gt;"/> class.
		/// </summary>
		/// <param name="binding">The binding.</param>
		/// <param name="remoteAddress">The remote address.</param>
		public ServiceClient(Binding binding, EndpointAddress remoteAddress) :
			base(binding, remoteAddress)
		{
		}

		/// <summary>
		/// Gets the channel.
		/// </summary>
		/// <value>The channel.</value>
		public new TChannel Channel
		{
			get { return base.Channel; }
		}
	}
}