using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Toolkit14
{
	///	<summary>
	///	Description for WebService.Architecture.Folder.DataContract.Serializer
	///	</summary>
	[Description("Description for WebService.Architecture.Folder.DataContract.Serializer")]
	[ToolkitInterface(DefinitionId = "fe5452b1-427d-4295-8864-f021d6780785", ProxyType = typeof(Serializer))]
	public partial interface ISerializer : IToolkitInterface
	{
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[ParenthesizePropertyName(true)]
		[Description("The name of this element instance.")]
		String InstanceName { get; set; }

		///	<summary>
		///	Description for Microsoft.VisualStudio.Patterning.Runtime.Store.ProductElementHasReferences.ProductElement
		///	</summary>
		[Description("Description for Microsoft.VisualStudio.Patterning.Runtime.Store.ProductElementHasReferences.ProductElement")]
		IEnumerable<IReference> References { get; }

		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		[Description("Notes for this element.")]
		String Notes { get; set; }
	}
}