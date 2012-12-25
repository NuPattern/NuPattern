using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using NuPattern.Runtime;

namespace Toolkit14
{
	///	<summary>
	///	Description for WebService.Architecture.Folder.DataContract
	///	</summary>
	[Description("Description for WebService.Architecture.Folder.DataContract")]
    [ToolkitInterfaceProxy(ExtensionId = "NuPattern", DefinitionId = "85a60694-1cd9-42db-b531-102c987ab947")]
	internal partial class DataContract : IDataContract
	{
		private IAbstractElement target;
		private IAbstractElementProxy<IDataContract> proxy;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataContract"/> class.
		/// </summary>
		public DataContract(IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IDataContract>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}

		[DisplayName("Xsd File")]
		[Category("General")]
		public String XsdFile
		{
			get { return this.proxy.GetValue(() => this.XsdFile); }
			set { this.proxy.SetValue(() => this.XsdFile, value); }
		}

		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[ParenthesizePropertyName(true)]
		[Description("The name of this element instance.")]
		public String InstanceName
		{
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}

		///	<summary>
		///	Description for NuPattern.Runtime.Store.ProductElementHasReferences.ProductElement
		///	</summary>
		[Description("Description for NuPattern.Runtime.Store.ProductElementHasReferences.ProductElement")]
		public IEnumerable<IReference> References
		{
			get { return this.proxy.GetValue(() => this.References); }
		}

		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		[Description("Notes for this element.")]
		public String Notes
		{
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}

		/// <summary>
		/// Gets all instances of <see cref="ISerializer"/> contained in this element.
		/// </summary>
		public IEnumerable<ISerializer> Serializers
		{
			get { return proxy.GetExtensions(() => this.Serializers, element => new Serializer(element)); }
		}
	}
}