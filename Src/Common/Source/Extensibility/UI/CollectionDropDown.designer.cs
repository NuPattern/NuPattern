namespace NuPattern.Extensibility
{
	partial class CollectionDropDown<TValue>
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
		private void InitializeComponent()
		{
			this.ListView = new System.Windows.Forms.ListView();
			this.Accept = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.Description = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// ListView
			// 
			this.ListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ListView.Location = new System.Drawing.Point(4, 4);
			this.ListView.Name = "ListView";
			this.ListView.Size = new System.Drawing.Size(229, 113);
			this.ListView.TabIndex = 0;
			this.ListView.UseCompatibleStateImageBehavior = false;
			this.ListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView_ItemSelectionChanged);
			// 
			// Accept
			// 
			this.Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Accept.Location = new System.Drawing.Point(130, 148);
			this.Accept.Name = "Accept";
			this.Accept.Size = new System.Drawing.Size(50, 23);
			this.Accept.TabIndex = 1;
			this.Accept.Text = "&Accept";
			this.Accept.UseVisualStyleBackColor = true;
			this.Accept.Click += new System.EventHandler(this.Accept_Click);
			// 
			// Cancel
			// 
			this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Cancel.Location = new System.Drawing.Point(183, 148);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(50, 23);
			this.Cancel.TabIndex = 2;
			this.Cancel.Text = "&Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// Description
			// 
			this.Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.Description.AutoSize = true;
			this.Description.Location = new System.Drawing.Point(4, 120);
			this.Description.Name = "Description";
			this.Description.Size = new System.Drawing.Size(60, 13);
			this.Description.TabIndex = 3;
			this.Description.Text = "Description";
			// 
			// CollectionDropDown
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.Description);
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.Accept);
			this.Controls.Add(this.ListView);
			this.Name = "CollectionDropDown";
			this.Size = new System.Drawing.Size(236, 177);
			this.Load += new System.EventHandler(this.AcceptCancelListView_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView ListView;
		private System.Windows.Forms.Button Accept;
		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.Label Description;
	}
}
