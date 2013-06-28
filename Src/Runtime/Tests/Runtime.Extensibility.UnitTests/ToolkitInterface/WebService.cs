using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Drawing.Design;
using NuPattern.Runtime.ToolkitInterface;

namespace NuPattern.Runtime.UnitTests.ToolkitInterface
{
    ///	<summary>
    ///	Description for WebService
    ///	</summary>
    [Description("Description for WebService")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ToolkitInterfaceProxy(ExtensionId = "NuPattern", DefinitionId = "a7d76993-7a93-4bd1-b4f2-1e72af2796a2")]
    internal partial class WebService : IWebService
    {
        private IProductProxy<IWebService> proxy;
        private IProduct target;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebService"/> class.
        /// </summary>
        [ImportingConstructor]
        public WebService(IProduct target)
        {
            this.target = target;
            this.proxy = target.ProxyFor<IWebService>();
        }

        /// <summary>
        /// Gets the generic underlying element as the given type if possible.
        /// </summary>
        public TRuntimeInterface As<TRuntimeInterface>()
            where TRuntimeInterface : class
        {
            return this.target as TRuntimeInterface;
        }

        [DisplayName("Is Secured")]
        [Category("General")]
        [TypeConverter(typeof(TrueConverter))]
        public Boolean IsSecured
        {
            get { return this.proxy.GetValue(() => this.IsSecured); }
            set { this.proxy.SetValue(() => this.IsSecured, value); }
        }

        [DisplayName("Xml Namespace")]
        [Category("General")]
        public String XmlNamespace
        {
            get { return this.proxy.GetValue(() => this.XmlNamespace); }
            set { this.proxy.SetValue(() => this.XmlNamespace, value); }
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

        ///	<summary>
        ///	Description for WebService.Architecture
        ///	</summary>
        [Description("Description for WebService.Architecture")]
        public IArchitecture Architecture
        {
            get { return this.proxy.GetView(() => this.Architecture, view => new Architecture(view)); }
        }
    }
}