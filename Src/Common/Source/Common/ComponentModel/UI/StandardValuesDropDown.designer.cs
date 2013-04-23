namespace NuPattern.ComponentModel.UI
{
	partial class StandardValuesDropDown
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
			this.panelBottom = new System.Windows.Forms.Panel();
			this.labelDescription = new System.Windows.Forms.Label();
			this.textBoxSearch = new System.Windows.Forms.TextBox();
			this.labelDescriptionTitle = new System.Windows.Forms.Label();
			this.listViewValues = new System.Windows.Forms.ListView();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.labelDescription);
			this.panelBottom.Controls.Add(this.textBoxSearch);
			this.panelBottom.Controls.Add(this.labelDescriptionTitle);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 147);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(356, 107);
			this.panelBottom.TabIndex = 1;
			// 
			// labelDescription
			// 
			this.labelDescription.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelDescription.Location = new System.Drawing.Point(0, 13);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(356, 13);
			this.labelDescription.TabIndex = 2;
			// 
			// textBoxSearch
			// 
			this.textBoxSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textBoxSearch.Location = new System.Drawing.Point(0, 87);
			this.textBoxSearch.Name = "textBoxSearch";
			this.textBoxSearch.Size = new System.Drawing.Size(356, 20);
			this.textBoxSearch.TabIndex = 1;
			this.textBoxSearch.TextChanged += new System.EventHandler(this.OnTextBoxSearchTextChanged);
			this.textBoxSearch.Enter += new System.EventHandler(this.OnTextBoxSearchEnter);
			this.textBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnTextBoxSearchKeyDown);
			this.textBoxSearch.Leave += new System.EventHandler(this.OnTextBoxSearchLeave);
			// 
			// labelDescriptionTitle
			// 
			this.labelDescriptionTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelDescriptionTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDescriptionTitle.Location = new System.Drawing.Point(0, 0);
			this.labelDescriptionTitle.Name = "labelDescriptionTitle";
			this.labelDescriptionTitle.Size = new System.Drawing.Size(356, 13);
			this.labelDescriptionTitle.TabIndex = 0;
			this.labelDescriptionTitle.Text = "Description";
			// 
			// listViewValues
			// 
			this.listViewValues.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listViewValues.Dock = System.Windows.Forms.DockStyle.Top;
			this.listViewValues.HideSelection = false;
			this.listViewValues.Location = new System.Drawing.Point(0, 0);
			this.listViewValues.MultiSelect = false;
			this.listViewValues.Name = "listViewValues";
			this.listViewValues.Size = new System.Drawing.Size(356, 148);
			this.listViewValues.TabIndex = 2;
			this.listViewValues.TileSize = new System.Drawing.Size(168, 20);
			this.listViewValues.UseCompatibleStateImageBehavior = false;
			this.listViewValues.View = System.Windows.Forms.View.Tile;
			this.listViewValues.SelectedIndexChanged += new System.EventHandler(this.OnListViewValuesSelectedIndexChanged);
			this.listViewValues.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnListViewValuesMouseDoubleClick);
			// 
			// StandardValuesDropDown
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listViewValues);
			this.Controls.Add(this.panelBottom);
			this.Name = "StandardValuesDropDown";
			this.Size = new System.Drawing.Size(356, 254);
			this.panelBottom.ResumeLayout(false);
			this.panelBottom.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.TextBox textBoxSearch;
		private System.Windows.Forms.Label labelDescriptionTitle;
		private System.Windows.Forms.ListView listViewValues;

	}
}
