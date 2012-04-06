﻿
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	A custom automation command that executes some automation.
	///	</summary>
	[Description("A custom automation command that executes some automation.")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "5a6b3a47-aabf-4f51-bd30-0737f6d4bbbe", ProxyType = typeof(Command))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class Command : ICommand
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<ICommand> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private Command() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Command"/> class.
		/// </summary>
		public Command(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<ICommand>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual ICommands Parent
		{ 
			get { return this.target.Parent.As<ICommands>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	A custom condition that is evaluated to determine if automation executes.
	///	</summary>
	[Description("A custom condition that is evaluated to determine if automation executes.")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "2d022abe-eaf9-4366-843a-a9d262ef4b00", ProxyType = typeof(Condition))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class Condition : ICondition
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<ICondition> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private Condition() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Condition"/> class.
		/// </summary>
		public Condition(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<ICondition>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual IConditions Parent
		{ 
			get { return this.target.Parent.As<IConditions>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	A custom event that can trigger some automation.
	///	</summary>
	[Description("A custom event that can trigger some automation.")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "d937c559-01f8-43ad-bac8-f580013fdd8a", ProxyType = typeof(Event))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class Event : IEvent
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<IEvent> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private Event() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Event"/> class.
		/// </summary>
		public Event(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IEvent>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual IEvents Parent
		{ 
			get { return this.target.Parent.As<IEvents>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	A custom type converter that can be used to define a dropdown of values for a variable property.
	///	</summary>
	[Description("A custom type converter that can be used to define a dropdown of values for a variable property.")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "8d45d1e8-2551-4539-8449-ff3e0962d5f5", ProxyType = typeof(EnumTypeConverter))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class EnumTypeConverter : IEnumTypeConverter
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<IEnumTypeConverter> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private EnumTypeConverter() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumTypeConverter"/> class.
		/// </summary>
		public EnumTypeConverter(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IEnumTypeConverter>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual ITypeConverters Parent
		{ 
			get { return this.target.Parent.As<ITypeConverters>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	Description for AutomationLibrary.Development.TypeConverters.DataTypeConverter
	///	</summary>
	[Description("Description for AutomationLibrary.Development.TypeConverters.DataTypeConverter")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "4caa371c-0404-4dcd-ba61-457962baaa53", ProxyType = typeof(DataTypeConverter))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class DataTypeConverter : IDataTypeConverter
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<IDataTypeConverter> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private DataTypeConverter() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DataTypeConverter"/> class.
		/// </summary>
		public DataTypeConverter(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IDataTypeConverter>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual ITypeConverters Parent
		{ 
			get { return this.target.Parent.As<ITypeConverters>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	A custom type picker editor that displays a filtered list of types found in the current solution.
	///	</summary>
	[Description("A custom type picker editor that displays a filtered list of types found in the current solution.")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "4a6d1cc8-3845-4b08-b2b1-e27a1a6e9c9f", ProxyType = typeof(TypePickerEditor))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class TypePickerEditor : ITypePickerEditor
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<ITypePickerEditor> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private TypePickerEditor() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="TypePickerEditor"/> class.
		/// </summary>
		public TypePickerEditor(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<ITypePickerEditor>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual ITypeEditors Parent
		{ 
			get { return this.target.Parent.As<ITypeEditors>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	A custom editor that provides a custom GUI to configure the value of a variable property.
	///	</summary>
	[Description("A custom editor that provides a custom GUI to configure the value of a variable property.")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "27b16755-4f3e-43a6-80ee-38cad4dec8ab", ProxyType = typeof(UIEditor))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class UIEditor : IUIEditor
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<IUIEditor> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private UIEditor() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UIEditor"/> class.
		/// </summary>
		public UIEditor(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IUIEditor>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual ITypeEditors Parent
		{ 
			get { return this.target.Parent.As<ITypeEditors>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	A custom element validation rule for verifying an element.
	///	</summary>
	[Description("A custom element validation rule for verifying an element.")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "ab7af436-f1f6-4d3e-b78d-f90a5f81d636", ProxyType = typeof(ElementValidationRule))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class ElementValidationRule : IElementValidationRule
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<IElementValidationRule> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private ElementValidationRule() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ElementValidationRule"/> class.
		/// </summary>
		public ElementValidationRule(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IElementValidationRule>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual IValidationRules Parent
		{ 
			get { return this.target.Parent.As<IValidationRules>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	A custom property validation rule for verifying a property.
	///	</summary>
	[Description("A custom property validation rule for verifying a property.")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "ffa532c8-f334-4bda-b9de-cb208f6d2a81", ProxyType = typeof(PropertyValidationRule))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class PropertyValidationRule : IPropertyValidationRule
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<IPropertyValidationRule> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private PropertyValidationRule() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyValidationRule"/> class.
		/// </summary>
		public PropertyValidationRule(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IPropertyValidationRule>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual IValidationRules Parent
		{ 
			get { return this.target.Parent.As<IValidationRules>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

namespace Microsoft.VisualStudio.Patterning.Authoring.Library
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
	///	A custom value provider that returns the calculated value for a variable property.
	///	</summary>
	[Description("A custom value provider that returns the calculated value for a variable property.")]
	[ToolkitInterfaceProxy(ExtensionId ="97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "835cd8dc-4fb1-4d60-94a9-c73342788a16", ProxyType = typeof(ValueProvider))]
	[System.CodeDom.Compiler.GeneratedCode("Pattern Toolkit Library Support", "1.2.19.0")]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal partial class ValueProvider : IValueProvider
	{
		private Runtime.IAbstractElement target;
		private Runtime.IAbstractElementProxy<IValueProvider> proxy;

		/// <summary>
		/// For MEF.
		/// </summary>
		[ImportingConstructor]
		private ValueProvider() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueProvider"/> class.
		/// </summary>
		public ValueProvider(Runtime.IAbstractElement target)
		{
			this.target = target;
			this.proxy = target.ProxyFor<IValueProvider>();
			OnCreated();
		}	

		partial void OnCreated();

		/// <summary>
		/// Gets the parent element.
		/// </summary>
		public virtual IValueProviders Parent
		{ 
			get { return this.target.Parent.As<IValueProviders>(); }
		}

		/// <summary>
		/// Gets the generic <see cref="Runtime.IElement"/> underlying element.
		/// </summary>
		public virtual Runtime.IElement AsElement()
		{
			return this.As<Runtime.IElement>();
		}

		/// <summary>
		/// Gets the generic underlying element as the given type if possible.
		/// </summary>
		public virtual TRuntimeInterface As<TRuntimeInterface>()
			where TRuntimeInterface : class
		{
			return this.target as TRuntimeInterface;
		}
		
		///	<summary>
		///	Notes for this element.
		///	</summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual String Notes 
		{ 
			get { return this.proxy.GetValue(() => this.Notes); }
			set { this.proxy.SetValue(() => this.Notes, value); }
		}
		
		///	<summary>
		///	The InTransaction.
		///	</summary>
		public virtual Boolean InTransaction 
		{ 
			get { return this.proxy.GetValue(() => this.InTransaction); }
		}
		
		///	<summary>
		///	The IsSerializing.
		///	</summary>
		public virtual Boolean IsSerializing 
		{ 
			get { return this.proxy.GetValue(() => this.IsSerializing); }
		}
		
		///	<summary>
		///	The name of this element instance.
		///	</summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		public virtual String InstanceName 
		{ 
			get { return this.proxy.GetValue(() => this.InstanceName); }
			set { this.proxy.SetValue(() => this.InstanceName, value); }
		}
		
		///	<summary>
		///	The references for this element.
		///	</summary>
		[Description("The references for this element.")]
		public virtual IEnumerable<IReference> References 
		{ 
			get { return this.proxy.GetValue(() => this.References); }
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

