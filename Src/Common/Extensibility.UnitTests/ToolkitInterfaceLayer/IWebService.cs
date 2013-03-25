using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.UnitTests.ToolkitInterfaceLayer
{
    ///	<summary>
    ///	Description for WebService
    ///	</summary>
    [Description("Description for WebService")]
    [ToolkitInterface(DefinitionId = "a7d76993-7a93-4bd1-b4f2-1e72af2796a2", ProxyType = typeof(WebService))]
    public partial interface IWebService : IToolkitInterface
    {
        [DisplayName("Is Secured")]
        [Category("General")]
        [TypeConverter(typeof(TrueConverter))]
        Boolean IsSecured { get; set; }

        [DisplayName("Xml Namespace")]
        [Category("General")]
        String XmlNamespace { get; set; }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [ParenthesizePropertyName(true)]
        [Description("The name of this element instance.")]
        String InstanceName { get; set; }

        ///	<summary>
        ///	Description for NuPattern.Runtime.Store.ProductElementHasReferences.ProductElement
        ///	</summary>
        [Description("Description for NuPattern.Runtime.Store.ProductElementHasReferences.ProductElement")]
        IEnumerable<IReference> References { get; }

        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Description("Notes for this element.")]
        String Notes { get; set; }

        ///	<summary>
        ///	Description for WebService.Architecture
        ///	</summary>
        [Description("Description for WebService.Architecture")]
        IArchitecture Architecture { get; }
    }
}