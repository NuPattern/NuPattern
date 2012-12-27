using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// MultipleValuesDropDown control wiht accept and cancel buttons
	/// </summary>
	public partial class CollectionDropDown<TValue> : UserControl
	{
		/// <summary>
		/// Occurs when [on accept].
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value")]
		public event EventHandler OnAccept = (sender, args) => { };
		/// <summary>
		/// Occurs when [on cancel].
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value")]
		public event EventHandler OnCancel = (sender, args) => { };

		private List<ItemValue<TValue>> values = new List<ItemValue<TValue>>();
		private ICollection<TValue> previousValues;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionDropDown&lt;TValue&gt;"/> class.
		/// </summary>
		public CollectionDropDown()
			: this(null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionDropDown&lt;TValue&gt;"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="previousValues">The previous values.</param>
		public CollectionDropDown(ITypeDescriptorContext context, ICollection<TValue> previousValues)
		{
			Guard.NotNull(() => context, context);

			InitializeComponent();
			this.previousValues = previousValues;
			InitializeValueList(context.PropertyDescriptor.Converter.GetStandardValues(context));
		}

		/// <summary>
		/// Gets the checked values.
		/// </summary>
		/// <value>The checked values.</value>
		public ICollection<TValue> CheckedValues
		{
			get
			{
				return this.ListView.CheckedItems
					.OfType<ListViewItem>()
					.Select(item => ((ItemValue<TValue>)item.Tag).Value).ToList();
			}
		}

		private void InitializeValueList(ICollection vals)
		{
			foreach (var val in vals)
			{
				var standardValue = val as ItemValue<TValue>;

				if (standardValue == null)
				{
					var descriptionAtt =
						TypeDescriptor.GetAttributes(val)
							.OfType<DescriptionAttribute>()
							.Select(att => att.Description)
							.FirstOrDefault();

					descriptionAtt = descriptionAtt ?? string.Empty;

					var displayNameAtt =
						TypeDescriptor.GetAttributes(val)
							.OfType<DisplayNameAttribute>()
							.Select(att => att.DisplayName)
							.FirstOrDefault();

					displayNameAtt = displayNameAtt ?? TypeDescriptor.GetConverter(val).ConvertToString(val);

					standardValue = new ItemValue<TValue>(descriptionAtt, displayNameAtt, (TValue)val);

					this.values.Add(standardValue);
				}
			}

			this.values = this.values.OrderBy(v => v.DisplayText).ToList();
		}

		private void InitializeItems()
		{
			foreach (var val in this.values)
			{
				var item = new ListViewItem(val.DisplayText) { Tag = val };

				if (this.previousValues != null)
				{
					item.Checked = this.previousValues.Any(v => v.Equals(val.Value));
				}

				this.ListView.Items.Add(item);
			}
		}

		private void Accept_Click(object sender, EventArgs e)
		{
			OnAccept(sender, e);
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			OnCancel(sender, e);
		}

		private void AcceptCancelListView_Load(object sender, EventArgs e)
		{
			this.ListView.CheckBoxes = true;
			this.ListView.Sorting = SortOrder.Ascending;
			this.ListView.HeaderStyle = ColumnHeaderStyle.None;
			this.ListView.View = View.List;
			this.Description.Text = string.Empty;

			InitializeItems();
		}

		private void ListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			this.Description.Text = ((ItemValue<TValue>)e.Item.Tag).Description;
		}
	}
}