﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace ExtensionProject.Automation.GeneratedCode
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using NuPattern.Runtime;
    using NuPattern.Runtime.ToolkitInterface;

    ///	<summary>
    ///	Description for ExtensionProject.DefaultView
    ///	</summary>
    [Description("Description for ExtensionProject.DefaultView")]
    [ToolkitInterfaceProxy(ExtensionId = "7bab16a4-5604-441c-bb71-7b0091f96ba3", DefinitionId = "b8523980-b898-49e5-a0f2-109dcd9380c5", ProxyType = typeof(DefaultView))]
    public partial class DefaultView : IDefaultView
    {
        private IView target;
        private IContainerProxy<IDefaultView> proxy;

        /// <summary>
        /// For MEF.
        /// </summary>
        [ImportingConstructor]
        private DefaultView() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultView"/> class.
        /// </summary>
        public DefaultView(IView target)
        {
            this.target = target;
            this.proxy = target.ProxyFor<IDefaultView>();
        }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        public IExtensionProject Parent
        {
            get { return this.target.Parent.As<IExtensionProject>(); }
        }

        /// <summary>
        /// Gets the generic <see cref="IView"/> underlying element.
        /// </summary>
        public IView AsView()
        {
            return this.target;
        }

        /// <summary>
        /// Gets the generic underlying element as the given type if possible.
        /// </summary>
        public TRuntimeInterface As<TRuntimeInterface>()
            where TRuntimeInterface : class
        {
            return this.target as TRuntimeInterface;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Delete()
        {
            this.target.Delete();
        }
    }
}

