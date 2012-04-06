﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace ExtensionItem.Automation.GeneratedCode
{
	///	<summary>
	///	A Description of ExtensionItem
	///	</summary>
	[Description("A Description of ExtensionItem")]
	[ToolkitInterfaceProxy(ExtensionId = "8b378a9d-d437-4ca7-8dd3-318c38dc0969", DefinitionId = "0a7c0dc0-eab1-41a4-8556-c269fc314e02", ProxyType = typeof(ExtensionItem))]
	internal partial class ExtensionItem : IExtensionItem
	{
		private IProduct target;
		private IProductProxy<IExtensionItem> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private ExtensionItem() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionItem"/> class.
		/// </summary>
		public ExtensionItem(IProduct target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IExtensionItem>();
		}

		///	<summary>
		///	Provides registration information for the product
		///	</summary>
		[Description("Provides registration information for the product")]
		public IProductToolkitInfo ToolkitInfo
		{
			get { return this.proxy.GetValue(() => this.ToolkitInfo); }
		}

		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public String InstanceName
		{
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}

		///	<summary>
		///	Description for Microsoft.VisualStudio.Patterning.Runtime.Store.ProductElementHasReferences.ProductElement
		///	</summary>
		[Description("Description for Microsoft.VisualStudio.Patterning.Runtime.Store.ProductElementHasReferences.ProductElement")]
		public IEnumerable<IReference> References
		{
			get { return this.proxy.GetValue(() => this.References); }
		}

		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public String Notes
		{
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}

		///	<summary>
		///	Description for ExtensionItem.DefaultView
		///	</summary>
		[Description("Description for ExtensionItem.DefaultView")]
		public IDefaultView DefaultView
		{
			get { return this.proxy.GetView(() => this.DefaultView, view => new DefaultView(view)); }
		}

		/// <summary>
		/// Gets the generic <see cref="IProduct"/> underlying element.
		/// </summary>
		public IProduct AsProduct()
		{
			return this.target;
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
		/// Deletes this instance.
		/// </summary>
		public void Delete()
		{
			this.target.Delete();
		}
	}
}
