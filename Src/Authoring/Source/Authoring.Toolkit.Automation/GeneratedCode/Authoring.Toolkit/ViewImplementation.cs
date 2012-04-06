﻿
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.VisualStudio.Patterning.Authoring.Authoring
{
	using global::Microsoft.VisualStudio.Patterning.Runtime;
	using global::System;
	using global::System.Collections.Generic;
	using global::System.ComponentModel;
	using global::System.ComponentModel.Composition;
	using global::System.ComponentModel.Design;
	using global::System.Drawing.Design;
	using Runtime = global::Microsoft.VisualStudio.Patterning.Runtime;

	///	<summary>
	///	The development view of a toolkit.
	///	</summary>
	[Description("The development view of a toolkit.")]
	[ToolkitInterfaceProxy(ExtensionId ="9f6dc301-6f66-4d21-9f9c-b37412b162f6", DefinitionId = "e4f9702a-3b97-4b11-bb76-037a32de07c7", ProxyType = typeof(Development))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class Development : IDevelopment
	{
		private Runtime.IView target;
		private Runtime.IContainerProxy<IDevelopment> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private Development() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Development"/> class.
		/// </summary>
		public Development(Runtime.IView target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IDevelopment>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual IPatternToolkit Parent
		{ 
			get { return this.target.Parent.As<IPatternToolkit>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IView"/> underlying element.
		/// </summary>
		public virtual Runtime.IView AsView()
		{
			return this.target;
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		/// <summary>
		/// Gets the <see cref="IAssets"/> contained in this element.
		/// </summary>
		public virtual IAssets Assets 
		{ 
			get { return proxy.GetElement(() => this.Assets, element => new Assets(element)); }
		}
		
		/// <summary>
		/// Gets the <see cref="IAutomationCollection"/> contained in this element.
		/// </summary>
		public virtual IAutomationCollection AutomationCollection 
		{ 
			get { return proxy.GetElement(() => this.AutomationCollection, element => new AutomationCollection(element)); }
		}
		
		/// <summary>
		/// Gets the <see cref="IPatternModel"/> contained in this element.
		/// </summary>
		public virtual IPatternModel PatternModel 
		{ 
			get { return proxy.GetElement(() => this.PatternModel, element => new PatternModel(element)); }
		}
		
		/// <summary>
		/// Gets the <see cref="IPatternToolkitInfo"/> contained in this element.
		/// </summary>
		public virtual IPatternToolkitInfo PatternToolkitInfo 
		{ 
			get { return proxy.GetElement(() => this.PatternToolkitInfo, element => new PatternToolkitInfo(element)); }
		}
		
		/// <summary>
		///	Creates a new <see cref="IAssets"/>  
		/// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
		///	</summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual IAssets CreateAssets(string name, Action<IAssets> initializer = null, bool raiseInstantiateEvents = true)
		{
			return proxy.CreateCollection<IAssets>(name, initializer, raiseInstantiateEvents);
		}
		
		/// <summary>
		///	Creates a new <see cref="IAutomationCollection"/>  
		/// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
		///	</summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual IAutomationCollection CreateAutomationCollection(string name, Action<IAutomationCollection> initializer = null, bool raiseInstantiateEvents = true)
		{
			return proxy.CreateCollection<IAutomationCollection>(name, initializer, raiseInstantiateEvents);
		}
		
		/// <summary>
		///	Creates a new <see cref="IPatternModel"/>  
		/// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
		///	</summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual IPatternModel CreatePatternModel(string name, Action<IPatternModel> initializer = null, bool raiseInstantiateEvents = true)
		{
			return proxy.CreateElement<IPatternModel>(name, initializer, raiseInstantiateEvents);	
		}
		
		/// <summary>
		///	Creates a new <see cref="IPatternToolkitInfo"/>  
		/// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
		///	</summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual IPatternToolkitInfo CreatePatternToolkitInfo(string name, Action<IPatternToolkitInfo> initializer = null, bool raiseInstantiateEvents = true)
		{
			return proxy.CreateElement<IPatternToolkitInfo>(name, initializer, raiseInstantiateEvents);	
		}

		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public virtual void Delete()
		{
			this.target.Delete();
		}
	}
}

namespace Microsoft.VisualStudio.Patterning.Authoring.Authoring
{
	using global::Microsoft.VisualStudio.Patterning.Runtime;
	using global::System;
	using global::System.Collections.Generic;
	using global::System.ComponentModel;
	using global::System.ComponentModel.Composition;
	using global::System.ComponentModel.Design;
	using global::System.Drawing.Design;
	using Runtime = global::Microsoft.VisualStudio.Patterning.Runtime;

	///	<summary>
	///	The design view of the toolkit.
	///	</summary>
	[Description("The design view of the toolkit.")]
	[ToolkitInterfaceProxy(ExtensionId ="9f6dc301-6f66-4d21-9f9c-b37412b162f6", DefinitionId = "a5541c90-1637-4405-9fe7-4b31f28eb3cd", ProxyType = typeof(Design))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class Design : IDesign
	{
		private Runtime.IView target;
		private Runtime.IContainerProxy<IDesign> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private Design() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Design"/> class.
		/// </summary>
		public Design(Runtime.IView target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IDesign>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual IPatternToolkit Parent
		{ 
			get { return this.target.Parent.As<IPatternToolkit>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IView"/> underlying element.
		/// </summary>
		public virtual Runtime.IView AsView()
		{
			return this.target;
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		/// <summary>
		/// Gets the <see cref="IProductionTooling"/> contained in this element.
		/// </summary>
		public virtual IProductionTooling ProductionTooling 
		{ 
			get { return proxy.GetElement(() => this.ProductionTooling, element => new ProductionTooling(element)); }
		}
		
		/// <summary>
		///	Creates a new <see cref="IProductionTooling"/>  
		/// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
		///	</summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual IProductionTooling CreateProductionTooling(string name, Action<IProductionTooling> initializer = null, bool raiseInstantiateEvents = true)
		{
			return proxy.CreateElement<IProductionTooling>(name, initializer, raiseInstantiateEvents);	
		}

		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public virtual void Delete()
		{
			this.target.Delete();
		}
	}
}

