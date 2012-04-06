using System;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Exposes the metadata about a toolkit interface proxy implementation.
	/// </summary>
	public interface IToolkitInterfaceMetadata
	{
		/// <summary>
		/// Gets the identifier of the Visual Studio extension that contains this definition.
		/// </summary>
		string ExtensionId { get; }

		/// <summary>
		/// Gets the definition id of the element represented by this interface layer component.
		/// </summary>
		string DefinitionId { get; }

		/// <summary>
		/// Gets the type that implements the interface layer for the definition element.
		/// </summary>
		Type ProxyType { get; }
	}
}
