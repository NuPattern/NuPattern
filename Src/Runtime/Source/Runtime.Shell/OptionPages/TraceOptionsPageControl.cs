using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using NuPattern.Runtime.Settings;
using NuPattern.Runtime.Shell.Properties;

namespace NuPattern.Runtime.Shell.OptionPages
{
    /// <summary>
    /// Trace Options Page Control
    /// </summary>
    [CLSCompliant(false)]
    public partial class TraceOptionsPageControl : UserControl
    {
        private static Dictionary<string, SourceLevels> levels = Enum.GetValues(typeof(SourceLevels))
                    .OfType<SourceLevels>()
                    .Select(v => new { Key = Enum.GetName(typeof(SourceLevels), v), Value = v })
                    .Where(v => v.Value != SourceLevels.ActivityTracing && v.Value != SourceLevels.Critical)
                    .OrderBy(v => v.Value, new SourceLevelComparer())
                    .ToDictionary(v => v.Key, v => v.Value);

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceOptionsPageControl"/> class.
        /// </summary>
        public TraceOptionsPageControl()
        {
            InitializeComponent();
            InitializeDropDowns();
            InitializeListBox();
        }

        internal TraceOptionsPage TraceOptionsPage { get; set; }

        internal void BoundControls()
        {
            BoundListBox();
            BoundCombo();
        }

        private void BoundCombo()
        {
            this.RootSourceLevel.SelectedValue = this.TraceOptionsPage.RuntimeSettings.Tracing.RootSourceLevel;
        }

        private void BoundListBox()
        {
            this.TraceOptionsPage.RuntimeSettings.Tracing.TraceSources.ForEach(
                setting => AddItem(setting.SourceName, setting.LoggingLevel.ToString()));
        }

        private void InitializeListBox()
        {
            this.lstLevels.View = View.Details;

            this.lstLevels.Columns.Add("", 30, HorizontalAlignment.Left);
            this.lstLevels.Columns.Add(Resources.TraceOptionsPageControl_SourceNameTitle, 200, HorizontalAlignment.Left);
            this.lstLevels.Columns.Add(Resources.TraceOptionsPageControl_LoggingLevelTitle, 100, HorizontalAlignment.Left);

            this.lstLevels.Items.Clear();
        }

        private void InitializeDropDowns()
        {
            this.RootSourceLevel.DataSource = new BindingSource(levels, null);
            this.RootSourceLevel.DisplayMember = "Key";
            this.RootSourceLevel.ValueMember = "Value";

            this.cmbLevel.DataSource = new BindingSource(levels, null);
            this.cmbLevel.DisplayMember = "Key";
            this.cmbLevel.ValueMember = "Value";
        }

        private void ClearInputControls()
        {
            this.txtSourceName.Text = string.Empty;
            this.cmbLevel.SelectedIndex = 0;
        }

        private void AddItem(string sourceName, string level)
        {
            var item = new ListViewItem() { ImageIndex = 0 };
            var subtItem1 = new ListViewItem.ListViewSubItem() { Text = sourceName };
            var subtItem2 = new ListViewItem.ListViewSubItem() { Text = level };

            item.SubItems.Add(subtItem1);
            item.SubItems.Add(subtItem2);

            lstLevels.Items.Add(item);
        }

        private void RootSourceLevel_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.TraceOptionsPage.RuntimeSettings.Tracing.RootSourceLevel =
                (SourceLevels)this.RootSourceLevel.SelectedValue;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.errorProvider.SetError(this.txtSourceName, string.Empty);

            if (string.IsNullOrEmpty(this.txtSourceName.Text))
            {
                this.errorProvider.SetError(this.txtSourceName, Resources.TraceOptionsPageControl_SourceNameRequired);
            }
            else
            {
                if (!NuPattern.VisualStudio.Solution.DataFormats.IsValidCSharpIdentifier(this.txtSourceName.Text) &&
                    !NuPattern.VisualStudio.Solution.DataFormats.DesignTime.IsValidNamespaceIdentifier(this.txtSourceName.Text))
                {
                    this.errorProvider.SetError(this.txtSourceName, Resources.TraceOptionsPageControl_SourceNameInvalid);

                    return;
                }

                AddItem(this.txtSourceName.Text, ((SourceLevels)this.cmbLevel.SelectedValue).ToString());

                this.TraceOptionsPage.RuntimeSettings.Tracing.TraceSources.Add(
                    new TraceSourceSetting(this.txtSourceName.Text, (SourceLevels)this.cmbLevel.SelectedValue));

                ClearInputControls();
            }
        }

        private void lstLevels_MouseClick(object sender, MouseEventArgs e)
        {
            var info = lstLevels.HitTest(e.X, e.Y);

            if (info.Location == ListViewHitTestLocations.Image)
            {
                var item = this.TraceOptionsPage.RuntimeSettings.Tracing.TraceSources.FirstOrDefault(
                    source => source.SourceName == info.Item.SubItems[1].Text &&
                        source.LoggingLevel == (SourceLevels)Enum.Parse(typeof(SourceLevels), info.Item.SubItems[2].Text));

                if (item != null)
                {
                    this.TraceOptionsPage.RuntimeSettings.Tracing.TraceSources.Remove(item);
                }

                this.lstLevels.Items.Remove(info.Item);
            }
        }

        private void lstLevels_MouseMove(object sender, MouseEventArgs e)
        {
            this.lstLevels.Cursor = Cursors.Default;

            var info = lstLevels.HitTest(e.X, e.Y);

            if (info.Location == ListViewHitTestLocations.Image)
            {
                this.lstLevels.Cursor = Cursors.Hand;
            }
        }

        private class SourceLevelComparer : IComparer<SourceLevels>
        {
            public int Compare(SourceLevels x, SourceLevels y)
            {
                var left = (int)x;
                var right = (int)y;

                if (right == (int)SourceLevels.All)
                {
                    return -1;
                }
                else
                {
                    if (left > right)
                    {
                        return 1;
                    }
                    else
                    {
                        if (left > right)
                        {
                            return -1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
        }
    }
}