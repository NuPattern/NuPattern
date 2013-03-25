using System;
using System.ComponentModel.Composition;

namespace NuPattern.Runtime.ToolkitInterface
{
    /// <summary>
    /// Attribute applied to the interface layer implementation classes 
    /// to signal them as the interface layer implementation.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "MEF attributes are better non-sealed for extensibility.")]
    [AttributeUsage(AttributeTargets.Class)]
    [MetadataAttribute]
    public class ToolkitInterfaceProxyAttribute : InheritedExportAttribute, IToolkitInterfaceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolkitInterfaceProxyAttribute"/> class.
        /// </summary>
        public ToolkitInterfaceProxyAttribute()
            : base(typeof(IToolkitInterface))
        {
        }

        /// <summary>
        /// Gets the identifier of the Visual Studio extension that contains this definition.
        /// </summary>
        public string ExtensionId { get; set; }

        /// <summary>
        /// Gets the definition id of the element represented by this interface layer component.
        /// </summary>
        public string DefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the type that implements the interface layer for the definition element.
        /// </summary>
        public Type ProxyType { get; set; }
    }
}
