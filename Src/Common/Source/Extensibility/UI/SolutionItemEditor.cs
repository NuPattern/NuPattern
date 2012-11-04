using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Representes the editor to pick a solution item.
	/// </summary>
	[CLSCompliant(false)]
	public class SolutionItemEditor : UITypeEditor
	{
		/// <summary>
		/// Edits the specified object's value using the editor style indicated by the <see cref="UITypeEditor.GetEditStyle()"/> method.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
		/// <param name="provider">An <see cref="IServiceProvider"/> that this editor can use to obtain services.</param>
		/// <param name="value">The object to edit.</param>
		/// <returns>
		/// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
		/// </returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			Guard.NotNull(() => context, context);
			Guard.NotNull(() => provider, provider);

			var componentModel = provider.GetService<SComponentModel, IComponentModel>();
			var picker = componentModel.GetService<Func<ISolutionPicker>>()();
			picker.RootItem = provider.GetService<ISolution>();
			SetOwner(picker);
			SetSettings(context.PropertyDescriptor, picker);
			SetSelectedItem(context, picker, value);

			if (picker.ShowDialog().GetValueOrDefault())
			{
				return ConvertValue(context, provider, picker.SelectedItem);
			}

			return value;
		}

		/// <summary>
		/// Converts the value to the destination type.
		/// </summary>
		protected virtual object ConvertValue(ITypeDescriptorContext context, IServiceProvider provider, IItemContainer value)
		{
			Guard.NotNull(() => context, context);

			var converter = context.PropertyDescriptor.Converter;
			if (converter != null &&
				value != null &&
				!context.PropertyDescriptor.PropertyType.IsAssignableFrom(value.GetType()) &&
				converter.CanConvertTo(context, context.PropertyDescriptor.PropertyType))
			{
				return converter.ConvertTo(context, CultureInfo.CurrentCulture, value, context.PropertyDescriptor.PropertyType);
			}

			return value;
		}

		/// <summary>
		/// Gets the editor style used by the <see cref="UITypeEditor.EditValue(IServiceProvider,Object)"/> method.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
		/// <returns>
		/// A <see cref="UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="UITypeEditor.EditValue(IServiceProvider,Object)"/> method. If the <see cref="UITypeEditor"/> does not support this method, then <see cref="UITypeEditor.GetEditStyle()"/> will return <see cref="UITypeEditorEditStyle.None"/>.
		/// </returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		private static void SetOwner(ISolutionPicker picker)
		{
			if (Application.Current != null)
			{
				picker.Owner = Application.Current.MainWindow;
			}
		}

		private static void SetSelectedItem(ITypeDescriptorContext context, ISolutionPicker picker, object value)
		{
			var item = value as IItemContainer;
			if (item == null)
			{
				var converter = context.PropertyDescriptor.Converter;
				if (converter != null && converter.CanConvertFrom(context.PropertyDescriptor.PropertyType))
				{
					picker.SelectedItem = converter.ConvertFrom(context, CultureInfo.CurrentCulture, value) as IItemContainer;
				}
			}
			else
			{
				picker.SelectedItem = item;
			}
		}

		private static void SetSettings(PropertyDescriptor descriptor, ISolutionPicker picker)
		{
			var settings = descriptor.Attributes.OfType<SolutionEditorSettingsAttribute>().FirstOrDefault();
			if (settings != null)
			{
				if (settings.Title != null)
				{
					picker.Title = settings.Title;
				}

				picker.Filter.Kind = settings.Kind;
				picker.Filter.IncludeFileExtensions = settings.IncludeFileExtensions;
				picker.Filter.IncludeEmptyContainers = settings.IncludeEmptyContainers;
			}
		}
	}
}