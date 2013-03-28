﻿
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuPattern.Authoring.PatternToolkitLibrary
{
    using global::NuPattern.Runtime;
    using global::NuPattern.Runtime.ToolkitInterface;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.ComponentModel.Design;
    using global::System.Drawing.Design;
    using Runtime = global::NuPattern.Runtime;

    ///	<summary>
    ///	The commands defined in this library.
    ///	</summary>
    [Description("The commands defined in this library.")]
    [ToolkitInterface(ExtensionId = "97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "184bf063-7339-4866-978c-964d9d995c32", ProxyType = typeof(Commands))]
    [System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Library", "1.3.20.0")]
    public partial interface ICommands : IToolkitInterface
    {
        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Description("Notes for this element.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        String Notes { get; set; }

        ///	<summary>
        ///	The InTransaction.
        ///	</summary>
        Boolean InTransaction { get; }

        ///	<summary>
        ///	The IsSerializing.
        ///	</summary>
        Boolean IsSerializing { get; }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [Description("The name of this element instance.")]
        [ParenthesizePropertyName(true)]
        String InstanceName { get; set; }

        ///	<summary>
        ///	The order of this element relative to its siblings.
        ///	</summary>
        [Description("The order of this element relative to its siblings.")]
        [ReadOnly(true)]
        Double InstanceOrder { get; set; }

        ///	<summary>
        ///	The references of this element.
        ///	</summary>
        [Description("The references of this element.")]
        IEnumerable<IReference> References { get; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        IDevelopment Parent { get; }

        /// <summary>
        /// Gets all instances of <see cref="ICommand"/> contained in this element.
        /// </summary>
        IEnumerable<ICommand> Command { get; }

        /// <summary>
        ///	Creates a new <see cref="ICommand"/>  and adds it to the <see cref="Command"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        ICommand CreateCommand(string name, Action<ICommand> initializer = null, bool raiseInstantiateEvents = true);

        ///	<summary>
        ///	Deletes this element from the store.
        ///	</summary>
        void Delete();

        /// <summary>
        /// Gets the generic <see cref="Runtime.ICollection"/> underlying element.
        /// </summary>
        Runtime.ICollection AsCollection();
    }
}

namespace NuPattern.Authoring.PatternToolkitLibrary
{
    using global::NuPattern.Runtime;
    using global::NuPattern.Runtime.ToolkitInterface;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.ComponentModel.Design;
    using global::System.Drawing.Design;
    using Runtime = global::NuPattern.Runtime;

    ///	<summary>
    ///	The conditions defined in this library.
    ///	</summary>
    [Description("The conditions defined in this library.")]
    [ToolkitInterface(ExtensionId = "97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "2226c6bc-3e33-4570-8807-f68bbfbbffcd", ProxyType = typeof(Conditions))]
    [System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Library", "1.3.20.0")]
    public partial interface IConditions : IToolkitInterface
    {
        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Description("Notes for this element.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        String Notes { get; set; }

        ///	<summary>
        ///	The InTransaction.
        ///	</summary>
        Boolean InTransaction { get; }

        ///	<summary>
        ///	The IsSerializing.
        ///	</summary>
        Boolean IsSerializing { get; }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [Description("The name of this element instance.")]
        [ParenthesizePropertyName(true)]
        String InstanceName { get; set; }

        ///	<summary>
        ///	The order of this element relative to its siblings.
        ///	</summary>
        [Description("The order of this element relative to its siblings.")]
        [ReadOnly(true)]
        Double InstanceOrder { get; set; }

        ///	<summary>
        ///	The references of this element.
        ///	</summary>
        [Description("The references of this element.")]
        IEnumerable<IReference> References { get; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        IDevelopment Parent { get; }

        /// <summary>
        /// Gets all instances of <see cref="ICondition"/> contained in this element.
        /// </summary>
        IEnumerable<ICondition> Condition { get; }

        /// <summary>
        ///	Creates a new <see cref="ICondition"/>  and adds it to the <see cref="Condition"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        ICondition CreateCondition(string name, Action<ICondition> initializer = null, bool raiseInstantiateEvents = true);

        ///	<summary>
        ///	Deletes this element from the store.
        ///	</summary>
        void Delete();

        /// <summary>
        /// Gets the generic <see cref="Runtime.ICollection"/> underlying element.
        /// </summary>
        Runtime.ICollection AsCollection();
    }
}

namespace NuPattern.Authoring.PatternToolkitLibrary
{
    using global::NuPattern.Runtime;
    using global::NuPattern.Runtime.ToolkitInterface;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.ComponentModel.Design;
    using global::System.Drawing.Design;
    using Runtime = global::NuPattern.Runtime;

    ///	<summary>
    ///	The events defined in this library.
    ///	</summary>
    [Description("The events defined in this library.")]
    [ToolkitInterface(ExtensionId = "97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "f0e386f0-bf6f-4560-978f-32396e30c4e5", ProxyType = typeof(Events))]
    [System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Library", "1.3.20.0")]
    public partial interface IEvents : IToolkitInterface
    {
        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Description("Notes for this element.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        String Notes { get; set; }

        ///	<summary>
        ///	The InTransaction.
        ///	</summary>
        Boolean InTransaction { get; }

        ///	<summary>
        ///	The IsSerializing.
        ///	</summary>
        Boolean IsSerializing { get; }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [Description("The name of this element instance.")]
        [ParenthesizePropertyName(true)]
        String InstanceName { get; set; }

        ///	<summary>
        ///	The order of this element relative to its siblings.
        ///	</summary>
        [Description("The order of this element relative to its siblings.")]
        [ReadOnly(true)]
        Double InstanceOrder { get; set; }

        ///	<summary>
        ///	The references of this element.
        ///	</summary>
        [Description("The references of this element.")]
        IEnumerable<IReference> References { get; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        IDevelopment Parent { get; }

        /// <summary>
        /// Gets all instances of <see cref="IEvent"/> contained in this element.
        /// </summary>
        IEnumerable<IEvent> Event { get; }

        /// <summary>
        ///	Creates a new <see cref="IEvent"/>  and adds it to the <see cref="Event"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        IEvent CreateEvent(string name, Action<IEvent> initializer = null, bool raiseInstantiateEvents = true);

        ///	<summary>
        ///	Deletes this element from the store.
        ///	</summary>
        void Delete();

        /// <summary>
        /// Gets the generic <see cref="Runtime.ICollection"/> underlying element.
        /// </summary>
        Runtime.ICollection AsCollection();
    }
}

namespace NuPattern.Authoring.PatternToolkitLibrary
{
    using global::NuPattern.Runtime;
    using global::NuPattern.Runtime.ToolkitInterface;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.ComponentModel.Design;
    using global::System.Drawing.Design;
    using Runtime = global::NuPattern.Runtime;

    ///	<summary>
    ///	The type converters defined in this library.
    ///	</summary>
    [Description("The type converters defined in this library.")]
    [ToolkitInterface(ExtensionId = "97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "96227e6c-f4a5-421c-8f0a-e38763d86740", ProxyType = typeof(TypeConverters))]
    [System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Library", "1.3.20.0")]
    public partial interface ITypeConverters : IToolkitInterface
    {
        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Description("Notes for this element.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        String Notes { get; set; }

        ///	<summary>
        ///	The InTransaction.
        ///	</summary>
        Boolean InTransaction { get; }

        ///	<summary>
        ///	The IsSerializing.
        ///	</summary>
        Boolean IsSerializing { get; }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [Description("The name of this element instance.")]
        [ParenthesizePropertyName(true)]
        String InstanceName { get; set; }

        ///	<summary>
        ///	The order of this element relative to its siblings.
        ///	</summary>
        [Description("The order of this element relative to its siblings.")]
        [ReadOnly(true)]
        Double InstanceOrder { get; set; }

        ///	<summary>
        ///	The references of this element.
        ///	</summary>
        [Description("The references of this element.")]
        IEnumerable<IReference> References { get; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        IDevelopment Parent { get; }

        /// <summary>
        /// Gets all instances of <see cref="IEnumTypeConverter"/> contained in this element.
        /// </summary>
        IEnumerable<IEnumTypeConverter> EnumTypeConverters { get; }

        /// <summary>
        /// Gets all instances of <see cref="IDataTypeConverter"/> contained in this element.
        /// </summary>
        IEnumerable<IDataTypeConverter> DataTypeConverters { get; }

        /// <summary>
        ///	Creates a new <see cref="IEnumTypeConverter"/>  and adds it to the <see cref="EnumTypeConverters"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        IEnumTypeConverter CreateEnumTypeConverter(string name, Action<IEnumTypeConverter> initializer = null, bool raiseInstantiateEvents = true);

        /// <summary>
        ///	Creates a new <see cref="IDataTypeConverter"/>  and adds it to the <see cref="DataTypeConverters"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        IDataTypeConverter CreateDataTypeConverter(string name, Action<IDataTypeConverter> initializer = null, bool raiseInstantiateEvents = true);

        ///	<summary>
        ///	Deletes this element from the store.
        ///	</summary>
        void Delete();

        /// <summary>
        /// Gets the generic <see cref="Runtime.ICollection"/> underlying element.
        /// </summary>
        Runtime.ICollection AsCollection();
    }
}

namespace NuPattern.Authoring.PatternToolkitLibrary
{
    using global::NuPattern.Runtime;
    using global::NuPattern.Runtime.ToolkitInterface;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.ComponentModel.Design;
    using global::System.Drawing.Design;
    using Runtime = global::NuPattern.Runtime;

    ///	<summary>
    ///	The type editors defined in this library.
    ///	</summary>
    [Description("The type editors defined in this library.")]
    [ToolkitInterface(ExtensionId = "97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "7fe79778-3880-4daf-a4e5-c401f131c5db", ProxyType = typeof(TypeEditors))]
    [System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Library", "1.3.20.0")]
    public partial interface ITypeEditors : IToolkitInterface
    {
        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Description("Notes for this element.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        String Notes { get; set; }

        ///	<summary>
        ///	The InTransaction.
        ///	</summary>
        Boolean InTransaction { get; }

        ///	<summary>
        ///	The IsSerializing.
        ///	</summary>
        Boolean IsSerializing { get; }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [Description("The name of this element instance.")]
        [ParenthesizePropertyName(true)]
        String InstanceName { get; set; }

        ///	<summary>
        ///	The order of this element relative to its siblings.
        ///	</summary>
        [Description("The order of this element relative to its siblings.")]
        [ReadOnly(true)]
        Double InstanceOrder { get; set; }

        ///	<summary>
        ///	The references of this element.
        ///	</summary>
        [Description("The references of this element.")]
        IEnumerable<IReference> References { get; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        IDevelopment Parent { get; }

        /// <summary>
        /// Gets all instances of <see cref="ITypePickerEditor"/> contained in this element.
        /// </summary>
        IEnumerable<ITypePickerEditor> TypePickerEditors { get; }

        /// <summary>
        /// Gets all instances of <see cref="IUIEditor"/> contained in this element.
        /// </summary>
        IEnumerable<IUIEditor> UIEditors { get; }

        /// <summary>
        ///	Creates a new <see cref="ITypePickerEditor"/>  and adds it to the <see cref="TypePickerEditors"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        ITypePickerEditor CreateTypePickerEditor(string name, Action<ITypePickerEditor> initializer = null, bool raiseInstantiateEvents = true);

        /// <summary>
        ///	Creates a new <see cref="IUIEditor"/>  and adds it to the <see cref="UIEditors"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        IUIEditor CreateUIEditor(string name, Action<IUIEditor> initializer = null, bool raiseInstantiateEvents = true);

        ///	<summary>
        ///	Deletes this element from the store.
        ///	</summary>
        void Delete();

        /// <summary>
        /// Gets the generic <see cref="Runtime.ICollection"/> underlying element.
        /// </summary>
        Runtime.ICollection AsCollection();
    }
}

namespace NuPattern.Authoring.PatternToolkitLibrary
{
    using global::NuPattern.Runtime;
    using global::NuPattern.Runtime.ToolkitInterface;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.ComponentModel.Design;
    using global::System.Drawing.Design;
    using Runtime = global::NuPattern.Runtime;

    ///	<summary>
    ///	The validation rules defined in this library.
    ///	</summary>
    [Description("The validation rules defined in this library.")]
    [ToolkitInterface(ExtensionId = "97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "c398e040-eb92-481a-a1a4-67d1eac1edff", ProxyType = typeof(ValidationRules))]
    [System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Library", "1.3.20.0")]
    public partial interface IValidationRules : IToolkitInterface
    {
        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Description("Notes for this element.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        String Notes { get; set; }

        ///	<summary>
        ///	The InTransaction.
        ///	</summary>
        Boolean InTransaction { get; }

        ///	<summary>
        ///	The IsSerializing.
        ///	</summary>
        Boolean IsSerializing { get; }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [Description("The name of this element instance.")]
        [ParenthesizePropertyName(true)]
        String InstanceName { get; set; }

        ///	<summary>
        ///	The order of this element relative to its siblings.
        ///	</summary>
        [Description("The order of this element relative to its siblings.")]
        [ReadOnly(true)]
        Double InstanceOrder { get; set; }

        ///	<summary>
        ///	The references of this element.
        ///	</summary>
        [Description("The references of this element.")]
        IEnumerable<IReference> References { get; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        IDevelopment Parent { get; }

        /// <summary>
        /// Gets all instances of <see cref="IElementValidationRule"/> contained in this element.
        /// </summary>
        IEnumerable<IElementValidationRule> ElementValidationRules { get; }

        /// <summary>
        /// Gets all instances of <see cref="IPropertyValidationRule"/> contained in this element.
        /// </summary>
        IEnumerable<IPropertyValidationRule> PropertyValidationRules { get; }

        /// <summary>
        ///	Creates a new <see cref="IElementValidationRule"/>  and adds it to the <see cref="ElementValidationRules"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        IElementValidationRule CreateElementValidationRule(string name, Action<IElementValidationRule> initializer = null, bool raiseInstantiateEvents = true);

        /// <summary>
        ///	Creates a new <see cref="IPropertyValidationRule"/>  and adds it to the <see cref="PropertyValidationRules"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        IPropertyValidationRule CreatePropertyValidationRule(string name, Action<IPropertyValidationRule> initializer = null, bool raiseInstantiateEvents = true);

        ///	<summary>
        ///	Deletes this element from the store.
        ///	</summary>
        void Delete();

        /// <summary>
        /// Gets the generic <see cref="Runtime.ICollection"/> underlying element.
        /// </summary>
        Runtime.ICollection AsCollection();
    }
}

namespace NuPattern.Authoring.PatternToolkitLibrary
{
    using global::NuPattern.Runtime;
    using global::NuPattern.Runtime.ToolkitInterface;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.ComponentModel.Design;
    using global::System.Drawing.Design;
    using Runtime = global::NuPattern.Runtime;

    ///	<summary>
    ///	The value providers defined in this library.
    ///	</summary>
    [Description("The value providers defined in this library.")]
    [ToolkitInterface(ExtensionId = "97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "fa75b840-b733-4365-bcb0-5e513092f4dd", ProxyType = typeof(ValueProviders))]
    [System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Library", "1.3.20.0")]
    public partial interface IValueProviders : IToolkitInterface
    {
        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Description("Notes for this element.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        String Notes { get; set; }

        ///	<summary>
        ///	The InTransaction.
        ///	</summary>
        Boolean InTransaction { get; }

        ///	<summary>
        ///	The IsSerializing.
        ///	</summary>
        Boolean IsSerializing { get; }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [Description("The name of this element instance.")]
        [ParenthesizePropertyName(true)]
        String InstanceName { get; set; }

        ///	<summary>
        ///	The order of this element relative to its siblings.
        ///	</summary>
        [Description("The order of this element relative to its siblings.")]
        [ReadOnly(true)]
        Double InstanceOrder { get; set; }

        ///	<summary>
        ///	The references of this element.
        ///	</summary>
        [Description("The references of this element.")]
        IEnumerable<IReference> References { get; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        IDevelopment Parent { get; }

        /// <summary>
        /// Gets all instances of <see cref="IValueProvider"/> contained in this element.
        /// </summary>
        IEnumerable<IValueProvider> ValueProvider { get; }

        /// <summary>
        ///	Creates a new <see cref="IValueProvider"/>  and adds it to the <see cref="ValueProvider"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        IValueProvider CreateValueProvider(string name, Action<IValueProvider> initializer = null, bool raiseInstantiateEvents = true);

        ///	<summary>
        ///	Deletes this element from the store.
        ///	</summary>
        void Delete();

        /// <summary>
        /// Gets the generic <see cref="Runtime.ICollection"/> underlying element.
        /// </summary>
        Runtime.ICollection AsCollection();
    }
}

namespace NuPattern.Authoring.PatternToolkitLibrary
{
    using global::NuPattern.Runtime;
    using global::NuPattern.Runtime.ToolkitInterface;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.ComponentModel.Design;
    using global::System.Drawing.Design;
    using Runtime = global::NuPattern.Runtime;

    ///	<summary>
    ///	The value comparers defined in this library.
    ///	</summary>
    [Description("The value comparers defined in this library.")]
    [ToolkitInterface(ExtensionId = "97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "18765960-f08b-4bd2-a0e5-fb547a182517", ProxyType = typeof(ValueComparers))]
    [System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Library", "1.3.20.0")]
    public partial interface IValueComparers : IToolkitInterface
    {
        ///	<summary>
        ///	Notes for this element.
        ///	</summary>
        [Description("Notes for this element.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        String Notes { get; set; }

        ///	<summary>
        ///	The InTransaction.
        ///	</summary>
        Boolean InTransaction { get; }

        ///	<summary>
        ///	The IsSerializing.
        ///	</summary>
        Boolean IsSerializing { get; }

        ///	<summary>
        ///	The name of this element instance.
        ///	</summary>
        [Description("The name of this element instance.")]
        [ParenthesizePropertyName(true)]
        String InstanceName { get; set; }

        ///	<summary>
        ///	The order of this element relative to its siblings.
        ///	</summary>
        [Description("The order of this element relative to its siblings.")]
        [ReadOnly(true)]
        Double InstanceOrder { get; set; }

        ///	<summary>
        ///	The references of this element.
        ///	</summary>
        [Description("The references of this element.")]
        IEnumerable<IReference> References { get; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        IDevelopment Parent { get; }

        /// <summary>
        /// Gets all instances of <see cref="IElementOrderingComparer"/> contained in this element.
        /// </summary>
        IEnumerable<IElementOrderingComparer> ElementOrderingComparers { get; }

        /// <summary>
        ///	Creates a new <see cref="IElementOrderingComparer"/>  and adds it to the <see cref="ElementOrderingComparers"/> collection,  
        /// executing the optional <paramref name="initializer"/> if not <see langword="null"/>.
        ///	</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        IElementOrderingComparer CreateElementOrderingComparer(string name, Action<IElementOrderingComparer> initializer = null, bool raiseInstantiateEvents = true);

        ///	<summary>
        ///	Deletes this element from the store.
        ///	</summary>
        void Delete();

        /// <summary>
        /// Gets the generic <see cref="Runtime.ICollection"/> underlying element.
        /// </summary>
        Runtime.ICollection AsCollection();
    }
}

