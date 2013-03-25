using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using NuPattern.Runtime.ToolkitInterface;

namespace NuPattern.Runtime.UnitTests.ToolkitInterface
{
    ///	<summary>
    ///	Description for WebService.Architecture.Folder.DataContract.Serializer
    ///	</summary>
    [Description("Description for WebService.Architecture.Folder.DataContract.Serializer")]
    [ToolkitInterfaceProxy(ExtensionId = "NuPattern", DefinitionId = "fe5452b1-427d-4295-8864-f021d6780785")]
    internal partial class Serializer : ISerializer
    {
        private IProductProxy<ISerializer> proxy;
        private IProduct target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Serializer"/> class.
        /// </summary>
        public Serializer(IProduct target)
        {
            this.target = target;
            this.proxy = target.ProxyFor<ISerializer>();
        }

        /// <summary>
        /// Gets the generic underlying element as the given type if possible.
        /// </summary>
        public TRuntimeInterface As<TRuntimeInterface>()
            where TRuntimeInterface : class
        {
            return this.target as TRuntimeInterface;
        }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [ParenthesizePropertyName(true)]
        [Description("The name of this element instance.")]
        public String InstanceName
        {
            get { return this.proxy.GetValue(() => this.InstanceName); }
            set { this.proxy.SetValue(() => this.InstanceName, value); }
        }

        ///	<summary>
        ///	Description for NuPattern.Runtime.Store.ProductElementHasReferences.ProductElement
        ///	</summary>
        [Description("Description for NuPattern.Runtime.Store.ProductElementHasReferences.ProductElement")]
        public IEnumerable<IReference> References
        {
            get { return this.proxy.GetValue(() => this.References); }
        }

        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Description("Notes for this element.")]
        public String Notes
        {
            get { return this.proxy.GetValue(() => this.Notes); }
            set { this.proxy.SetValue(() => this.Notes, value); }
        }
    }
}