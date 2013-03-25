using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using NuPattern.Runtime.ToolkitInterface;

namespace NuPattern.Runtime.UnitTests.ToolkitInterface
{
    ///	<summary>
    ///	Description for WebService.Architecture.Folder
    ///	</summary>
    [Description("Description for WebService.Architecture.Folder")]
    [ToolkitInterfaceProxy(ExtensionId = "NuPattern", DefinitionId = "ccca4e03-00c1-49cf-bcb7-a5eef9243a71")]
    internal partial class Folder : IFolder
    {
        private IAbstractElementProxy<IFolder> proxy;
        private IAbstractElement target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class.
        /// </summary>
        public Folder(IAbstractElement target)
        {
            this.target = target;
            this.proxy = target.ProxyFor<IFolder>();
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

        /// <summary>
        /// Gets all instances of <see cref="IDataContract"/> contained in this element.
        /// </summary>
        public IEnumerable<IDataContract> DataContracts
        {
            get { return proxy.GetElements(() => this.DataContracts, element => new DataContract(element)); }
        }

        /// <summary>
        ///	Creates a new <see cref="IDataContract"/> and adds it to the <see cref="DataContracts"/> 
        ///	collection, executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        public IDataContract CreateDataContract(string name, Action<IDataContract> initializer = null)
        {
            return proxy.CreateElement<IDataContract>(name, initializer);
        }
    }
}