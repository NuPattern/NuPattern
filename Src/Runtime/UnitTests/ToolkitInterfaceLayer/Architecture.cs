using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Toolkit14
{
	///	<summary>
	///	Description for WebService.Architecture
	///	</summary>
	[Description("Description for WebService.Architecture")]
    [ToolkitInterfaceProxy(ExtensionId = "The Outercurve Foundation", DefinitionId = "decb63d3-5dd6-488c-a606-65e01a232320")]
	public partial class Architecture : IArchitecture
	{
		private IView target;
		private IContainerProxy<IArchitecture> proxy;

		/// <summary>
		/// Initializes a new instance of the <see cref="Architecture"/> class.
		/// </summary>
		public Architecture(IView target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IArchitecture>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}

		/// <summary>
		/// Gets all instances of <see cref="IFolder"/> contained in this element.
		/// </summary>
		public IEnumerable<IFolder> Folders
		{
			get { return proxy.GetElements(() => this.Folders, element => new Folder(element)); }
		}

		/// <summary>
		///	Creates a new <see cref="IFolder"/> and adds it to the <see cref="Folders"/> 
		///	collection, executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
		///	</summary>
		public IFolder CreateFolder(string name, Action<IFolder> initializer = null)
		{
			return proxy.CreateCollection<IFolder>(name, initializer);
		}

		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public void Delete()
		{
			this.target.Delete();
		}
	}
}