﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DslShell = global::Microsoft.VisualStudio.Modeling.Shell;
using VSShell = global::Microsoft.VisualStudio.Shell;
using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslDiagrams = global::Microsoft.VisualStudio.Modeling.Diagrams;
using DslValidation = global::Microsoft.VisualStudio.Modeling.Validation;

namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal partial class WorkflowDesignCommandSet : WorkflowDesignCommandSetBase
	{
		/// <summary>
		/// Constructs a new WorkflowDesignCommandSet.
		/// </summary>
		public WorkflowDesignCommandSet(global::System.IServiceProvider serviceProvider) 
			: base(serviceProvider)
		{
		}
	}

	/// <summary>
	/// Class containing handlers for commands supported by this DSL.
	/// </summary>
	internal abstract class WorkflowDesignCommandSetBase : DslShell::CommandSet
	{
		/// <summary>
		/// Constructs a new WorkflowDesignCommandSetBase.
		/// </summary>
		protected WorkflowDesignCommandSetBase(global::System.IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		/// <summary>
		/// Provide the menu commands that this command set handles
		/// </summary>
		protected override global::System.Collections.Generic.IList<global::System.ComponentModel.Design.MenuCommand> GetMenuCommands()
		{
			// Get the standard commands
			global::System.Collections.Generic.IList<global::System.ComponentModel.Design.MenuCommand> commands = base.GetMenuCommands();

			global::System.ComponentModel.Design.MenuCommand menuCommand;

			// Add handler for "Validate All" menu command (validates the entire model).
			menuCommand = new DslShell::DynamicStatusMenuCommand(new global::System.EventHandler(OnStatusValidateModel), 
				new global::System.EventHandler(OnMenuValidateModel),
				DslShell::CommonModelingCommands.ValidateModel);
			commands.Add(menuCommand);
			// Add handler for "Validate" menu command (validates the current selection).
			menuCommand = new DslShell::DynamicStatusMenuCommand(new global::System.EventHandler(OnStatusValidate), 
				new global::System.EventHandler(OnMenuValidate),
				DslShell::CommonModelingCommands.Validate);
			commands.Add(menuCommand);

			return commands;
		}
		/// <summary>
		/// Status event handler for validating the model.
		/// </summary>
		internal virtual void OnStatusValidateModel(object sender, global::System.EventArgs e)
		{
			System.ComponentModel.Design.MenuCommand cmd = sender as System.ComponentModel.Design.MenuCommand;
			cmd.Enabled = cmd.Visible = true;
		}

		/// <summary>
		/// Handler for validating the model.
		/// </summary>
		internal virtual void OnMenuValidateModel(object sender, System.EventArgs e)
		{
			if (this.CurrentWorkflowDesignDocData != null && this.CurrentWorkflowDesignDocData.Store != null)
			{
				this.CurrentWorkflowDesignDocData.ValidationController.Validate(this.CurrentWorkflowDesignDocData.GetAllElementsForValidation(), DslValidation::ValidationCategories.Menu);
			}
		}
		
		/// <summary>
		/// Status event handler for validating the current selection.
		/// </summary>
		internal virtual void OnStatusValidate(object sender, System.EventArgs e)
		{
			global::System.ComponentModel.Design.MenuCommand cmd = sender as global::System.ComponentModel.Design.MenuCommand;
			cmd.Visible = cmd.Enabled = false;

			foreach (object selectedObject in this.CurrentSelection)
			{
				DslModeling::ModelElement element = GetValidationTarget(selectedObject);
					
				if (element != null)
				{
					cmd.Visible = cmd.Enabled = true;
					break;
				}
			}
		}

		/// <summary>
		/// Handler for validating the current selection.
		/// </summary>
		internal virtual void OnMenuValidate(object sender, global::System.EventArgs e)
		{
			if (this.CurrentWorkflowDesignDocData != null && this.CurrentWorkflowDesignDocData.Store != null)
			{
				System.Collections.Generic.List<DslModeling::ModelElement> elementList = new System.Collections.Generic.List<Microsoft.VisualStudio.Modeling.ModelElement>();
				DslModeling::DepthFirstElementWalker elementWalker = new DslModeling::DepthFirstElementWalker(new ValidateCommandVisitor(elementList), new DslModeling::EmbeddingVisitorFilter(), true);
				foreach (object selectedObject in this.CurrentSelection)
				{
					// Build list of elements embedded beneath the selected root.
					DslModeling::ModelElement element = GetValidationTarget(selectedObject);
					if (element != null && !elementList.Contains(element))
					{
						elementWalker.DoTraverse(element);
					}
				}

				this.CurrentWorkflowDesignDocData.ValidationController.Validate(elementList, DslValidation::ValidationCategories.Menu);
			}
		}
		
		/// <summary>
		/// Helper method to retrieve the target root element for validation from the selected object.
		/// </summary>
		protected static DslModeling::ModelElement GetValidationTarget(object selectedObject)
		{
			DslModeling::ModelElement element = null;
			DslDiagrams::PresentationElement presentation = selectedObject as DslDiagrams::PresentationElement;
			if (presentation != null)
			{
				if (presentation.Subject != null)
				{
					element = presentation.Subject;
				}
			}
			else
			{
				element = selectedObject as DslModeling::ModelElement;
			}
			return element;
		}
		
		#region ValidateCommandVisitor
		/// <summary>
		/// Helper class for building the list of elements to validate when the Validate command is executed.
		/// </summary>
		protected sealed class ValidateCommandVisitor : DslModeling::IElementVisitor
		{
			private System.Collections.Generic.IList<DslModeling::ModelElement> validationList;
 
			/// <summary>
			/// Construct a ValidateCommandVisitor that adds elements to be validated to the specified list.
			/// </summary>
			public ValidateCommandVisitor(System.Collections.Generic.IList<DslModeling::ModelElement> elementList)
			{
				this.validationList = elementList;
			}

			/// <summary>
			/// Called when traversal begins. 
			/// </summary>
			public void StartTraverse(Microsoft.VisualStudio.Modeling.ElementWalker walker) { }

			/// <summary>
			/// Called when traversal ends. 
			/// </summary>
			public void EndTraverse(Microsoft.VisualStudio.Modeling.ElementWalker walker) { }
			
			/// <summary>
			/// Called for each element in the traversal.
			/// </summary>
			public bool Visit(Microsoft.VisualStudio.Modeling.ElementWalker walker, Microsoft.VisualStudio.Modeling.ModelElement element)
			{
				this.validationList.Add(element);
				return true;
			}
		}
		#endregion

		/// <summary>
		/// Returns the currently focused document.
		/// </summary>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		protected WorkflowDesignDocData CurrentWorkflowDesignDocData
		{
			get
			{
				return this.MonitorSelection.CurrentDocument as WorkflowDesignDocData;
			}
		}

		/// <summary>
		/// Returns the currently focused document view.
		/// </summary>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		protected WorkflowDesignDocView CurrentWorkflowDesignDocView
		{
			get
			{
				return this.MonitorSelection.CurrentDocumentView as WorkflowDesignDocView;
			}
		}

	}
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal partial class WorkflowDesignClipboardCommandSet : WorkflowDesignClipboardCommandSetBase
	{
		/// <summary>
		/// Constructs a new WorkflowDesignClipboardCommandSet.
		/// </summary>
		public WorkflowDesignClipboardCommandSet(global::System.IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}
	}

	/// <summary>
	/// Class containing handlers for cut/copy/paste commands supported by this DSL.
	/// </summary>
	internal abstract partial class WorkflowDesignClipboardCommandSetBase : DslShell::ClipboardCommandSet
	{
		/// <summary>
		/// Constructs a new WorkflowDesignClipboardCommandSetBase.
		/// </summary>
		protected WorkflowDesignClipboardCommandSetBase(global::System.IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}

		/// <summary>
		/// Provide the menu commands that this command set handles
		/// </summary>
		protected override global::System.Collections.Generic.IList<global::System.ComponentModel.Design.MenuCommand> GetMenuCommands()
		{
			// Get the standard commands
			var commands = new global::System.Collections.Generic.List<global::System.ComponentModel.Design.MenuCommand>(3);

			global::System.ComponentModel.Design.MenuCommand menuCommand;

			menuCommand = new DslShell::DynamicStatusMenuCommand(
				new global::System.EventHandler(this.OnStatusCut),
				new global::System.EventHandler(this.OnMenuCut),
				global::System.ComponentModel.Design.StandardCommands.Cut);
			commands.Add(menuCommand);

			menuCommand = new DslShell::DynamicStatusMenuCommand(
				new global::System.EventHandler(this.OnStatusCopy),
				new global::System.EventHandler(this.OnMenuCopy),
				global::System.ComponentModel.Design.StandardCommands.Copy);
			commands.Add(menuCommand);

			menuCommand = new DslShell::DynamicStatusMenuCommand(
				new global::System.EventHandler(this.OnStatusPaste),
				new global::System.EventHandler(this.OnMenuPaste),
				global::System.ComponentModel.Design.StandardCommands.Paste);
			commands.Add(menuCommand);

			return commands;
		}

		/// <summary>
		/// Determines whether Cut menu item should be visible and if so, enabled.
		/// </summary>
		/// <param name="sender">The sender of the message</param>
		/// <param name="args">empty</param>
		private void OnStatusCut(object sender, global::System.EventArgs args)
		{
			System.ComponentModel.Design.MenuCommand cmd = sender as System.ComponentModel.Design.MenuCommand;
			this.ProcessOnStatusCutCommand(cmd);
		}

		/// <summary>
		/// Determines whether Copy menu item should be visible and if so, enabled.
		/// </summary>
		/// <param name="sender">The sender of the message</param>
		/// <param name="args">empty</param>
		private void OnStatusCopy(object sender, global::System.EventArgs args)
		{
			System.ComponentModel.Design.MenuCommand cmd = sender as System.ComponentModel.Design.MenuCommand;
			this.ProcessOnStatusCopyCommand(cmd);
		}

		/// <summary>
		/// Updates the UI for the Paste command
		/// </summary>
		/// <param name="sender">The sender of the message</param>
		/// <param name="args">Message parameters</param>
		private void OnStatusPaste(object sender, global::System.EventArgs args)
		{
			System.ComponentModel.Design.MenuCommand cmd = sender as System.ComponentModel.Design.MenuCommand;
			this.ProcessOnStatusPasteCommand(cmd);
		}

		/// <summary>
		/// Event handler to cut the selected objects to the clipboard then delete the original.
		/// </summary>
		/// <param name="sender">The MenuCommand selected.</param>
		/// <param name="args">not used</param>
		private void OnMenuCut(object sender, global::System.EventArgs args)
		{
			this.ProcessOnMenuCutCommand();
		}

		/// <summary>
		/// Event handler to copy the selected objects to the clipboard.
		/// </summary>
		/// <param name="sender">The MenuCommand selected.</param>
		/// <param name="args">not used</param>
		private void OnMenuCopy(object sender, global::System.EventArgs args)
		{
			this.ProcessOnMenuCopyCommand();
		}

		/// <summary>
		/// Event handler to paste a copy of the object on the clipboard.
		/// </summary>
		/// <param name="sender">The MenuCommand selected.</param>
		/// <param name="args">not used</param>
		private void OnMenuPaste(object sender, global::System.EventArgs args)
		{
			this.ProcessOnMenuPasteCommand();
		}
	}
}

