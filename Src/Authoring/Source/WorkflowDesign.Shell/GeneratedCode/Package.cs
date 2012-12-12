﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using VSShellInterop = global::Microsoft.VisualStudio.Shell.Interop;
using VSShell = global::Microsoft.VisualStudio.Shell;
using DslShell = global::Microsoft.VisualStudio.Modeling.Shell;
using DslDesign = global::Microsoft.VisualStudio.Modeling.Design;
using DslModeling = global::Microsoft.VisualStudio.Modeling;
using VSTextTemplatingHost = global::Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
	
namespace Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign
{
	/// <summary>
	/// This class implements the VS package that integrates this DSL into Visual Studio.
	/// </summary>
	[VSShell::DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\10.0")]
	[VSShell::PackageRegistration(RegisterUsing = VSShell::RegistrationMethod.Assembly, UseManagedResourcesOnly = true)]
	[VSShell::ProvideStaticToolboxGroup("@Production ToolingToolboxTab;Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll", "Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.Production ToolingToolboxTab")]
	[VSShell::ProvideStaticToolboxItem("Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.Production ToolingToolboxTab",
					"@SuppliedAssetToolboxItem;Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll", 
					"Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.SuppliedAssetToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"SuppliedAsset", 
					"@SuppliedAssetToolboxBitmap;Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.Production ToolingToolboxTab",
					"@ProductionToolToolboxItem;Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll", 
					"Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.ProductionToolToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"ProductionTool", 
					"@ProductionToolToolboxBitmap;Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.Production ToolingToolboxTab",
					"@ProducedAssetToolboxItem;Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll", 
					"Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.ProducedAssetToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"ProducedAsset", 
					"@ProducedAssetToolboxBitmap;Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.Production ToolingToolboxTab",
					"@ProductionWorkflowConnectorToolboxItem;Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll", 
					"Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.ProductionWorkflowConnectorToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"ProductionWorkflowConnector", 
					"@ProductionWorkflowConnectorToolboxBitmap;Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll", 
					0xff00ff)]
	[VSShell::ProvideEditorFactory(typeof(WorkflowDesignEditorFactory), 103, TrustLevel = VSShellInterop::__VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
	[VSShell::ProvideEditorExtension(typeof(WorkflowDesignEditorFactory), "." + Constants.DesignerFileExtension, 50)]
	[DslShell::ProvideRelatedFile("." + Constants.DesignerFileExtension, Constants.DefaultDiagramExtension,
		ProjectSystem = DslShell::ProvideRelatedFileAttribute.CSharpProjectGuid,
		FileOptions = DslShell::RelatedFileType.FileName)]
	[DslShell::ProvideRelatedFile("." + Constants.DesignerFileExtension, Constants.DefaultDiagramExtension,
		ProjectSystem = DslShell::ProvideRelatedFileAttribute.VisualBasicProjectGuid,
		FileOptions = DslShell::RelatedFileType.FileName)]
	[DslShell::RegisterAsDslToolsEditor]
	[global::System.Runtime.InteropServices.ComVisible(true)]
	[DslShell::ProvideBindingPath]
	[DslShell::ProvideXmlEditorChooserBlockSxSWithXmlEditor(@"WorkflowDesign", typeof(WorkflowDesignEditorFactory))]
	internal abstract partial class WorkflowDesignPackageBase : DslShell::ModelingPackage
	{
		protected global::Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.WorkflowDesignToolboxHelper toolboxHelper;	
		
		/// <summary>
		/// Initialization method called by the package base class when this package is loaded.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			// Register the editor factory used to create the DSL editor.
			this.RegisterEditorFactory(new WorkflowDesignEditorFactory(this));
			
			// Initialize the toolbox helper
			toolboxHelper = new global::Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.WorkflowDesignToolboxHelper(this);

			// Create the command set that handles menu commands provided by this package.
			WorkflowDesignCommandSet commandSet = new WorkflowDesignCommandSet(this);
			commandSet.Initialize();
			
			// Create the command set that handles cut/copy/paste commands provided by this package.
			WorkflowDesignClipboardCommandSet clipboardCommandSet = new WorkflowDesignClipboardCommandSet(this);
			clipboardCommandSet.Initialize();
			
			// Initialize Extension Registars
			// this is a partial method call
			this.InitializeExtensions();

			// Add dynamic toolbox items
			this.SetupDynamicToolbox();
		}

		/// <summary>
		/// Partial method to initialize ExtensionRegistrars (if any) in the DslPackage
		/// </summary>
		partial void InitializeExtensions();
		
		/// <summary>
		/// Returns any dynamic tool items for the designer
		/// </summary>
		/// <remarks>The default implementation is to return the list of items from the generated toolbox helper.</remarks>
		protected override global::System.Collections.Generic.IList<DslDesign::ModelingToolboxItem> CreateToolboxItems()
		{
			try
			{
				Debug.Assert(toolboxHelper != null, "Toolbox helper is not initialized");
				return toolboxHelper.CreateToolboxItems();
			}
			catch(global::System.Exception e)
			{
				global::System.Diagnostics.Debug.Fail("Exception thrown during toolbox item creation.  This may result in Package Load Failure:\r\n\r\n" + e);
				throw;
			}
		}
		
		
		/// <summary>
		/// Given a toolbox item "unique ID" and a data format identifier, returns the content of
		/// the data format. 
		/// </summary>
		/// <param name="itemId">The unique ToolboxItem to retrieve data for</param>
		/// <param name="format">The desired format of the resulting data</param>
		protected override object GetToolboxItemData(string itemId, DataFormats.Format format)
		{
			Debug.Assert(toolboxHelper != null, "Toolbox helper is not initialized");
		
			// Retrieve the specified ToolboxItem from the DSL
			return toolboxHelper.GetToolboxItemData(itemId, format);
		}
	}

}

//
// Package attributes which may need to change are placed on the partial class below, rather than in the main include file.
//
namespace Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	[VSShell::ProvideMenuResource("1000.ctmenu", 1)]
	[VSShell::ProvideToolboxItems(1)]
	[global::Microsoft.VisualStudio.Modeling.Shell.ProvideXmlEditorChooserDesignerView(
		"WorkflowDesign",
		Constants.DesignerFileExtension,
		EnvDTE.Constants.vsViewKindDesigner,
		1,
		CodeLogicalViewEditor = Constants.WorkflowDesignEditorFactoryId,
		DebuggingLogicalViewEditor = Constants.WorkflowDesignEditorFactoryId,
		DesignerLogicalViewEditor = Constants.WorkflowDesignEditorFactoryId,
		TextLogicalViewEditor = Constants.WorkflowDesignEditorFactoryId)]
	[global::Microsoft.VisualStudio.TextTemplating.VSHost.ProvideDirectiveProcessor(typeof(global::Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.WorkflowDesignDirectiveProcessor), global::Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.WorkflowDesignDirectiveProcessor.WorkflowDesignDirectiveProcessorName, "A directive processor that provides access to WorkflowDesign files")]
	[global::System.Runtime.InteropServices.Guid(Constants.WorkflowDesignPackageId)]
	internal sealed partial class WorkflowDesignPackage : WorkflowDesignPackageBase
	{
	}
}