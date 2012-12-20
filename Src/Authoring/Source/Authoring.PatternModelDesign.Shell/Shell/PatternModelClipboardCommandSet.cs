using System.ComponentModel.Design;
using System.Linq;

namespace NuPattern.Runtime.Schema
{
	internal partial class PatternModelClipboardCommandSet
	{
		protected override void ProcessOnStatusPasteCommand(System.ComponentModel.Design.MenuCommand cmd)
		{
			if (this.CurrentModelingDocView != null && this.CurrentDocData != null)
			{
				base.ProcessOnStatusPasteCommand(cmd);
			}
		}

		protected override void ProcessOnStatusCopyCommand(MenuCommand cmd)
		{
			if (cmd != null && this.CurrentModelingDocView != null && this.CurrentDocData != null)
			{
				cmd.Visible = true;

				var selection = this.SelectedElements.OfType<NamedElementSchema>();

				if (selection.Count() == this.SelectedElements.Count)
				{
					cmd.Enabled = !selection.Any(n => !string.IsNullOrEmpty(n.BaseId));
				}
				else
				{
					base.ProcessOnStatusCopyCommand(cmd);
				}
			}
		}

		protected override void ProcessOnStatusCutCommand(System.ComponentModel.Design.MenuCommand cmd)
		{
			if (this.CurrentModelingDocView != null && this.CurrentDocData != null)
			{
				base.ProcessOnStatusCutCommand(cmd);
			}
		}
	}
}