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
using System.ComponentModel.Design;
using System.Drawing.Design;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace ExtensionItem.Automation.GeneratedCode
{
	///	<summary>
	///	A Description of ExtensionItem
	///	</summary>
	[Description("A Description of ExtensionItem")]
	[ToolkitInterface(ExtensionId = "8b378a9d-d437-4ca7-8dd3-318c38dc0969", DefinitionId = "0a7c0dc0-eab1-41a4-8556-c269fc314e02", ProxyType = typeof(ExtensionItem))]
	public partial interface IExtensionItem : IToolkitInterface
	{
		///	<summary>
		///	Provides registration information for the product
		///	</summary>
		[Description("Provides registration information for the product")]
		IProductToolkitInfo ToolkitInfo { get; }

		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		String InstanceName { get; set; }

		///	<summary>
		///	Description for Microsoft.VisualStudio.Patterning.Runtime.Store.ProductElementHasReferences.ProductElement
		///	</summary>
		[Description("Description for Microsoft.VisualStudio.Patterning.Runtime.Store.ProductElementHasReferences.ProductElement")]
		IEnumerable<IReference> References { get; }

		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		String Notes { get; set; }

		///	<summary>
		///	Description for ExtensionItem.DefaultView
		///	</summary>
		[Description("Description for ExtensionItem.DefaultView")]
		IDefaultView DefaultView { get; }

		///	<summary>
		///	Deletes this element from the store.
		///	</summary>
		void Delete();

		/// <summary>
		/// Gets the generic <see cref="IProduct"/> underlying element.
		/// </summary>
		IProduct AsProduct();
	}
}
