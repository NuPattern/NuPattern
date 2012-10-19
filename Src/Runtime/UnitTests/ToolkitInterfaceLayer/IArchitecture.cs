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
	[ToolkitInterface(DefinitionId = "decb63d3-5dd6-488c-a606-65e01a232320", ProxyType = typeof(Architecture))]
	public partial interface IArchitecture : IToolkitInterface
	{
		/// <summary>
		/// Gets all instances of <see cref="IFolder"/> contained in this element.
		/// </summary>
		IEnumerable<IFolder> Folders { get; }

		/// <summary>
		///	Creates a new <see cref="IFolder"/> and adds it to the <see cref="Folders"/> 
		///	collection, executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
		///	</summary>
		IFolder CreateFolder(string name, Action<IFolder> initializer = null);

		void Delete();
	}
}