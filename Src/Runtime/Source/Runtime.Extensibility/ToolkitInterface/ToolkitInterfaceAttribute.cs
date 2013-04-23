using System;

namespace NuPattern.Runtime.ToolkitInterface
{
    /// <summary>
    /// Annotates the toolkit interface layer types with information about their definition 
    /// and implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class ToolkitInterfaceAttribute : Attribute
    {
        /// <summary>
        /// Gets the identifier of the Visual Studio extension that contains this definition.
        /// </summary>
        public string ExtensionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the definition of this element in the toolkit.
        /// </summary>
        public string DefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the type that implements the interface layer for the definition element.
        /// </summary>
        public Type ProxyType { get; set; }
    }
}
