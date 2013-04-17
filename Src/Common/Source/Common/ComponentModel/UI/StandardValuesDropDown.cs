using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace NuPattern.ComponentModel.UI
{
    partial class StandardValuesDropDown : UserControl
    {
        // The list of values uses the 60% of the drop down space
        private const double ValuesDisplayPercentage = 0.7;

        private string searchText;
        private bool searching = false;
        private List<StandardValue> values = new List<StandardValue>();
        private IWindowsFormsEditorService editorService;

        public StandardValuesDropDown(IWindowsFormsEditorService editorService)
            : this(editorService, new object[0])
        {
        }

        public StandardValuesDropDown(IWindowsFormsEditorService editorService, ICollection values)
        {
            InitializeComponent();

            this.editorService = editorService;
            UpdateSearchTextBoxState();

            foreach (var value in values)
            {
                var standardValue = value as StandardValue;

                if (standardValue == null)
                    standardValue = new StandardValue(GetDisplayTextFromValue(value), value);

                this.Add(standardValue, false);
            }

            this.values = this.values.OrderBy(value => value.DisplayText).ToList();

            this.UpdateFilteredListboxItems();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            DoSizeChanged();
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            DoSizeChanged();
        }

        private void DoSizeChanged()
        {
            // Giving some % of the space to the list and the rest to the Description+Search
            this.listViewValues.Height = (int)(this.Height * ValuesDisplayPercentage);
            this.panelBottom.Height = this.Height - this.listViewValues.Height;

            // Setting the Tile width to avoid multiple items in the same row
            // And also setting the width to the 90% to avoid the horizontal scrollbar
            this.listViewValues.TileSize = new Size((int)(this.listViewValues.Width * 0.9), this.listViewValues.TileSize.Height);

            // Updating the available space for the description
            this.labelDescription.Height = this.panelBottom.Height - this.labelDescriptionTitle.Height - this.textBoxSearch.Height;
        }

        private string GetDisplayTextFromValue(object value)
        {
            var converter = TypeDescriptor.GetConverter(value);

            return converter.ConvertToString(value);
        }

        public void Add(StandardValue value)
        {
            this.Add(value, true);
        }

        public void Add(StandardValue value, bool shouldUpdateListBoxItems)
        {
            if (!string.IsNullOrEmpty(value.DisplayText))
            {
                this.values.Add(value);

                if (shouldUpdateListBoxItems)
                    this.UpdateFilteredListboxItems();
            }
        }

        public object SelectedValue
        {
            get
            {
                var selectedStandardValue = this.SelectedStandardValue;

                return selectedStandardValue != null ? selectedStandardValue.Value : null;
            }
        }

        public StandardValue SelectedStandardValue
        {
            get
            {
                return this.listViewValues.SelectedItems
                    .OfType<ListViewItem>()
                    .Select(item => item.Tag)
                    .OfType<StandardValue>()
                    .FirstOrDefault();
            }
        }

        private void UpdateSearchTextBoxState()
        {
            if (string.IsNullOrEmpty(this.searchText) && !searching)
            {
                this.textBoxSearch.Text = "Search";
                this.textBoxSearch.Font = new Font(this.textBoxSearch.Font, FontStyle.Italic);
                this.textBoxSearch.ForeColor = Color.Gray;
            }
            else
            {
                if (this.searchText != this.textBoxSearch.Text)
                    this.textBoxSearch.Text = string.Empty;

                this.textBoxSearch.Font = new Font(this.textBoxSearch.Font, FontStyle.Regular);
                this.textBoxSearch.ForeColor = Color.Black;
            }
        }

        private void OnTextBoxSearchEnter(object sender, EventArgs e)
        {
            searching = true;
            UpdateSearchTextBoxState();
        }

        private void OnTextBoxSearchLeave(object sender, EventArgs e)
        {
            searching = false;
            UpdateSearchTextBoxState();
        }

        private void OnTextBoxSearchTextChanged(object sender, EventArgs e)
        {
            if (this.searching)
            {
                this.searchText = this.textBoxSearch.Text;
                this.UpdateFilteredListboxItems();
            }
        }

        private void UpdateFilteredListboxItems()
        {
            var tokens = new List<string>();

            if (!string.IsNullOrEmpty(this.searchText))
                tokens = this.searchText
                    .Split(" ".ToCharArray())
                    .Select(token => token.Trim().ToLower())
                    .ToList();

            foreach (var value in values)
            {
                bool valueShouldBeShown =
                    tokens.Count == 0 ||
                    tokens.All(token =>
                        value.DisplayText.ToLower().Contains(token) ||
                        value.Description.ToLower().Contains(token) ||
                        value.Group.ToLower().Contains(token));

                var listViewItem = this.listViewValues.Items.OfType<ListViewItem>().FirstOrDefault(item => item.Tag == value);

                if (valueShouldBeShown && listViewItem == null)
                {
                    var group = this.listViewValues.Groups.OfType<ListViewGroup>().FirstOrDefault(g => g.Name == value.Group);

                    if (group == null)
                    {
                        group = new ListViewGroup(value.Group);
                        group.Name = value.Group;
                        group.HeaderAlignment = HorizontalAlignment.Center;

                        this.listViewValues.Groups.Add(group);
                    }

                    this.listViewValues.Items.Add(new ListViewItem(value.DisplayText) { Tag = value, ToolTipText = value.Description, Group = group });
                }
                else if (!valueShouldBeShown && listViewItem != null)
                {
                    this.listViewValues.Items.Remove(listViewItem);
                }

                var emptyGroupsToBeRemoved = this.listViewValues.Groups
                    .OfType<ListViewGroup>()
                    .Where(group => group.Items.Count == 0);

                emptyGroupsToBeRemoved.ToList().ForEach(group => this.listViewValues.Groups.Remove(group));
            }
        }

        private void OnListViewValuesSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedStandardValue = this.SelectedStandardValue;

            this.labelDescription.Text = selectedStandardValue != null ? selectedStandardValue.Description : string.Empty;
        }

        private void OnTextBoxSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (this.listViewValues.Items.Count > 0)
                {
                    var nextSelectedListViewItem = this.listViewValues.Items[0];

                    // Select the previous/next item in the list
                    var selectedListViewItem = this.listViewValues.SelectedItems.OfType<ListViewItem>().FirstOrDefault();

                    if (selectedListViewItem != null)
                    {
                        nextSelectedListViewItem = selectedListViewItem;

                        if (e.KeyCode == Keys.Down)
                        {
                            var index = selectedListViewItem.Group.Items.IndexOf(selectedListViewItem) + 1;
                            if (index < selectedListViewItem.Group.Items.Count)
                            {
                                nextSelectedListViewItem = selectedListViewItem.Group.Items[index];
                            }
                            else
                            {
                                var groupIndex = listViewValues.Groups.IndexOf(selectedListViewItem.Group) + 1;
                                if (groupIndex < listViewValues.Groups.Count)
                                {
                                    nextSelectedListViewItem = listViewValues.Groups[groupIndex].Items.OfType<ListViewItem>().First();
                                }
                            }
                        }
                        else
                        {
                            var index = selectedListViewItem.Group.Items.IndexOf(selectedListViewItem) - 1;
                            if (index >= 0)
                            {
                                nextSelectedListViewItem = selectedListViewItem.Group.Items[index];
                            }
                            else
                            {
                                var groupIndex = listViewValues.Groups.IndexOf(selectedListViewItem.Group) - 1;
                                if (groupIndex >= 0)
                                {
                                    nextSelectedListViewItem = listViewValues.Groups[groupIndex].Items.OfType<ListViewItem>().Last();
                                }
                            }
                        }

                        selectedListViewItem.Selected = false;
                    }

                    nextSelectedListViewItem.Selected = true;
                    nextSelectedListViewItem.EnsureVisible();
                }

                e.Handled = true;
            }
        }

        private void OnListViewValuesMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = this.listViewValues.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                this.editorService.CloseDropDown();
            }
        }
    }
}