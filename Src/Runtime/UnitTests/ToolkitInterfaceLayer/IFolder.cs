using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Toolkit14
{
	///	<summary>
	///	Description for WebService.Architecture.Folder
	///	</summary>
	[Description("Description for WebService.Architecture.Folder")]
	[ToolkitInterface(DefinitionId = "ccca4e03-00c1-49cf-bcb7-a5eef9243a71", ProxyType = typeof(Folder))]
	public partial interface IFolder : IToolkitInterface
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

		/// <summary>
		/// Gets all instances of <see cref="IDataContract"/> contained in this element.
		/// </summary>
		IEnumerable<IDataContract> DataContracts { get; }

		/// <summary>
		///	Creates a new <see cref="IDataContract"/> and adds it to the <see cref="DataContracts"/> 
		///	collection, executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
		///	</summary>
		IDataContract CreateDataContract(string name, Action<IDataContract> initializer = null);
	}
}