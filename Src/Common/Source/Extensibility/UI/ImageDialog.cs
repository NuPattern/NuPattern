using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Modeling.Shell;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Image dialog to select an image file.
	/// </summary>
	internal partial class ImageDialog : DialogBase
	{
		private ImageEditor editor;
		private string lastLoadedPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageDialog"/> class.
		/// </summary>
		/// <param name="editor">The editor.</param>
		/// <param name="serviceProvider">The service provider.</param>
		public ImageDialog(ImageEditor editor, IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			this.editor = editor;

			this.InitializeComponent();
			this.Text = this.editor.Filter.DialogTitle;
			this.openFileDialog.Title = this.editor.Filter.DialogTitle;
			this.openFileDialog.Filter = this.editor.Filter.FileFilter;

			if (this.editor.ImageFilePaths != null)
			{
				foreach (var path in this.editor.ImageFilePaths)
				{
					this.fileNameComboBox.Items.Add(path);
				}
			}
		}

		/// <summary>
		/// Gets or sets the image path.
		/// </summary>
		/// <value>The image path.</value>
		public string ImagePath
		{
			get { return this.fileNameComboBox.Text; }
			set { this.fileNameComboBox.Text = value; }
		}

		/// <summary>
		/// Form on load.
		/// </summary>
		/// <param name="e">The EventArgs.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.UpdateState();
		}

		private void FileNameComboBox_TextChanged(object sender, EventArgs e)
		{
			this.UpdateState();
		}

		private void FileNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateState();
		}

		private void FileNameComboBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			this.UpdateState();
		}

		private void UpdateState()
		{
			var imagePath = this.fileNameComboBox.Text;

			if (this.lastLoadedPath == imagePath)
			{
				return;
			}

			this.lastLoadedPath = imagePath;

			this.pictureBox1.SuspendLayout();

			try
			{
				this.okButton.Enabled = imagePath.Length == 0 || this.editor.Filter.IsSupported(imagePath);
				if (this.okButton.Enabled)
				{
					if (ImageEditor.IsCursorFile(imagePath))
					{
						using (Cursor cursor = new Cursor(imagePath))
						{
							this.pictureBox1.Image = new Bitmap(cursor.Size.Width, cursor.Size.Height);

							using (Graphics g = Graphics.FromImage(this.pictureBox1.Image))
							{
								cursor.Draw(g, new Rectangle(Point.Empty, cursor.Size));
							}
						}
					}
					else if (ImageEditor.IsIconFile(imagePath))
					{
						using (Icon icon = new Icon(imagePath))
						{
							this.pictureBox1.Image = icon.ToBitmap();
						}
					}
					else if (imagePath.Length == 0)
					{
						this.pictureBox1.Image = null;
					}
					else
					{
						this.pictureBox1.Image = Image.FromFile(imagePath);
					}
				}
				else
				{
					this.pictureBox1.Image = this.pictureBox1.ErrorImage;
				}
			}
			catch (Exception ex)
			{
				if (ErrorHandler.IsCriticalException(ex))
				{
					throw;
				}

				this.okButton.Enabled = false;
				this.pictureBox1.Image = this.pictureBox1.ErrorImage;
			}

			this.pictureBox1.ResumeLayout();
		}
	}
}