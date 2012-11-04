using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// A simple collection editor that supports cancelling of editing.
	/// </summary>
	public class CancelableCollectionEditor : CollectionEditor
	{
		private string caption;

		/// <summary>
		/// Initializes a new instance of the <see cref="CancelableCollectionEditor"/> class.
		/// </summary>
		/// <param name="type">The type of the collection for this editor to edit.</param>
		public CancelableCollectionEditor(Type type)
			: base(type)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CancelableCollectionEditor"/> class.
		/// </summary>
		/// <param name="type">The type of the collection for this editor to edit.</param>
		/// <param name="caption">The title for the editor window.</param>
		public CancelableCollectionEditor(Type type, string caption)
			: base(type)
		{
			this.caption = caption;
		}

		/// <summary>
		/// Gets a value determining whether the editor was cancelled.
		/// </summary>
		protected bool IsCanceled { get; private set; }

		/// <summary>
		/// Edits the value of the specified object using the specified service provider and context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
		/// <param name="provider">A service provider object through which editing services can be obtained.</param>
		/// <param name="value">The object to edit the value of.</param>
		/// <returns>
		/// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
		/// </returns>
		/// <exception cref="CheckoutException">An attempt to check out a file that is checked into a source code management program did not succeed.</exception>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			Guard.NotNull(() => context, context);

			this.IsCanceled = false;
			var obj = base.EditValue(context, provider, value);
			if (!this.IsCanceled)
			{
				if (obj != null && !context.PropertyDescriptor.PropertyType.IsAssignableFrom(obj.GetType()))
					context.PropertyDescriptor.SetValue(context.Instance, context.PropertyDescriptor.Converter.ConvertFrom(context, CultureInfo.CurrentCulture, obj));
				else
					context.PropertyDescriptor.SetValue(context.Instance, obj);
			}

			return obj;
		}

		/// <summary>
		/// Cancels changes to the collection.
		/// </summary>
		protected override void CancelChanges()
		{
			base.CancelChanges();
			this.IsCanceled = true;
		}

		/// <summary>
		/// Creates a new form to display and edit the current collection, with the help 
		/// panel on the properties grid turned on.
		/// </summary>
		protected override CollectionEditor.CollectionForm CreateCollectionForm()
		{
			var form = base.CreateCollectionForm();
			if (!string.IsNullOrEmpty(this.caption))
			{
				form.Text = this.caption;
			}

			form.Controls.Cast<Control>()
				.Traverse(control => control.Controls.Cast<Control>())
				.OfType<PropertyGrid>()
				.First()
				.HelpVisible = true;

			return form;
		}
	}
}