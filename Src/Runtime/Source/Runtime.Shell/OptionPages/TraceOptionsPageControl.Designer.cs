namespace Microsoft.VisualStudio.Patterning.Runtime.Shell.OptionPages
{
	partial class TraceOptionsPageControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TraceOptionsPageControl));
			this.RootSourceLevel = new System.Windows.Forms.ComboBox();
			this.lblRootSourceLevel = new System.Windows.Forms.Label();
			this.txtSourceName = new System.Windows.Forms.TextBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.lstLevels = new System.Windows.Forms.ListView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.cmbLevel = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// RootSourceLevel
			// 
			this.RootSourceLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RootSourceLevel.FormattingEnabled = true;
			this.RootSourceLevel.Location = new System.Drawing.Point(114, 8);
			this.RootSourceLevel.Name = "RootSourceLevel";
			this.RootSourceLevel.Size = new System.Drawing.Size(121, 21);
			this.RootSourceLevel.TabIndex = 1;
			this.RootSourceLevel.SelectionChangeCommitted += new System.EventHandler(this.RootSourceLevel_SelectionChangeCommitted);
			// 
			// lblRootSourceLevel
			// 
			this.lblRootSourceLevel.AutoSize = true;
			this.lblRootSourceLevel.Location = new System.Drawing.Point(11, 11);
			this.lblRootSourceLevel.Name = "lblRootSourceLevel";
			this.lblRootSourceLevel.Size = new System.Drawing.Size(104, 13);
			this.lblRootSourceLevel.TabIndex = 0;
			this.lblRootSourceLevel.Text = "Default Trace Level:";
			// 
			// txtSourceName
			// 
			this.txtSourceName.Location = new System.Drawing.Point(6, 26);
			this.txtSourceName.Name = "txtSourceName";
			this.txtSourceName.Size = new System.Drawing.Size(202, 20);
			this.txtSourceName.TabIndex = 0;
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(340, 27);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(41, 20);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// lstLevels
			// 
			this.lstLevels.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.lstLevels.FullRowSelect = true;
			this.lstLevels.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lstLevels.Location = new System.Drawing.Point(6, 52);
			this.lstLevels.MultiSelect = false;
			this.lstLevels.Name = "lstLevels";
			this.lstLevels.Size = new System.Drawing.Size(375, 140);
			this.lstLevels.SmallImageList = this.imageList;
			this.lstLevels.TabIndex = 3;
			this.lstLevels.UseCompatibleStateImageBehavior = false;
			this.lstLevels.View = System.Windows.Forms.View.SmallIcon;
			this.lstLevels.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstLevels_MouseClick);
			this.lstLevels.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lstLevels_MouseMove);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "Delete.ico");
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// cmbLevel
			// 
			this.cmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbLevel.FormattingEnabled = true;
			this.cmbLevel.Location = new System.Drawing.Point(214, 26);
			this.cmbLevel.Name = "cmbLevel";
			this.cmbLevel.Size = new System.Drawing.Size(121, 21);
			this.cmbLevel.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cmbLevel);
			this.groupBox1.Controls.Add(this.txtSourceName);
			this.groupBox1.Controls.Add(this.lstLevels);
			this.groupBox1.Controls.Add(this.btnAdd);
			this.groupBox1.Location = new System.Drawing.Point(9, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(390, 198);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Additional Trace Entries:";
			// 
			// TraceOptionsPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.RootSourceLevel);
			this.Controls.Add(this.lblRootSourceLevel);
			this.Controls.Add(this.groupBox1);
			this.Name = "TraceOptionsPageControl";
			this.Size = new System.Drawing.Size(406, 251);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox RootSourceLevel;
		private System.Windows.Forms.Label lblRootSourceLevel;
		private System.Windows.Forms.TextBox txtSourceName;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.ListView lstLevels;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ComboBox cmbLevel;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
