﻿//------------------------------------------------------------------------------
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
	using global::NuPattern.Runtime.Bindings;
	using global::NuPattern.Runtime.Design;
	using global::NuPattern.Runtime.ToolkitInterface;
	using global::System;
	using global::System.Collections.Generic;
	using global::System.ComponentModel;
	using global::System.ComponentModel.Design;
	using global::System.Drawing.Design;
	using Runtime = global::NuPattern.Runtime;

	/// <summary>
	/// A library that contains reusable automation types for sharing between Pattern Toolkit projects.
	/// </summary>
	[Description("A library that contains reusable automation types for sharing between Pattern Toolkit projects.")]
	[ToolkitInterface(ExtensionId = "97bd7ab2-964b-43f1-8a08-be6db68b018b", DefinitionId = "d6139b37-9971-4834-a9dc-2b3daef962cf", ProxyType = typeof(PatternToolkitLibrary))]
	[System.CodeDom.Compiler.GeneratedCode("NuPattern Toolkit Library", "1.3.20.0")]
	public partial interface IPatternToolkitLibrary : IToolkitInterface
	{

		/// <summary>
		/// When to transform all code generation templates in this library, to ensure that all library artifacts are up to date.
		/// </summary>
		[Description("When to transform all code generation templates in this library, to ensure that all library artifacts are up to date.")]
		[DisplayName("Transform On Build")]
		[Category("Code Generation")]
		[TypeConverter(typeof(TransformOnBuildConverter))]
		String TransformOnBuild { get; set; }

		/// <summary>
		/// The project root namespace
		/// </summary>
		[Description("The project root namespace")]
		[DisplayName("Toolkit Project Root Namespace")]
		[Category("General")]
		String ToolkitProjectRootNamespace { get; set; }

		/// <summary>
		/// The assembly name of the automation library project.
		/// </summary>
		[Description("The assembly name of the automation library project.")]
		[DisplayName("Project Assembly Name")]
		[Category("General")]
		String ProjectAssemblyName { get; set; }

		/// <summary>
		/// The name of the automation library project.
		/// </summary>
		[Description("The name of the automation library project.")]
		[DisplayName("Project Name")]
		[Category("General")]
		String ProjectName { get; set; }

		/// <summary>
		/// Excludes all generated code from code coverage metrics.
		/// </summary>
		[Description("Excludes all generated code from code coverage metrics.")]
		[DisplayName("Exclude From Code Coverage")]
		[Category("Code Generation")]
		Boolean ExcludeFromCodeCoverage { get; set; }

		/// <summary>
		/// Gets or sets the ToolkitInfo property.
		/// </summary>
		IProductToolkitInfo ToolkitInfo { get; }

		/// <summary>
		/// Gets or sets the CurrentView property.
		/// </summary>
		IView CurrentView { get; set; }

		/// <summary>
		/// The name of this element instance.
		/// </summary>
		[Description("The name of this element instance.")]
		[ParenthesizePropertyName(true)]
		String InstanceName { get; set; }

		/// <summary>
		/// The order of this element relative to its siblings.
		/// </summary>
		[Description("The order of this element relative to its siblings.")]
		[ReadOnly(true)]
		Double InstanceOrder { get; set; }

		/// <summary>
		/// The references of this element.
		/// </summary>
		[Description("The references of this element.")]
		IEnumerable<IReference> References { get; }

		/// <summary>
		/// Notes for this element.
		/// </summary>
		[Description("Notes for this element.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		String Notes { get; set; }

		/// <summary>
		/// Gets or sets the InTransaction property.
		/// </summary>
		Boolean InTransaction { get; }

		/// <summary>
		/// Gets or sets the IsSerializing property.
		/// </summary>
		Boolean IsSerializing { get; }

		/// <summary>
		/// Description for PatternToolkitLibrary.Development
		/// </summary>
		[Description("Description for PatternToolkitLibrary.Development")]
		IDevelopment Development { get; }

		/// <summary>
		/// Deletes this element.
		/// </summary>
		void Delete();

		/// <summary>
		/// Gets the generalized interface (<see cref="Runtime.IProduct"/>) of this element.
		/// </summary>
		Runtime.IProduct AsProduct();
	}
}