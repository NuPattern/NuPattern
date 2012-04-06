namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	internal partial class ImageDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageDialog));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.fileNameComboBox = new System.Windows.Forms.ComboBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.fileNameComboBox, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.okButton, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.cancelButton, 2, 3);
			this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// fileNameComboBox
			// 
			this.fileNameComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.fileNameComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
			this.tableLayoutPanel1.SetColumnSpan(this.fileNameComboBox, 4);
			resources.ApplyResources(this.fileNameComboBox, "fileNameComboBox");
			this.fileNameComboBox.Name = "fileNameComboBox";
			this.fileNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.FileNameComboBox_SelectionChangeCommitted);
			this.fileNameComboBox.SelectedIndexChanged += new System.EventHandler(this.FileNameComboBox_SelectedIndexChanged);
			this.fileNameComboBox.TextChanged += new System.EventHandler(this.FileNameComboBox_TextChanged);
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.cancelButton, 2);
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel1.SetColumnSpan(this.pictureBox1, 4);
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// openFileDialog
			// 
			resources.ApplyResources(this.openFileDialog, "openFileDialog");
			// 
			// ImageResourceDialog
			// 
			this.AcceptButton = this.okButton;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Dialog;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.tableLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImageResourceDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox fileNameComboBox;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
	}
}