using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
	/// <summary>
	/// Creates a new instance of a child element for each picked file.
	/// </summary>
	[CLSCompliant(false)]
	public abstract class CreateElementFromPickedFileCommand : CreateElementFromFileCommand
	{
		private static readonly ITraceSource tracer = Tracer.GetSourceFor<CreateElementFromPickedFileCommand>();

		/// <summary>
		/// The file extensions which are supported.
		/// </summary>
		[Required(AllowEmptyStrings = false)]
		[DisplayNameResource("CreateElementFromPickedFileCommand_Extension_DisplayName", typeof(Resources))]
		[DescriptionResource("CreateElementFromPickedFileCommand_Extension_Description", typeof(Resources))]
		public virtual string Extension { get; set; }

		/// <summary>
		/// The title for the picker
		/// </summary>
		[Required(AllowEmptyStrings = true)]
		[DisplayNameResource("CreateElementFromPickedFileCommand_PickerTitle_DisplayName", typeof(Resources))]
		[DescriptionResource("CreateElementFromPickedFileCommand_PickerTitle_Description", typeof(Resources))]
		public virtual string PickerTitle { get; set; }

		/// <summary>
		/// The initial directory for the picker
		/// </summary>
		[Required(AllowEmptyStrings = true)]
		[DisplayNameResource("CreateElementFromPickedFileCommand_InitialDirectory_DisplayName", typeof(Resources))]
		[DescriptionResource("CreateElementFromPickedFileCommand_InitialDirectory_Description", typeof(Resources))]
		public virtual string InitialDirectory { get; set; }

		/// <summary>
		/// Returns the files from the picker.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<string> GetFilePaths()
		{
			tracer.TraceInformation(
				Resources.CreateElementFromPickedFileCommand_TracePickingFiles, this.Extension, this.InitialDirectory, this.PickerTitle);

			// Set filter, multiselect, title
			// Show file browser
			using (var dialog = new OpenFileDialog())
			{
				dialog.CheckFileExists = dialog.CheckPathExists = true;
				dialog.Filter = GetFilter();
				dialog.InitialDirectory = GetInitialDirectory();
				dialog.Multiselect = true;
				dialog.Title = this.PickerTitle;
				var result = dialog.ShowDialog();
				if (DialogResult.OK == result)
				{
					return dialog.FileNames;
				}
			}

			return Enumerable.Empty<string>();
		}

		private string GetInitialDirectory()
		{
			if (!String.IsNullOrEmpty(this.InitialDirectory)
				&& Directory.Exists(this.InitialDirectory))
			{
				return this.InitialDirectory;
			}
			else
			{
				return string.Empty;
			}
		}

		private string GetFilter()
		{
			List<string> filters = new List<string>();
			var extensions = DragDropHelpers.GetSafeExtensions(this.Extension);
			extensions.ForEach(ext =>
				{
					filters.Add(string.Format(CultureInfo.CurrentCulture, "(*{0})|*{0}", ext));
				});
			filters.Add("All files (*.*)|*.*");

			return string.Join("|", filters);
		}
	}
}
